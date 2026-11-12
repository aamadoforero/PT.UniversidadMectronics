using Aplicacion.Seguridad.Dtos.Usuarios;
using Aplicacion.Seguridad.Errores;
using Aplicacion.Seguridad.Interfaces;
using Aplicacion.Seguridad.Mapeos;
using Aplicacion.Seguridad.Validadores;
using Dominio.Seguridad.Autorizacion;
using Dominio.Seguridad.Entidades;
using Dominio.Seguridad.ObjetosValor;
using FluentValidation;
using FluentValidation.Results;
using Universidad.Mectronics.Transversal.Auditoria;
using Universidad.Mectronics.Transversal.Resultados;

namespace Aplicacion.Seguridad.Servicios.Usuarios;

/// <summary>Implementación de la creación y edición de cuentas.</summary>
/// <param name="autorizador">Reglas de alcance sobre las cuentas y los roles.</param>
/// <param name="repositorioUsuarios">Acceso a las cuentas.</param>
/// <param name="repositorioRoles">Acceso a los roles.</param>
/// <param name="verificadorDatosAcademicos">Verifica los datos académicos según el tipo de perfil.</param>
/// <param name="unidadTrabajo">Guarda la cuenta y su perfil en una sola transacción.</param>
/// <param name="servicioHash">Calcula el hash de la contraseña temporal.</param>
/// <param name="generadorContrasena">Genera la contraseña temporal.</param>
/// <param name="reloj">Hora actual en UTC.</param>
/// <param name="registroEventos">Auditoría de las altas y ediciones.</param>
/// <param name="validadorCreacion">Reglas de formato para crear una cuenta.</param>
/// <param name="validadorActualizacion">Reglas de formato para editar una cuenta.</param>
public sealed class ServicioRegistroUsuarios(
    AutorizadorUsuarios autorizador,
    IRepositorioUsuarios repositorioUsuarios,
    IRepositorioRoles repositorioRoles,
    VerificadorDatosAcademicos verificadorDatosAcademicos,
    IUnidadTrabajo unidadTrabajo,
    IServicioHashContrasena servicioHash,
    IGeneradorContrasenaTemporal generadorContrasena,
    IReloj reloj,
    IRegistroEventos registroEventos,
    IValidator<CrearUsuarioDto> validadorCreacion,
    IValidator<ActualizarUsuarioDto> validadorActualizacion) : IServicioRegistroUsuarios
{
    /// <summary>
    /// Crea una cuenta con su perfil académico y una contraseña temporal aleatoria.
    /// </summary>
    /// <param name="datos">Datos de la cuenta y, según el rol, sus datos académicos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La cuenta creada con su contraseña temporal, o un error.</returns>
    public async Task<Resultado<UsuarioCreadoDto>> CrearAsync(CrearUsuarioDto datos, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);
        if (!autorizador.TienePermiso(CodigosPermisoUsuarios.Crear))
        {
            return ErroresUsuario.CrearSinPermiso();
        }

        ValidationResult validacion = await validadorCreacion.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresUsuario.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Rol? rol = await repositorioRoles.ObtenerPorIdAsync(datos.RolId!.Value, cancelacion);
        if (rol is null)
        {
            return ErroresUsuario.CrearRolNoEncontrado();
        }

        IReadOnlyDictionary<string, string[]> erroresAcademicos = await verificadorDatosAcademicos.VerificarAsync(
            rol.TipoPerfil, datos.ProgramaId, datos.UbicacionSemestral, datos.CategoriaId, cancelacion);
        if (erroresAcademicos.Count > 0)
        {
            return ErroresUsuario.CrearDatosInvalidos(erroresAcademicos);
        }

        NombreCompleto nombre = NombreCompleto.Crear(datos.NombreCompleto);
        Identificacion identificacion = Identificacion.Crear(datos.Identificacion);
        CorreoElectronico email = CorreoElectronico.Crear(datos.Email);
        Error? conflicto = await BuscarConflictoAsync(email, identificacion, null, cancelacion);
        if (conflicto is not null)
        {
            return conflicto;
        }

        string contrasenaTemporal = generadorContrasena.Generar();
        string hash = servicioHash.CalcularHash(contrasenaTemporal);
        DateTimeOffset ahora = reloj.AhoraUtc;
        Usuario usuario = rol.TipoPerfil switch
        {
            TipoPerfil.Estudiante => Usuario.CrearEstudiante(
                nombre, identificacion, email, hash, rol, datos.ProgramaId!.Value, datos.UbicacionSemestral!.Value, ahora),
            TipoPerfil.Profesor => Usuario.CrearProfesor(
                nombre, identificacion, email, hash, rol, datos.CategoriaId!.Value, ahora),
            _ => Usuario.CrearSinPerfilAcademico(nombre, identificacion, email, hash, rol, ahora)
        };

        repositorioUsuarios.Agregar(usuario);
        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        await registroEventos.RegistrarInformacionAsync(
            $"La cuenta {autorizador.IdActor} creó la cuenta {usuario.Id} con el rol {rol.Nombre}.", cancelacion);

        Usuario creado = await repositorioUsuarios.ObtenerPorIdAsync(usuario.Id, cancelacion) ?? usuario;
        return new UsuarioCreadoDto(creado.ConvertirAUsuarioDetalleDto(ahora), contrasenaTemporal);
    }

    /// <summary>
    /// Edita los datos de una cuenta y su perfil académico.
    /// </summary>
    /// <param name="id">Identificador de la cuenta.</param>
    /// <param name="datos">Datos nuevos.</param>
    /// <param name="cancelacion">Permite cancelar la operación si la petición se abandona.</param>
    /// <returns>La cuenta actualizada, o un error.</returns>
    public async Task<Resultado<UsuarioDetalleDto>> ActualizarAsync(
        int id,
        ActualizarUsuarioDto datos,
        CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);
        if (!autorizador.TienePermiso(CodigosPermisoUsuarios.Editar))
        {
            return ErroresUsuario.CrearSinPermiso();
        }

        ValidationResult validacion = await validadorActualizacion.ValidateAsync(datos, cancelacion);
        if (!validacion.IsValid)
        {
            return ErroresUsuario.CrearDatosInvalidos(validacion.ObtenerDetalleError());
        }

        Usuario? usuario = await repositorioUsuarios.ObtenerPorIdAsync(id, cancelacion);
        if (usuario is null)
        {
            return ErroresUsuario.CrearNoEncontrado(id);
        }

        IReadOnlyDictionary<string, string[]> erroresAcademicos = await verificadorDatosAcademicos.VerificarAsync(
            usuario.Rol.TipoPerfil, datos.ProgramaId, datos.UbicacionSemestral, datos.CategoriaId, cancelacion);
        if (erroresAcademicos.Count > 0)
        {
            return ErroresUsuario.CrearDatosInvalidos(erroresAcademicos);
        }

        Identificacion identificacion = Identificacion.Crear(datos.Identificacion);
        CorreoElectronico email = CorreoElectronico.Crear(datos.Email);
        Error? conflicto = await BuscarConflictoAsync(email, identificacion, usuario.Id, cancelacion);
        if (conflicto is not null)
        {
            return conflicto;
        }

        DateTimeOffset ahora = reloj.AhoraUtc;
        usuario.ActualizarDatosCuenta(NombreCompleto.Crear(datos.NombreCompleto), identificacion, email, ahora);
        if (usuario.Rol.TipoPerfil == TipoPerfil.Estudiante)
        {
            usuario.ActualizarPerfilEstudiante(datos.ProgramaId!.Value, datos.UbicacionSemestral!.Value, ahora);
        }
        else if (usuario.Rol.TipoPerfil == TipoPerfil.Profesor)
        {
            usuario.ActualizarPerfilProfesor(datos.CategoriaId!.Value, ahora);
        }

        await unidadTrabajo.GuardarCambiosAsync(cancelacion);
        await registroEventos.RegistrarInformacionAsync(
            $"La cuenta {autorizador.IdActor} editó los datos de la cuenta {usuario.Id}.", cancelacion);

        Usuario editado = await repositorioUsuarios.ObtenerPorIdAsync(usuario.Id, cancelacion) ?? usuario;
        return editado.ConvertirAUsuarioDetalleDto(ahora);
    }

    /// <summary>
    /// Busca si el correo o la identificación ya los usa otra cuenta.
    /// </summary>
    /// <param name="email">Correo normalizado.</param>
    /// <param name="identificacion">Identificación normalizada.</param>
    /// <param name="excluirUsuarioId">Cuenta que se está editando; <see langword="null"/> al crear.</param>
    /// <param name="cancelacion">Permite cancelar las consultas si la petición se abandona.</param>
    /// <returns>El error de conflicto, o <see langword="null"/> si ambos datos están libres.</returns>
    private async Task<Error?> BuscarConflictoAsync(
        CorreoElectronico email,
        Identificacion identificacion,
        int? excluirUsuarioId,
        CancellationToken cancelacion)
    {
        if (await repositorioUsuarios.ExisteEmailAsync(email, excluirUsuarioId, cancelacion))
        {
            return ErroresUsuario.CrearEmailDuplicado();
        }

        if (await repositorioUsuarios.ExisteIdentificacionAsync(identificacion, excluirUsuarioId, cancelacion))
        {
            return ErroresUsuario.CrearIdentificacionDuplicada();
        }

        return null;
    }
}
