using System.Data;
using Aplicacion.Academico.Dtos.Inscripciones;
using Aplicacion.Academico.Errores;
using Aplicacion.Academico.Interfaces;
using Aplicacion.Academico.Mapeos;
using Aplicacion.Academico.Validadores;
using Dominio.Academico.Entidades;
using Dominio.Academico.Politicas;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Inscripciones;

/// <summary>
/// Implementación de las inscripciones del estudiante autenticado. Inscribir corre en una transacción serializable para
/// que dos peticiones simultáneas no superen los límites.
/// </summary>
/// <param name="usuarioActual">Cuenta autenticada.</param>
/// <param name="repositorioEstudiantes">Acceso a los estudiantes.</param>
/// <param name="repositorioMaterias">Acceso a las materias.</param>
/// <param name="unidadTrabajo">Transacciones y confirmación de cambios.</param>
/// <param name="reloj">Fecha actual.</param>
/// <param name="registroEventos">Auditoría.</param>
/// <param name="validador">Reglas de formato de la inscripción.</param>
public sealed class ServicioInscripciones(
    IUsuarioActual usuarioActual,
    IRepositorioEstudiantes repositorioEstudiantes,
    IRepositorioMaterias repositorioMaterias,
    IUnidadTrabajo unidadTrabajo,
    IReloj reloj,
    IRegistroEventos registroEventos,
    IValidator<InscribirMateriaDto> validador) : IServicioInscripciones
{
    /// <summary>
    /// Obtiene las materias inscritas por el estudiante y sus créditos.
    /// </summary>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las inscripciones vigentes, o un error si la cuenta no tiene perfil de estudiante.</returns>
    public async Task<Resultado<InscripcionesPropiasDto>> ObtenerPropiasAsync(CancellationToken cancelacion)
    {
        Estudiante? estudiante = await repositorioEstudiantes.ObtenerPorUsuarioIdAsync(usuarioActual.Id, cancelacion);
        return estudiante is null
            ? ErroresInscripcion.CrearSinPerfilEstudiante()
            : estudiante.ConvertirAInscripcionesPropiasDto();
    }

    /// <summary>
    /// Valida el formato y ejecuta la inscripción en una transacción serializable.
    /// </summary>
    /// <param name="datos">Materia a inscribir.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La inscripción creada, o el error de la regla incumplida.</returns>
    public async Task<Resultado<InscripcionDto>> InscribirAsync(InscribirMateriaDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        ValidationResult validacion = await validador.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresInscripcion.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        int materiaId = datos.MateriaId!.Value;
        Resultado<InscripcionDto> resultado = await unidadTrabajo.EjecutarEnTransaccionAsync(
            intento => InscribirEnTransaccionAsync(materiaId, intento),
            IsolationLevel.Serializable,
            cancelacion);

        if (resultado.EsExitoso)
        {
            await registroEventos.RegistrarInformacionAsync(
                $"La cuenta {usuarioActual.Id} inscribió la materia {materiaId}.", cancelacion);
        }

        return resultado;
    }

    /// <summary>
    /// Cancela la inscripción de una materia.
    /// </summary>
    /// <param name="materiaId">Materia a cancelar.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Éxito, o un error si el estudiante no tiene inscrita la materia.</returns>
    public async Task<Resultado> CancelarAsync(int materiaId, CancellationToken cancelacion)
    {
        Estudiante? estudiante = await repositorioEstudiantes.ObtenerPorUsuarioIdAsync(usuarioActual.Id, cancelacion);
        if (estudiante is null)
        {
            return ErroresInscripcion.CrearSinPerfilEstudiante();
        }

        if (!estudiante.CancelarInscripcion(materiaId, reloj.AhoraUtc))
        {
            return ErroresInscripcion.CrearMateriaNoInscrita(materiaId);
        }

        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        await registroEventos.RegistrarInformacionAsync(
            $"La cuenta {usuarioActual.Id} canceló la inscripción de la materia {materiaId}.", cancelacion);
        return Resultado.CrearExito();
    }

    /// <summary>
    /// Obtiene los nombres de los compañeros de una clase del estudiante.
    /// </summary>
    /// <param name="materiaId">Materia inscrita.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Los compañeros, o un error si el estudiante no tiene inscrita la materia.</returns>
    public async Task<Resultado<CompanerosClaseDto>> ObtenerCompanerosAsync(int materiaId, CancellationToken cancelacion)
    {
        Estudiante? estudiante = await repositorioEstudiantes.ObtenerPorUsuarioIdAsync(usuarioActual.Id, cancelacion);
        if (estudiante is null)
        {
            return ErroresInscripcion.CrearSinPerfilEstudiante();
        }

        Inscripcion? inscripcion = estudiante.InscripcionesActivas.FirstOrDefault(actual => actual.MateriaId == materiaId);
        if (inscripcion is null)
        {
            return ErroresInscripcion.CrearCompanerosNoPermitidos();
        }

        IReadOnlyList<string> companeros = await repositorioEstudiantes.ObtenerNombresInscritosAsync(materiaId, estudiante.Id, cancelacion);
        return new CompanerosClaseDto(materiaId, inscripcion.Materia.Nombre, companeros);
    }

    /// <summary>
    /// Lee el estudiante y la materia, aplica la política y guarda la inscripción dentro de la transacción.
    /// </summary>
    /// <param name="materiaId">Materia a inscribir.</param>
    /// <param name="cancelacion">Cancelación del intento.</param>
    /// <returns>La inscripción creada, o el error de la regla incumplida.</returns>
    private async Task<Resultado<InscripcionDto>> InscribirEnTransaccionAsync(int materiaId, CancellationToken cancelacion)
    {
        Estudiante? estudiante = await repositorioEstudiantes.ObtenerPorUsuarioIdAsync(usuarioActual.Id, cancelacion);
        if (estudiante is null)
        {
            return ErroresInscripcion.CrearSinPerfilEstudiante();
        }

        Materia? materia = await repositorioMaterias.ObtenerPorIdAsync(materiaId, cancelacion);
        if (materia is null)
        {
            return ErroresInscripcion.CrearMateriaNoEncontrada(materiaId);
        }

        MotivoRechazoInscripcion? motivo = PoliticaInscripcion.ObtenerMotivoRechazo(estudiante, materia);
        if (motivo is not null)
        {
            return ErroresInscripcion.CrearRechazo(motivo.Value);
        }

        Inscripcion inscripcion = estudiante.Inscribir(materia, reloj.AhoraUtc);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return inscripcion.ConvertirAInscripcionDto();
    }
}
