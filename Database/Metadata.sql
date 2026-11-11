/* =====================================================================
   Universidad Mectronics - Metadata
   Documenta tablas y columnas con la propiedad extendida MS_Description.
   Ejecutar después de DDL.sql. Es idempotente.
   ===================================================================== */

USE UniversidadMectronics;
GO

CREATE OR ALTER PROCEDURE dbo.sp_DocumentarTabla
    @SchemaName SYSNAME,
    @TableName  SYSNAME,
    @ColumnName SYSNAME = NULL,
    @Value      NVARCHAR(4000)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ColumnName IS NULL
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM sys.extended_properties ep
            JOIN sys.tables t  ON ep.major_id = t.object_id
            JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @SchemaName AND t.name = @TableName AND ep.minor_id = 0 AND ep.name = N'MS_Description')
            EXEC sys.sp_dropextendedproperty @name = N'MS_Description',
                @level0type = N'SCHEMA', @level0name = @SchemaName,
                @level1type = N'TABLE',  @level1name = @TableName;

        EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @Value,
            @level0type = N'SCHEMA', @level0name = @SchemaName,
            @level1type = N'TABLE',  @level1name = @TableName;
    END
    ELSE
    BEGIN
        IF EXISTS (
            SELECT 1
            FROM sys.extended_properties ep
            JOIN sys.columns c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
            JOIN sys.tables t  ON c.object_id = t.object_id
            JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = @SchemaName AND t.name = @TableName AND c.name = @ColumnName AND ep.name = N'MS_Description')
            EXEC sys.sp_dropextendedproperty @name = N'MS_Description',
                @level0type = N'SCHEMA', @level0name = @SchemaName,
                @level1type = N'TABLE',  @level1name = @TableName,
                @level2type = N'COLUMN', @level2name = @ColumnName;

        EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = @Value,
            @level0type = N'SCHEMA', @level0name = @SchemaName,
            @level1type = N'TABLE',  @level1name = @TableName,
            @level2type = N'COLUMN', @level2name = @ColumnName;
    END
END
GO

-- Seguridad
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Rol', NULL, N'Roles de autorización. Cada usuario tiene un único rol. Lo que puede hacer lo definen sus permisos (Seguridad.RolPermiso) y los datos académicos que exige, su TipoPerfil.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Rol', 'Id', N'Identificador único del rol (PK).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Rol', 'Nombre', N'Nombre único del rol. El código no depende de él.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Rol', 'TipoPerfil', N'Datos académicos que exige el rol: 0 = Ninguno, 1 = Estudiante (Academico.Estudiante), 2 = Profesor (Academico.Profesor). No debe cambiarse si el rol ya tiene usuarios.';

EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Permiso', NULL, N'Catálogo de permisos. Cada permiso habilita una funcionalidad y su código está definido en el código del microservicio que la implementa.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Permiso', 'Id', N'Identificador único del permiso (PK).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Permiso', 'Codigo', N'Código único del permiso (por ejemplo, usuarios.crear). Viaja en el token de acceso.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Permiso', 'Descripcion', N'Descripción de la funcionalidad que habilita el permiso.';

EXEC dbo.sp_DocumentarTabla 'Seguridad', 'RolPermiso', NULL, N'Asignación de permisos a roles (relación muchos a muchos). Configurable sin cambiar código.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'RolPermiso', 'RolId', N'Rol que recibe el permiso (FK a Seguridad.Rol, parte de la PK).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'RolPermiso', 'PermisoId', N'Permiso asignado (FK a Seguridad.Permiso, parte de la PK).';

EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', NULL, N'Cuentas con acceso al sistema. Solo las crean usuarios con el permiso usuarios.crear.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'Id', N'Identificador único del usuario (PK).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'NombreCompleto', N'Nombre completo legal. Es el único dato visible para los compañeros de clase.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'Identificacion', N'Número de documento de identidad único.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'Email', N'Correo electrónico institucional único para iniciar sesión.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'Contrasena', N'Hash de la contraseña (PasswordHasher de ASP.NET Core, PBKDF2-HMAC-SHA512).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'RoleId', N'Rol asignado (FK a Seguridad.Rol).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'Activo', N'Estado de la cuenta (1 = Activa, 0 = Inactiva).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'DebeCambiarContrasena', N'Indica que la contraseña es temporal y debe cambiarse antes de usar el sistema.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'IntentosInicioFallidos', N'Intentos fallidos desde el último inicio válido o desde que venció el bloqueo.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'BloqueoHasta', N'Fecha y hora (UTC) hasta la que se rechazan nuevos inicios de sesión.';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'FechaCreacion', N'Fecha de creación del registro (UTC).';
EXEC dbo.sp_DocumentarTabla 'Seguridad', 'Usuario', 'FechaModificacion', N'Fecha de la última modificación (UTC).';

-- Academico: catálogos
EXEC dbo.sp_DocumentarTabla 'Academico', 'Nivel', NULL, N'Catálogo de niveles académicos (Técnico, Tecnólogo, Profesional).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Nivel', 'Id', N'Identificador único del nivel (PK).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Nivel', 'Nombre', N'Nombre único del nivel académico.';

EXEC dbo.sp_DocumentarTabla 'Academico', 'Programa', NULL, N'Programas académicos de créditos ofrecidos por la universidad.';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Programa', 'Id', N'Identificador único del programa (PK).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Programa', 'Nombre', N'Nombre único del programa.';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Programa', 'NivelId', N'Nivel académico del programa (FK a Academico.Nivel).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Programa', 'Activo', N'Estado del programa (1 = Activo, 0 = Inactivo).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Programa', 'FechaCreacion', N'Fecha de creación del registro (UTC).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Programa', 'FechaModificacion', N'Fecha de la última modificación (UTC).';

EXEC dbo.sp_DocumentarTabla 'Academico', 'CategoriaProfesor', NULL, N'Catálogo de categorías de vinculación docente (Planta, Cátedra, Ocasional).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'CategoriaProfesor', 'Id', N'Identificador único de la categoría (PK).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'CategoriaProfesor', 'Nombre', N'Nombre único de la categoría.';

-- Academico: perfiles por rol
EXEC dbo.sp_DocumentarTabla 'Academico', 'Estudiante', NULL, N'Perfil académico de los usuarios con rol Estudiante. Un estudiante pertenece a un único programa.';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Estudiante', 'Id', N'Identificador único del estudiante (PK).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Estudiante', 'UsuarioId', N'Cuenta asociada (FK única a Seguridad.Usuario, relación 1 a 1).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Estudiante', 'ProgramaId', N'Programa en el que está matriculado (FK a Academico.Programa).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Estudiante', 'UbicacionSemestral', N'Semestre que cursa actualmente (entre 1 y 10).';

EXEC dbo.sp_DocumentarTabla 'Academico', 'Profesor', NULL, N'Perfil académico de los usuarios con rol Profesor.';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Profesor', 'Id', N'Identificador único del profesor (PK).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Profesor', 'UsuarioId', N'Cuenta asociada (FK única y obligatoria a Seguridad.Usuario, relación 1 a 1).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Profesor', 'CategoriaId', N'Categoría de vinculación (FK a Academico.CategoriaProfesor).';

-- Academico: materias e inscripciones
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', NULL, N'Asignaturas de un programa. Cada materia la dicta un profesor y cada profesor dicta como máximo 2 materias (regla validada en la aplicación).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'Id', N'Identificador único de la materia (PK).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'Nombre', N'Nombre de la asignatura, único dentro del programa.';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'Creditos', N'Créditos académicos de la materia (por defecto 3).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'ProgramaId', N'Programa al que pertenece la materia (FK a Academico.Programa).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'ProfesorId', N'Profesor que dicta la materia (FK a Academico.Profesor).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'Activo', N'Estado de la materia (1 = Disponible para inscripción, 0 = Inactiva).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'FechaCreacion', N'Fecha de creación del registro (UTC).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Materia', 'FechaModificacion', N'Fecha de la última modificación (UTC).';

EXEC dbo.sp_DocumentarTabla 'Academico', 'Inscripcion', NULL, N'Inscripción de un estudiante en una materia. Máximo 3 activas por estudiante y sin repetir profesor (reglas validadas en la aplicación).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Inscripcion', 'Id', N'Identificador único de la inscripción (PK).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Inscripcion', 'EstudianteId', N'Estudiante que se inscribe (FK a Academico.Estudiante).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Inscripcion', 'MateriaId', N'Materia inscrita (FK a Academico.Materia).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Inscripcion', 'Activo', N'Estado de la inscripción (1 = Vigente, 0 = Cancelada; borrado lógico).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Inscripcion', 'FechaInscripcion', N'Fecha y hora de la inscripción (UTC).';
EXEC dbo.sp_DocumentarTabla 'Academico', 'Inscripcion', 'FechaModificacion', N'Fecha de la última modificación, por ejemplo la cancelación (UTC).';

-- Logs
EXEC dbo.sp_DocumentarTabla 'Logs', 'TipoEvento', NULL, N'Catálogo de severidad de los eventos (Informativo, Advertencia, Error).';
EXEC dbo.sp_DocumentarTabla 'Logs', 'TipoEvento', 'Id', N'Identificador único (PK).';
EXEC dbo.sp_DocumentarTabla 'Logs', 'TipoEvento', 'Nombre', N'Nombre único de la severidad.';

EXEC dbo.sp_DocumentarTabla 'Logs', 'Aplicacion', NULL, N'Catálogo de microservicios o componentes que emiten eventos.';
EXEC dbo.sp_DocumentarTabla 'Logs', 'Aplicacion', 'Id', N'Identificador único (PK).';
EXEC dbo.sp_DocumentarTabla 'Logs', 'Aplicacion', 'Nombre', N'Nombre único del componente.';

EXEC dbo.sp_DocumentarTabla 'Logs', 'RegistroEvento', NULL, N'Almacén centralizado de eventos técnicos y de negocio.';
EXEC dbo.sp_DocumentarTabla 'Logs', 'RegistroEvento', 'Id', N'Identificador único (PK).';
EXEC dbo.sp_DocumentarTabla 'Logs', 'RegistroEvento', 'FechaEvento', N'Fecha y hora del evento (UTC).';
EXEC dbo.sp_DocumentarTabla 'Logs', 'RegistroEvento', 'TipoEventoId', N'Severidad del evento (FK a Logs.TipoEvento).';
EXEC dbo.sp_DocumentarTabla 'Logs', 'RegistroEvento', 'AplicacionId', N'Componente emisor (FK a Logs.Aplicacion).';
EXEC dbo.sp_DocumentarTabla 'Logs', 'RegistroEvento', 'Mensaje', N'Descripción del evento.';
EXEC dbo.sp_DocumentarTabla 'Logs', 'RegistroEvento', 'Excepcion', N'Detalle técnico de la excepción (stack trace).';
GO

PRINT 'Metadata finalizada.';
GO
