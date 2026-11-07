USE UniversidadMectronics;
GO

CREATE OR ALTER PROCEDURE sp_DocumentarTabla
    @SchemaName SYSNAME,
    @TableName SYSNAME,
    @ColumnName SYSNAME = NULL,
    @Value NVARCHAR(4000)
AS
BEGIN
    IF @ColumnName IS NULL
    BEGIN
        IF EXISTS (SELECT * FROM sys.extended_properties ep JOIN sys.tables t ON ep.major_id = t.object_id JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = @SchemaName AND t.name = @TableName AND ep.minor_id = 0 AND ep.name = 'MS_Description')
            EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name=@TableName;
        
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=@Value, @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name=@TableName;
    END
    ELSE
    BEGIN
        IF EXISTS (SELECT * FROM sys.extended_properties ep JOIN sys.columns c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id JOIN sys.tables t ON c.object_id = t.object_id JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = @SchemaName AND t.name = @TableName AND c.name = @ColumnName AND ep.name = 'MS_Description')
            EXEC sys.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name=@TableName, @level2type=N'COLUMN', @level2name=@ColumnName;
        
        EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=@Value, @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name=@TableName, @level2type=N'COLUMN', @level2name=@ColumnName;
    END
END
GO

- Autenticación
EXEC sp_DocumentarTabla 'Autenticacion', 'Rol', NULL, 'Catálogo de roles de autorización del sistema.';
EXEC sp_DocumentarTabla 'Autenticacion', 'Rol', 'Id', 'Identificador único del rol (PK).';
EXEC sp_DocumentarTabla 'Autenticacion', 'Rol', 'Nombre', 'Nombre descriptivo del rol.';

EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', NULL, 'Tabla central de credenciales de acceso unificadas.';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'Id', 'Identificador único del usuario (PK).';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'NombreCompleto', 'Nombre completo legal.';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'Identificacion', 'Número de documento de identidad único.';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'Email', 'Correo electrónico institucional único para login.';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'PasswordHash', 'Contraseña cifrada mediante hash seguro.';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'RoleId', 'Relación con el rol asignado (FK).';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'Activo', 'Estado de la cuenta (1 = Activo, 0 = Inactivo).';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Autenticacion', 'Usuario', 'FechaModificacion', 'Fecha de última modificación.';

-- Académico
EXEC sp_DocumentarTabla 'Academico', 'Nivel', NULL, 'Catálogo de niveles académicos (Técnico, Tecnólogo, Profesional).';
EXEC sp_DocumentarTabla 'Academico', 'Nivel', 'Id', 'Identificador único del nivel (PK).';
EXEC sp_DocumentarTabla 'Academico', 'Nivel', 'Nombre', 'Nombre del nivel educativo.';

EXEC sp_DocumentarTabla 'Academico', 'Programa', NULL, 'Programas académicos ofrecidos por la universidad.';
EXEC sp_DocumentarTabla 'Academico', 'Programa', 'Id', 'Identificador único del programa (PK).';
EXEC sp_DocumentarTabla 'Academico', 'Programa', 'Nombre', 'Nombre del programa.';
EXEC sp_DocumentarTabla 'Academico', 'Programa', 'Activo', 'Estado del programa.';
EXEC sp_DocumentarTabla 'Academico', 'Programa', 'NivelId', 'Relación con el nivel académico (FK).';
EXEC sp_DocumentarTabla 'Academico', 'Programa', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Academico', 'Programa', 'FechaModificacion', 'Fecha de última modificación.';

EXEC sp_DocumentarTabla 'Academico', 'CategoriaProfesor', NULL, 'Catálogo de categorías para los docentes.';
EXEC sp_DocumentarTabla 'Academico', 'CategoriaProfesor', 'Id', 'Identificador único de categoría (PK).';
EXEC sp_DocumentarTabla 'Academico', 'CategoriaProfesor', 'Nombre', 'Nombre de la categoría.';
EXEC sp_DocumentarTabla 'Academico', 'CategoriaProfesor', 'Activo', 'Estado de la categoría.';
EXEC sp_DocumentarTabla 'Academico', 'CategoriaProfesor', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Academico', 'CategoriaProfesor', 'FechaModificacion', 'Fecha de última modificación.';

EXEC sp_DocumentarTabla 'Academico', 'Estudiante', NULL, 'Perfil extendido para los alumnos de la institución.';
EXEC sp_DocumentarTabla 'Academico', 'Estudiante', 'Id', 'Identificador único del estudiante (PK).';
EXEC sp_DocumentarTabla 'Academico', 'Estudiante', 'UsuarioId', 'Relación 1 a 1 con la tabla Usuario (FK).';
EXEC sp_DocumentarTabla 'Academico', 'Estudiante', 'UbicacionSemestral', 'Semestre actual cursado (Rango estricto del 1 al 10).';
EXEC sp_DocumentarTabla 'Academico', 'Estudiante', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Academico', 'Estudiante', 'FechaModificacion', 'Fecha de última modificación.';

EXEC sp_DocumentarTabla 'Academico', 'Profesor', NULL, 'Personal docente de la facultad.';
EXEC sp_DocumentarTabla 'Academico', 'Profesor', 'Id', 'Identificador único del profesor (PK).';
EXEC sp_DocumentarTabla 'Academico', 'Profesor', 'UsuarioId', 'Relación opcional 1 a 1 con Usuario (FK).';
EXEC sp_DocumentarTabla 'Academico', 'Profesor', 'CategoriaId', 'Relación con la categoría del profesor (FK).';
EXEC sp_DocumentarTabla 'Academico', 'Profesor', 'Activo', 'Estado operativo del profesor.';
EXEC sp_DocumentarTabla 'Academico', 'Profesor', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Academico', 'Profesor', 'FechaModificacion', 'Fecha de última modificación.';

EXEC sp_DocumentarTabla 'Academico', 'ProgramaEstudiante', NULL, 'Relación de matrícula de estudiantes por programa.';
EXEC sp_DocumentarTabla 'Academico', 'ProgramaEstudiante', 'Id', 'Identificador único (PK).';
EXEC sp_DocumentarTabla 'Academico', 'ProgramaEstudiante', 'EstudianteId', 'Estudiante matriculado (FK).';
EXEC sp_DocumentarTabla 'Academico', 'ProgramaEstudiante', 'ProgramaId', 'Programa académico (FK).';
EXEC sp_DocumentarTabla 'Academico', 'ProgramaEstudiante', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Academico', 'ProgramaEstudiante', 'FechaModificacion', 'Fecha de última modificación.';

EXEC sp_DocumentarTabla 'Academico', 'Materia', NULL, 'Catálogo general de asignaturas.';
EXEC sp_DocumentarTabla 'Academico', 'Materia', 'Id', 'Identificador único de la materia (PK).';
EXEC sp_DocumentarTabla 'Academico', 'Materia', 'Nombre', 'Nombre de la asignatura.';
EXEC sp_DocumentarTabla 'Academico', 'Materia', 'Creditos', 'Valor académico (por defecto 3 créditos).';
EXEC sp_DocumentarTabla 'Academico', 'Materia', 'Activo', 'Estado de la materia.';
EXEC sp_DocumentarTabla 'Academico', 'Materia', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Academico', 'Materia', 'FechaModificacion', 'Fecha de última modificación.';

EXEC sp_DocumentarTabla 'Academico', 'MateriaPrograma', NULL, 'Oferta académica que vincula materia, programa y profesor.';
EXEC sp_DocumentarTabla 'Academico', 'MateriaPrograma', 'Id', 'Identificador único (PK).';
EXEC sp_DocumentarTabla 'Academico', 'MateriaPrograma', 'MateriaId', 'Asignatura ofertada (FK).';
EXEC sp_DocumentarTabla 'Academico', 'MateriaPrograma', 'ProgramaId', 'Programa al que pertenece (FK).';
EXEC sp_DocumentarTabla 'Academico', 'MateriaPrograma', 'ProfesorId', 'Docente titular asignado (FK).';
EXEC sp_DocumentarTabla 'Academico', 'MateriaPrograma', 'FechaCreacion', 'Fecha de creación del registro.';
EXEC sp_DocumentarTabla 'Academico', 'MateriaPrograma', 'FechaModificacion', 'Fecha de última modificación.';

EXEC sp_DocumentarTabla 'Academico', 'Inscripcion', NULL, 'Matrícula de los estudiantes en las materias ofertadas.';
EXEC sp_DocumentarTabla 'Academico', 'Inscripcion', 'Id', 'Identificador único de la inscripción (PK).';
EXEC sp_DocumentarTabla 'Academico', 'Inscripcion', 'EstudianteId', 'Alumno que se inscribe (FK).';
EXEC sp_DocumentarTabla 'Academico', 'Inscripcion', 'MateriaProgramaId', 'Oferta de materia seleccionada (FK).';
EXEC sp_DocumentarTabla 'Academico', 'Inscripcion', 'FechaInscripcion', 'Fecha y hora exacta de la matrícula.';
EXEC sp_DocumentarTabla 'Academico', 'Inscripcion', 'Activo', 'Estado de la inscripción (Permite borrado lógico).';
EXEC sp_DocumentarTabla 'Academico', 'Inscripcion', 'FechaModificacion', 'Fecha de última modificación.';

-- Logs
EXEC sp_DocumentarTabla 'Logs', 'TipoEvento', NULL, 'Catálogo de severidad de Logs (Informativo, Error, Warning).';
EXEC sp_DocumentarTabla 'Logs', 'TipoEvento', 'Id', 'Identificador único (PK).';
EXEC sp_DocumentarTabla 'Logs', 'TipoEvento', 'Nombre', 'Nombre de la severidad.';

EXEC sp_DocumentarTabla 'Logs', 'Aplicacion', NULL, 'Catálogo de microservicios o componentes emisores.';
EXEC sp_DocumentarTabla 'Logs', 'Aplicacion', 'Id', 'Identificador único (PK).';
EXEC sp_DocumentarTabla 'Logs', 'Aplicacion', 'Nombre', 'Nombre del componente.';

EXEC sp_DocumentarTabla 'Logs', 'RegistroEvento', NULL, 'Almacén centralizado de trazas técnicas y de negocio.';
EXEC sp_DocumentarTabla 'Logs', 'RegistroEvento', 'Id', 'Identificador único (PK).';
EXEC sp_DocumentarTabla 'Logs', 'RegistroEvento', 'FechaEvento', 'Timestamp del suceso.';
EXEC sp_DocumentarTabla 'Logs', 'RegistroEvento', 'TipoEventoId', 'Nivel de severidad (FK).';
EXEC sp_DocumentarTabla 'Logs', 'RegistroEvento', 'AplicacionId', 'Componente emisor (FK).';
EXEC sp_DocumentarTabla 'Logs', 'RegistroEvento', 'Mensaje', 'Descripción del evento.';
EXEC sp_DocumentarTabla 'Logs', 'RegistroEvento', 'Excepcion', 'Detalle técnico de la excepción (Stack Trace).';
GO