using Dominio.Seguridad.Excepciones;
using Dominio.Seguridad.ObjetosValor;
using Dominio.Seguridad.Politicas;

namespace Dominio.Seguridad.Entidades;

/// <summary>
/// Cuenta de acceso al sistema (tabla <c>Seguridad.Usuario</c>) y raíz del agregado de Seguridad. Garantiza que el perfil
/// académico corresponda al tipo de perfil del rol, el bloqueo por intentos fallidos y el ciclo de vida de la contraseña.
/// Sus datos de identidad son objetos de valor que ya llegan validados. No lee el reloj: todas las operaciones reciben
/// la fecha actual en UTC.
/// </summary>
public sealed class Usuario
{
    /// <summary>Constructor requerido por Entity Framework para materializar el registro.</summary>
    private Usuario()
    {
        NombreCompleto = null!;
        Identificacion = null!;
        Email = null!;
        Contrasena = string.Empty;
        Rol = null!;
    }

    /// <summary>
    /// Inicializa los datos comunes a toda cuenta nueva: activa, con contraseña temporal y sin intentos fallidos.
    /// Lo usan las fábricas, que antes validan que el rol corresponda.
    /// </summary>
    /// <param name="nombreCompleto">Nombre completo.</param>
    /// <param name="identificacion">Documento de identidad.</param>
    /// <param name="email">Correo de inicio de sesión.</param>
    /// <param name="hashContrasenaTemporal">Hash de la contraseña temporal.</param>
    /// <param name="rol">Rol asignado.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    private Usuario(
        NombreCompleto nombreCompleto,
        Identificacion identificacion,
        CorreoElectronico email,
        string hashContrasenaTemporal,
        Rol rol,
        DateTimeOffset ahoraUtc)
    {
        NombreCompleto = nombreCompleto ?? throw new ArgumentNullException(nameof(nombreCompleto));
        Identificacion = identificacion ?? throw new ArgumentNullException(nameof(identificacion));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Contrasena = ValidarHash(hashContrasenaTemporal);
        Rol = rol;
        RolId = rol.Id;
        Activo = true;
        DebeCambiarContrasena = true;
        IntentosInicioFallidos = 0;
        FechaCreacion = ahoraUtc;
    }

    /// <summary>Identificador generado por la base de datos (PK).</summary>
    public int Id { get; private set; }

    /// <summary>Nombre completo. Es el único dato visible para los compañeros de clase.</summary>
    public NombreCompleto NombreCompleto { get; private set; }

    /// <summary>Documento de identidad. Es único en el sistema (lo verifica la capa de Aplicación).</summary>
    public Identificacion Identificacion { get; private set; }

    /// <summary>Correo con el que se inicia sesión. Es único en el sistema (lo verifica la capa de Aplicación).</summary>
    public CorreoElectronico Email { get; private set; }

    /// <summary>Hash de la contraseña. El dominio nunca conoce la contraseña en texto plano.</summary>
    public string Contrasena { get; private set; }

    /// <summary>Identificador del rol asignado (columna <c>RoleId</c>).</summary>
    public byte RolId { get; private set; }

    /// <summary>Rol asignado, con sus permisos y su tipo de perfil.</summary>
    public Rol Rol { get; private set; }

    /// <summary>Indica si la cuenta está habilitada. Una cuenta inactiva no puede iniciar sesión.</summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Indica que la contraseña es temporal (asignada al crear la cuenta o al restablecerla) y que el usuario
    /// debe cambiarla antes de usar el sistema.
    /// </summary>
    public bool DebeCambiarContrasena { get; private set; }

    /// <summary>Intentos fallidos desde el último inicio exitoso o desde que venció el último bloqueo.</summary>
    public int IntentosInicioFallidos { get; private set; }

    /// <summary>Fecha y hora (UTC) hasta la que la cuenta rechaza inicios de sesión; <see langword="null"/> si no está bloqueada.</summary>
    public DateTimeOffset? BloqueoHasta { get; private set; }

    /// <summary>Fecha y hora (UTC) de creación de la cuenta.</summary>
    public DateTimeOffset FechaCreacion { get; private set; }

    /// <summary>Fecha y hora (UTC) del último cambio de datos. Los intentos de inicio de sesión no la modifican.</summary>
    public DateTimeOffset? FechaModificacion { get; private set; }

    /// <summary>Datos académicos de estudiante; solo existe si el rol tiene <see cref="TipoPerfil.Estudiante"/>.</summary>
    public PerfilEstudiante? PerfilEstudiante { get; private set; }

    /// <summary>Datos académicos de profesor; solo existe si el rol tiene <see cref="TipoPerfil.Profesor"/>.</summary>
    public PerfilProfesor? PerfilProfesor { get; private set; }

    /// <summary>
    /// Crea una cuenta cuyo rol no lleva datos académicos (<see cref="TipoPerfil.Ninguno"/>), por ejemplo, Administrador.
    /// </summary>
    /// <param name="nombreCompleto">Nombre completo.</param>
    /// <param name="identificacion">Documento de identidad.</param>
    /// <param name="email">Correo con el que iniciará sesión.</param>
    /// <param name="hashContrasenaTemporal">Hash de la contraseña temporal generada para la cuenta.</param>
    /// <param name="rol">Rol con <see cref="TipoPerfil.Ninguno"/>.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    /// <returns>La cuenta nueva, activa y con contraseña temporal.</returns>
    public static Usuario CrearSinPerfilAcademico(
        NombreCompleto nombreCompleto,
        Identificacion identificacion,
        CorreoElectronico email,
        string hashContrasenaTemporal,
        Rol rol,
        DateTimeOffset ahoraUtc)
    {
        RequerirTipoPerfil(rol, TipoPerfil.Ninguno);
        return new Usuario(nombreCompleto, identificacion, email, hashContrasenaTemporal, rol, ahoraUtc);
    }

    /// <summary>
    /// Crea una cuenta cuyo rol tiene <see cref="TipoPerfil.Estudiante"/>, junto con su perfil de estudiante.
    /// </summary>
    /// <param name="nombreCompleto">Nombre completo.</param>
    /// <param name="identificacion">Documento de identidad.</param>
    /// <param name="email">Correo con el que iniciará sesión.</param>
    /// <param name="hashContrasenaTemporal">Hash de la contraseña temporal generada para la cuenta.</param>
    /// <param name="rol">Rol con <see cref="TipoPerfil.Estudiante"/> (por ejemplo, Estudiante o Monitor).</param>
    /// <param name="programaId">Programa académico del estudiante.</param>
    /// <param name="ubicacionSemestral">Semestre que cursa, entre 1 y 10.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    /// <returns>La cuenta nueva con su perfil de estudiante.</returns>
    public static Usuario CrearEstudiante(
        NombreCompleto nombreCompleto,
        Identificacion identificacion,
        CorreoElectronico email,
        string hashContrasenaTemporal,
        Rol rol,
        int programaId,
        int ubicacionSemestral,
        DateTimeOffset ahoraUtc)
    {
        RequerirTipoPerfil(rol, TipoPerfil.Estudiante);
        return new Usuario(nombreCompleto, identificacion, email, hashContrasenaTemporal, rol, ahoraUtc)
        {
            PerfilEstudiante = new PerfilEstudiante(programaId, ubicacionSemestral)
        };
    }

    /// <summary>
    /// Crea una cuenta cuyo rol tiene <see cref="TipoPerfil.Profesor"/>, junto con su perfil de profesor.
    /// </summary>
    /// <param name="nombreCompleto">Nombre completo.</param>
    /// <param name="identificacion">Documento de identidad.</param>
    /// <param name="email">Correo con el que iniciará sesión.</param>
    /// <param name="hashContrasenaTemporal">Hash de la contraseña temporal generada para la cuenta.</param>
    /// <param name="rol">Rol con <see cref="TipoPerfil.Profesor"/>.</param>
    /// <param name="categoriaId">Categoría de vinculación del profesor.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    /// <returns>La cuenta nueva con su perfil de profesor.</returns>
    public static Usuario CrearProfesor(
        NombreCompleto nombreCompleto,
        Identificacion identificacion,
        CorreoElectronico email,
        string hashContrasenaTemporal,
        Rol rol,
        byte categoriaId,
        DateTimeOffset ahoraUtc)
    {
        RequerirTipoPerfil(rol, TipoPerfil.Profesor);
        return new Usuario(nombreCompleto, identificacion, email, hashContrasenaTemporal, rol, ahoraUtc)
        {
            PerfilProfesor = new PerfilProfesor(categoriaId)
        };
    }

    /// <summary>
    /// Indica si la cuenta está bloqueada por intentos fallidos en el momento indicado.
    /// </summary>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    /// <returns><see langword="true"/> si el bloqueo sigue vigente.</returns>
    public bool EstaBloqueado(DateTimeOffset ahoraUtc) =>
        BloqueoHasta.HasValue && BloqueoHasta.Value > ahoraUtc;

    /// <summary>
    /// Indica si la cuenta puede iniciar sesión: debe estar activa y no bloqueada.
    /// No verifica la contraseña; eso lo hace la capa de Aplicación con el servicio de hash.
    /// </summary>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    /// <returns><see langword="true"/> si la cuenta está activa y sin bloqueo vigente.</returns>
    public bool PuedeIniciarSesion(DateTimeOffset ahoraUtc) => Activo && !EstaBloqueado(ahoraUtc);

    /// <summary>
    /// Registra un intento de inicio de sesión fallido y bloquea la cuenta al alcanzar el máximo de la política.
    /// Mientras la cuenta está bloqueada no cuenta intentos, y cuando el bloqueo vence el conteo vuelve a cero.
    /// </summary>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    /// <param name="politica">Máximo de intentos y duración del bloqueo.</param>
    public void RegistrarInicioFallido(DateTimeOffset ahoraUtc, PoliticaBloqueo politica)
    {
        ArgumentNullException.ThrowIfNull(politica);
        if (EstaBloqueado(ahoraUtc))
        {
            return;
        }

        if (BloqueoHasta.HasValue)
        {
            ReiniciarBloqueo();
        }

        IntentosInicioFallidos++;
        if (IntentosInicioFallidos >= politica.IntentosMaximos)
        {
            BloqueoHasta = ahoraUtc.Add(politica.DuracionBloqueo);
        }
    }

    /// <summary>
    /// Registra un inicio de sesión exitoso: reinicia los intentos fallidos y quita cualquier bloqueo vencido.
    /// </summary>
    public void RegistrarInicioExitoso() => ReiniciarBloqueo();

    /// <summary>
    /// El propio usuario define su contraseña: deja de ser temporal y se reinician los intentos fallidos.
    /// </summary>
    /// <param name="nuevoHash">Hash de la contraseña nueva, que ya cumplió <see cref="PoliticaContrasena"/>.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void CambiarContrasena(string nuevoHash, DateTimeOffset ahoraUtc)
    {
        Contrasena = ValidarHash(nuevoHash);
        DebeCambiarContrasena = false;
        ReiniciarBloqueo();
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Un administrador restablece la contraseña: queda temporal (el usuario deberá cambiarla) y la cuenta se desbloquea.
    /// </summary>
    /// <param name="hashTemporal">Hash de la contraseña temporal generada.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void AsignarContrasenaTemporal(string hashTemporal, DateTimeOffset ahoraUtc)
    {
        Contrasena = ValidarHash(hashTemporal);
        DebeCambiarContrasena = true;
        ReiniciarBloqueo();
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Actualiza los datos que el propio usuario puede modificar desde su perfil (solo el nombre).
    /// </summary>
    /// <param name="nombreCompleto">Nombre completo nuevo.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void ActualizarDatosPersonales(NombreCompleto nombreCompleto, DateTimeOffset ahoraUtc)
    {
        ArgumentNullException.ThrowIfNull(nombreCompleto);
        NombreCompleto = nombreCompleto;
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Actualiza los datos de identificación de la cuenta. Solo lo hace un usuario con permiso para editar cuentas.
    /// </summary>
    /// <param name="nombreCompleto">Nombre completo.</param>
    /// <param name="identificacion">Documento de identidad.</param>
    /// <param name="email">Correo de inicio de sesión.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void ActualizarDatosCuenta(
        NombreCompleto nombreCompleto,
        Identificacion identificacion,
        CorreoElectronico email,
        DateTimeOffset ahoraUtc)
    {
        ArgumentNullException.ThrowIfNull(nombreCompleto);
        ArgumentNullException.ThrowIfNull(identificacion);
        ArgumentNullException.ThrowIfNull(email);
        NombreCompleto = nombreCompleto;
        Identificacion = identificacion;
        Email = email;
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Actualiza el programa y el semestre del perfil de estudiante. Lanza <see cref="ExcepcionDominio"/> si la cuenta
    /// no tiene perfil de estudiante o si los datos no son válidos.
    /// </summary>
    /// <param name="programaId">Programa académico.</param>
    /// <param name="ubicacionSemestral">Semestre, entre 1 y 10.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void ActualizarPerfilEstudiante(int programaId, int ubicacionSemestral, DateTimeOffset ahoraUtc)
    {
        if (PerfilEstudiante is null)
        {
            throw new ExcepcionDominio("La cuenta no tiene perfil de estudiante.");
        }

        PerfilEstudiante.Actualizar(programaId, ubicacionSemestral);
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Actualiza la categoría del perfil de profesor. Lanza <see cref="ExcepcionDominio"/> si la cuenta no tiene
    /// perfil de profesor o si la categoría no es válida.
    /// </summary>
    /// <param name="categoriaId">Categoría de vinculación.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void ActualizarPerfilProfesor(byte categoriaId, DateTimeOffset ahoraUtc)
    {
        if (PerfilProfesor is null)
        {
            throw new ExcepcionDominio("La cuenta no tiene perfil de profesor.");
        }

        PerfilProfesor.Actualizar(categoriaId);
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Cambia el rol de la cuenta. Solo se permite hacia un rol con el mismo <see cref="TipoPerfil"/>, para que
    /// el perfil académico y su historial (inscripciones o materias) se conserven intactos.
    /// Por ejemplo: Estudiante a Monitor sí; Administrador a Coordinador sí; Estudiante a Profesor no.
    /// </summary>
    /// <param name="nuevoRol">Rol que se asignará.</param>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void CambiarRol(Rol nuevoRol, DateTimeOffset ahoraUtc)
    {
        ArgumentNullException.ThrowIfNull(nuevoRol);
        if (nuevoRol.TipoPerfil != Rol.TipoPerfil)
        {
            throw new ExcepcionDominio(
                $"No se puede cambiar de un rol con perfil '{Rol.TipoPerfil}' a uno con perfil '{nuevoRol.TipoPerfil}'.");
        }

        Rol = nuevoRol;
        RolId = nuevoRol.Id;
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Habilita la cuenta y quita cualquier bloqueo por intentos fallidos.
    /// </summary>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void Activar(DateTimeOffset ahoraUtc)
    {
        Activo = true;
        ReiniciarBloqueo();
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Deshabilita la cuenta: desde ese momento no puede iniciar sesión ni renovar la sesión.
    /// </summary>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    public void Desactivar(DateTimeOffset ahoraUtc)
    {
        Activo = false;
        RegistrarModificacion(ahoraUtc);
    }

    /// <summary>
    /// Exige un hash no vacío.
    /// </summary>
    /// <param name="hash">Hash de la contraseña.</param>
    /// <returns>El mismo hash, ya validado.</returns>
    private static string ValidarHash(string? hash) =>
        string.IsNullOrWhiteSpace(hash)
            ? throw new ExcepcionDominio("El hash de la contraseña es obligatorio.")
            : hash;

    /// <summary>
    /// Exige que el rol exista y que su tipo de perfil sea el que espera la fábrica.
    /// </summary>
    /// <param name="rol">Rol recibido por la fábrica.</param>
    /// <param name="esperado">Tipo de perfil que corresponde a la fábrica.</param>
    private static void RequerirTipoPerfil(Rol? rol, TipoPerfil esperado)
    {
        if (rol is null)
        {
            throw new ExcepcionDominio("El rol es obligatorio.");
        }

        if (rol.TipoPerfil != esperado)
        {
            throw new ExcepcionDominio($"El rol '{rol.Nombre}' exige el perfil '{rol.TipoPerfil}', no '{esperado}'.");
        }
    }

    /// <summary>Deja la cuenta sin intentos fallidos ni bloqueo.</summary>
    private void ReiniciarBloqueo()
    {
        IntentosInicioFallidos = 0;
        BloqueoHasta = null;
    }

    /// <summary>
    /// Registra la fecha de la última modificación de la cuenta.
    /// </summary>
    /// <param name="ahoraUtc">Fecha y hora actual en UTC.</param>
    private void RegistrarModificacion(DateTimeOffset ahoraUtc) => FechaModificacion = ahoraUtc;
}
