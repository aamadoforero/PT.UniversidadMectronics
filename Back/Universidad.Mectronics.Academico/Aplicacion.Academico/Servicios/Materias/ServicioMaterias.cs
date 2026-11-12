using System.Data;
using Aplicacion.Academico.Dtos.Materias;
using Aplicacion.Academico.Errores;
using Aplicacion.Academico.Interfaces;
using Aplicacion.Academico.Mapeos;
using Aplicacion.Academico.Validadores;
using Dominio.Academico.Autorizacion;
using Dominio.Academico.Entidades;
using Dominio.Academico.Politicas;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Academico.Servicios.Materias;

/// <summary>
/// Implementación de la consulta y administración de materias. Las operaciones que dependen de conteos corren en una
/// transacción serializable.
/// </summary>
/// <param name="usuarioActual">Cuenta autenticada.</param>
/// <param name="repositorioMaterias">Acceso a las materias.</param>
/// <param name="repositorioProfesores">Acceso a los profesores.</param>
/// <param name="repositorioProgramas">Acceso a los programas.</param>
/// <param name="unidadTrabajo">Transacciones y confirmación de cambios.</param>
/// <param name="reloj">Fecha actual.</param>
/// <param name="registroEventos">Auditoría.</param>
/// <param name="validadorCrear">Reglas de formato de la creación.</param>
/// <param name="validadorActualizar">Reglas de formato de la edición.</param>
/// <param name="validadorEstado">Reglas de formato del cambio de estado.</param>
public sealed class ServicioMaterias(
    IUsuarioActual usuarioActual,
    IRepositorioMaterias repositorioMaterias,
    IRepositorioProfesores repositorioProfesores,
    IRepositorioProgramas repositorioProgramas,
    IUnidadTrabajo unidadTrabajo,
    IReloj reloj,
    IRegistroEventos registroEventos,
    IValidator<CrearMateriaDto> validadorCrear,
    IValidator<ActualizarMateriaDto> validadorActualizar,
    IValidator<CambiarEstadoMateriaDto> validadorEstado) : IServicioMaterias
{
    /// <summary>
    /// Obtiene las materias, ordenadas por nombre.
    /// </summary>
    /// <param name="incluirInactivas">Si se incluyen las desactivadas.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>Las materias, o un error de falta de permiso.</returns>
    public async Task<Resultado<IReadOnlyList<MateriaDto>>> ObtenerTodasAsync(bool incluirInactivas, CancellationToken cancelacion)
    {
        if (incluirInactivas && !PuedeGestionar())
        {
            return ErroresMateria.CrearSinPermisoInactivas();
        }

        IReadOnlyList<Materia> materias = await repositorioMaterias.ObtenerTodasAsync(incluirInactivas, cancelacion);
        IReadOnlyList<MateriaDto> dtos = materias.Select(materia => materia.ConvertirAMateriaDto()).ToList();
        return Resultado.CrearExito(dtos);
    }

    /// <summary>
    /// Obtiene una materia.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia, o un error si no existe o está desactivada y quien consulta no la administra.</returns>
    public async Task<Resultado<MateriaDto>> ObtenerPorIdAsync(int id, CancellationToken cancelacion)
    {
        Materia? materia = await repositorioMaterias.ObtenerPorIdAsync(id, cancelacion);
        return materia is null || (!materia.Activo && !PuedeGestionar())
            ? ErroresMateria.CrearNoEncontrada(id)
            : materia.ConvertirAMateriaDto();
    }

    /// <summary>
    /// Valida el formato y crea la materia en una transacción serializable.
    /// </summary>
    /// <param name="datos">Datos de la materia.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia creada, o el error de la regla incumplida.</returns>
    public async Task<Resultado<MateriaDto>> CrearAsync(CrearMateriaDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        ValidationResult validacion = await validadorCrear.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresMateria.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Resultado<MateriaDto> resultado = await unidadTrabajo.EjecutarEnTransaccionAsync(
            intento => CrearEnTransaccionAsync(datos, intento), IsolationLevel.Serializable, cancelacion);

        if (resultado.EsExitoso)
        {
            await registroEventos.RegistrarInformacionAsync(
                $"La cuenta {usuarioActual.Id} creó la materia {resultado.Valor.Id} ({resultado.Valor.Nombre}).", cancelacion);
        }

        return resultado;
    }

    /// <summary>
    /// Valida el formato y edita la materia en una transacción serializable.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia editada, o el error de la regla incumplida.</returns>
    public async Task<Resultado<MateriaDto>> ActualizarAsync(int id, ActualizarMateriaDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        ValidationResult validacion = await validadorActualizar.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresMateria.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Resultado<MateriaDto> resultado = await unidadTrabajo.EjecutarEnTransaccionAsync(
            intento => ActualizarEnTransaccionAsync(id, datos, intento), IsolationLevel.Serializable, cancelacion);

        if (resultado.EsExitoso)
        {
            await registroEventos.RegistrarInformacionAsync($"La cuenta {usuarioActual.Id} editó la materia {id}.", cancelacion);
        }

        return resultado;
    }

    /// <summary>
    /// Valida el formato y cambia el estado de la materia en una transacción serializable.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="datos">Estado nuevo.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La materia con su estado nuevo, o el error de la regla incumplida.</returns>
    public async Task<Resultado<MateriaDto>> CambiarEstadoAsync(int id, CambiarEstadoMateriaDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);

        ValidationResult validacion = await validadorEstado.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresMateria.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        bool activo = datos.Activo!.Value;
        Resultado<MateriaDto> resultado = await unidadTrabajo.EjecutarEnTransaccionAsync(
            intento => CambiarEstadoEnTransaccionAsync(id, activo, intento), IsolationLevel.Serializable, cancelacion);

        if (resultado.EsExitoso)
        {
            await registroEventos.RegistrarInformacionAsync(
                $"La cuenta {usuarioActual.Id} {(activo ? "activó" : "desactivó")} la materia {id}.", cancelacion);
        }

        return resultado;
    }

    /// <summary>
    /// Verifica programa, profesor, nombre y cupo del profesor, y crea la materia.
    /// </summary>
    /// <param name="datos">Datos ya validados.</param>
    /// <param name="cancelacion">Cancelación del intento.</param>
    /// <returns>La materia creada, o el error de la regla incumplida.</returns>
    private async Task<Resultado<MateriaDto>> CrearEnTransaccionAsync(CrearMateriaDto datos, CancellationToken cancelacion)
    {
        int programaId = datos.ProgramaId!.Value;
        int profesorId = datos.ProfesorId!.Value;
        string nombre = datos.Nombre!.Trim();

        if (!await repositorioProgramas.ExisteActivoAsync(programaId, cancelacion))
        {
            return ErroresMateria.CrearProgramaNoValido();
        }

        Profesor? profesor = await repositorioProfesores.ObtenerActivoPorIdAsync(profesorId, cancelacion);
        if (profesor is null)
        {
            return ErroresMateria.CrearProfesorNoValido();
        }

        if (await repositorioMaterias.ExisteNombreAsync(programaId, nombre, null, cancelacion))
        {
            return ErroresMateria.CrearNombreDuplicado(nombre);
        }

        if (await repositorioMaterias.ContarActivasPorProfesorAsync(profesorId, null, cancelacion) >= PoliticaMateria.MaximoMateriasPorProfesor)
        {
            return ErroresMateria.CrearProfesorSinCupo();
        }

        Materia materia = Materia.Crear(nombre, programaId, profesorId, reloj.AhoraUtc);
        repositorioMaterias.Agregar(materia);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return materia.ConvertirAMateriaDto(profesor);
    }

    /// <summary>
    /// Verifica la materia, el profesor, el impacto del cambio de profesor en los inscritos y el nombre, y aplica los cambios.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="datos">Datos ya validados.</param>
    /// <param name="cancelacion">Cancelación del intento.</param>
    /// <returns>La materia editada, o el error de la regla incumplida.</returns>
    private async Task<Resultado<MateriaDto>> ActualizarEnTransaccionAsync(int id, ActualizarMateriaDto datos, CancellationToken cancelacion)
    {
        Materia? materia = await repositorioMaterias.ObtenerPorIdAsync(id, cancelacion);
        if (materia is null)
        {
            return ErroresMateria.CrearNoEncontrada(id);
        }

        int profesorId = datos.ProfesorId!.Value;
        string nombre = datos.Nombre!.Trim();

        Profesor? profesor = await repositorioProfesores.ObtenerActivoPorIdAsync(profesorId, cancelacion);
        if (profesor is null)
        {
            return ErroresMateria.CrearProfesorNoValido();
        }

        bool cambiaProfesor = profesorId != materia.ProfesorId;
        if (cambiaProfesor && await repositorioMaterias.ContarInscritosAsync(id, cancelacion) > 0)
        {
            return ErroresMateria.CrearCambioProfesorConInscritos();
        }

        if (cambiaProfesor && materia.Activo &&
            await repositorioMaterias.ContarActivasPorProfesorAsync(profesorId, id, cancelacion) >= PoliticaMateria.MaximoMateriasPorProfesor)
        {
            return ErroresMateria.CrearProfesorSinCupo();
        }

        if (await repositorioMaterias.ExisteNombreAsync(materia.ProgramaId, nombre, id, cancelacion))
        {
            return ErroresMateria.CrearNombreDuplicado(nombre);
        }

        materia.Actualizar(nombre, profesorId, reloj.AhoraUtc);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return materia.ConvertirAMateriaDto(profesor);
    }

    /// <summary>
    /// Verifica que desactivar no deje inscritos huérfanos y que activar no supere el cupo del profesor, y cambia el estado.
    /// </summary>
    /// <param name="id">Identificador de la materia.</param>
    /// <param name="activo">Estado nuevo.</param>
    /// <param name="cancelacion">Cancelación del intento.</param>
    /// <returns>La materia con su estado nuevo, o el error de la regla incumplida.</returns>
    private async Task<Resultado<MateriaDto>> CambiarEstadoEnTransaccionAsync(int id, bool activo, CancellationToken cancelacion)
    {
        Materia? materia = await repositorioMaterias.ObtenerPorIdAsync(id, cancelacion);
        if (materia is null)
        {
            return ErroresMateria.CrearNoEncontrada(id);
        }

        if (materia.Activo == activo)
        {
            return materia.ConvertirAMateriaDto();
        }

        if (!activo)
        {
            int inscritos = await repositorioMaterias.ContarInscritosAsync(id, cancelacion);
            if (inscritos > 0)
            {
                return ErroresMateria.CrearConInscritos(inscritos);
            }
        }
        else if (await repositorioMaterias.ContarActivasPorProfesorAsync(materia.ProfesorId, id, cancelacion) >= PoliticaMateria.MaximoMateriasPorProfesor)
        {
            return ErroresMateria.CrearProfesorSinCupo();
        }

        materia.CambiarEstado(activo, reloj.AhoraUtc);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        return materia.ConvertirAMateriaDto();
    }

    /// <summary>
    /// Indica si la cuenta autenticada administra materias.
    /// </summary>
    /// <returns><see langword="true"/> si tiene <c>materias.gestionar</c>.</returns>
    private bool PuedeGestionar() => usuarioActual.EstaAutenticado && usuarioActual.TienePermiso(CodigosPermisoMaterias.Gestionar);
}
