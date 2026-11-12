-- 1. Base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UniversidadMectronics')
BEGIN
    CREATE DATABASE UniversidadMectronics;
    PRINT 'Base de datos [UniversidadMectronics] creada exitosamente.';
END
ELSE
    PRINT 'La base de datos [UniversidadMectronics] ya existe.';
GO

USE UniversidadMectronics;
GO

-- Requeridos por los índices filtrados.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 2. Esquemas
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Seguridad')
BEGIN
    EXEC('CREATE SCHEMA Seguridad;');
    PRINT 'Esquema [Seguridad] creado.';
END
ELSE
    PRINT 'El esquema [Seguridad] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Academico')
BEGIN
    EXEC('CREATE SCHEMA Academico;');
    PRINT 'Esquema [Academico] creado.';
END
ELSE
    PRINT 'El esquema [Academico] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Logs')
BEGIN
    EXEC('CREATE SCHEMA Logs;');
    PRINT 'Esquema [Logs] creado.';
END
ELSE
    PRINT 'El esquema [Logs] ya existe.';
GO

-- 3. Esquema Seguridad
IF OBJECT_ID('Seguridad.Rol', 'U') IS NULL
BEGIN
    CREATE TABLE Seguridad.Rol (
        Id         TINYINT IDENTITY(1,1) NOT NULL,
        Nombre     NVARCHAR(50)          NOT NULL,
        -- Datos académicos que exige el rol: 0 = Ninguno, 1 = Estudiante, 2 = Profesor.
        TipoPerfil TINYINT               NOT NULL CONSTRAINT DF_Rol_TipoPerfil DEFAULT (0),
        CONSTRAINT PK_Rol PRIMARY KEY (Id),
        CONSTRAINT UQ_Rol_Nombre UNIQUE (Nombre),
        CONSTRAINT CK_Rol_TipoPerfil CHECK (TipoPerfil IN (0, 1, 2))
    );
    PRINT 'Tabla [Seguridad.Rol] creada.';
END
ELSE
    PRINT 'La tabla [Seguridad.Rol] ya existe.';
GO

IF OBJECT_ID('Seguridad.Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE Seguridad.Usuario (
        Id                     INT IDENTITY(1,1) NOT NULL,
        NombreCompleto         NVARCHAR(150)     NOT NULL,
        Identificacion         NVARCHAR(50)      NOT NULL,
        Email                  NVARCHAR(150)     NOT NULL,
        Contrasena             NVARCHAR(MAX)     NOT NULL,
        RoleId                 TINYINT           NOT NULL,
        Activo                 BIT               NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
        DebeCambiarContrasena  BIT               NOT NULL CONSTRAINT DF_Usuario_DebeCambiarContrasena DEFAULT (1),
        IntentosInicioFallidos INT               NOT NULL CONSTRAINT DF_Usuario_IntentosInicioFallidos DEFAULT (0),
        BloqueoHasta           DATETIMEOFFSET    NULL,
        FechaCreacion          DATETIMEOFFSET    NOT NULL CONSTRAINT DF_Usuario_FechaCreacion DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00')),
        FechaModificacion      DATETIMEOFFSET    NULL,
        CONSTRAINT PK_Usuario PRIMARY KEY (Id),
        CONSTRAINT UQ_Usuario_Identificacion UNIQUE (Identificacion),
        CONSTRAINT UQ_Usuario_Email UNIQUE (Email),
        CONSTRAINT FK_Usuario_Rol FOREIGN KEY (RoleId) REFERENCES Seguridad.Rol (Id),
        CONSTRAINT CK_Usuario_IntentosInicioFallidos CHECK (IntentosInicioFallidos >= 0)
    );
    PRINT 'Tabla [Seguridad.Usuario] creada.';
END
ELSE
    PRINT 'La tabla [Seguridad.Usuario] ya existe.';
GO

-- Los códigos de permiso están definidos en el código de cada microservicio;
-- la tabla los registra para poder asignarlos a los roles.
IF OBJECT_ID('Seguridad.Permiso', 'U') IS NULL
BEGIN
    CREATE TABLE Seguridad.Permiso (
        Id          SMALLINT IDENTITY(1,1) NOT NULL,
        Codigo      NVARCHAR(100)          NOT NULL,
        Descripcion NVARCHAR(250)          NOT NULL,
        CONSTRAINT PK_Permiso PRIMARY KEY (Id),
        CONSTRAINT UQ_Permiso_Codigo UNIQUE (Codigo)
    );
    PRINT 'Tabla [Seguridad.Permiso] creada.';
END
ELSE
    PRINT 'La tabla [Seguridad.Permiso] ya existe.';
GO

IF OBJECT_ID('Seguridad.RolPermiso', 'U') IS NULL
BEGIN
    CREATE TABLE Seguridad.RolPermiso (
        RolId     TINYINT  NOT NULL,
        PermisoId SMALLINT NOT NULL,
        CONSTRAINT PK_RolPermiso PRIMARY KEY (RolId, PermisoId),
        CONSTRAINT FK_RolPermiso_Rol FOREIGN KEY (RolId) REFERENCES Seguridad.Rol (Id),
        CONSTRAINT FK_RolPermiso_Permiso FOREIGN KEY (PermisoId) REFERENCES Seguridad.Permiso (Id)
    );
    PRINT 'Tabla [Seguridad.RolPermiso] creada.';
END
ELSE
    PRINT 'La tabla [Seguridad.RolPermiso] ya existe.';
GO

-- 4. Esquema Academico: catálogos
IF OBJECT_ID('Academico.Nivel', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.Nivel (
        Id     TINYINT IDENTITY(1,1) NOT NULL,
        Nombre NVARCHAR(150)         NOT NULL,
        CONSTRAINT PK_Nivel PRIMARY KEY (Id),
        CONSTRAINT UQ_Nivel_Nombre UNIQUE (Nombre)
    );
    PRINT 'Tabla [Academico.Nivel] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Nivel] ya existe.';
GO

IF OBJECT_ID('Academico.Programa', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.Programa (
        Id                INT IDENTITY(1,1) NOT NULL,
        Nombre            NVARCHAR(150)     NOT NULL,
        NivelId           TINYINT           NOT NULL,
        Activo            BIT               NOT NULL CONSTRAINT DF_Programa_Activo DEFAULT (1),
        FechaCreacion     DATETIMEOFFSET    NOT NULL CONSTRAINT DF_Programa_FechaCreacion DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00')),
        FechaModificacion DATETIMEOFFSET    NULL,
        CONSTRAINT PK_Programa PRIMARY KEY (Id),
        CONSTRAINT UQ_Programa_Nombre UNIQUE (Nombre),
        CONSTRAINT FK_Programa_Nivel FOREIGN KEY (NivelId) REFERENCES Academico.Nivel (Id)
    );
    PRINT 'Tabla [Academico.Programa] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Programa] ya existe.';
GO

IF OBJECT_ID('Academico.CategoriaProfesor', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.CategoriaProfesor (
        Id     TINYINT IDENTITY(1,1) NOT NULL,
        Nombre NVARCHAR(150)         NOT NULL,
        CONSTRAINT PK_CategoriaProfesor PRIMARY KEY (Id),
        CONSTRAINT UQ_CategoriaProfesor_Nombre UNIQUE (Nombre)
    );
    PRINT 'Tabla [Academico.CategoriaProfesor] creada.';
END
ELSE
    PRINT 'La tabla [Academico.CategoriaProfesor] ya existe.';
GO

-- 5. Esquema Academico: perfiles por rol
IF OBJECT_ID('Academico.Estudiante', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.Estudiante (
        Id                 INT IDENTITY(1,1) NOT NULL,
        UsuarioId          INT               NOT NULL,
        ProgramaId         INT               NOT NULL,
        UbicacionSemestral INT               NOT NULL CONSTRAINT DF_Estudiante_UbicacionSemestral DEFAULT (1),
        CONSTRAINT PK_Estudiante PRIMARY KEY (Id),
        CONSTRAINT UQ_Estudiante_UsuarioId UNIQUE (UsuarioId),
        CONSTRAINT FK_Estudiante_Usuario FOREIGN KEY (UsuarioId) REFERENCES Seguridad.Usuario (Id),
        CONSTRAINT FK_Estudiante_Programa FOREIGN KEY (ProgramaId) REFERENCES Academico.Programa (Id),
        CONSTRAINT CK_Estudiante_UbicacionSemestral CHECK (UbicacionSemestral BETWEEN 1 AND 10)
    );
    PRINT 'Tabla [Academico.Estudiante] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Estudiante] ya existe.';
GO

IF OBJECT_ID('Academico.Profesor', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.Profesor (
        Id          INT IDENTITY(1,1) NOT NULL,
        UsuarioId   INT               NOT NULL,
        CategoriaId TINYINT           NOT NULL,
        CONSTRAINT PK_Profesor PRIMARY KEY (Id),
        CONSTRAINT UQ_Profesor_UsuarioId UNIQUE (UsuarioId),
        CONSTRAINT FK_Profesor_Usuario FOREIGN KEY (UsuarioId) REFERENCES Seguridad.Usuario (Id),
        CONSTRAINT FK_Profesor_CategoriaProfesor FOREIGN KEY (CategoriaId) REFERENCES Academico.CategoriaProfesor (Id)
    );
    PRINT 'Tabla [Academico.Profesor] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Profesor] ya existe.';
GO

-- 6. Esquema Academico: materias e inscripciones
IF OBJECT_ID('Academico.Materia', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.Materia (
        Id                INT IDENTITY(1,1) NOT NULL,
        Nombre            NVARCHAR(100)     NOT NULL,
        Creditos          INT               NOT NULL CONSTRAINT DF_Materia_Creditos DEFAULT (3),
        ProgramaId        INT               NOT NULL,
        ProfesorId        INT               NOT NULL,
        Activo            BIT               NOT NULL CONSTRAINT DF_Materia_Activo DEFAULT (1),
        FechaCreacion     DATETIMEOFFSET    NOT NULL CONSTRAINT DF_Materia_FechaCreacion DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00')),
        FechaModificacion DATETIMEOFFSET    NULL,
        CONSTRAINT PK_Materia PRIMARY KEY (Id),
        CONSTRAINT UQ_Materia_Programa_Nombre UNIQUE (ProgramaId, Nombre),
        CONSTRAINT FK_Materia_Programa FOREIGN KEY (ProgramaId) REFERENCES Academico.Programa (Id),
        CONSTRAINT FK_Materia_Profesor FOREIGN KEY (ProfesorId) REFERENCES Academico.Profesor (Id),
        CONSTRAINT CK_Materia_Creditos CHECK (Creditos = 3)
    );
    PRINT 'Tabla [Academico.Materia] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Materia] ya existe.';
GO

IF OBJECT_ID('Academico.Inscripcion', 'U') IS NULL
BEGIN
    CREATE TABLE Academico.Inscripcion (
        Id                INT IDENTITY(1,1) NOT NULL,
        EstudianteId      INT               NOT NULL,
        MateriaId         INT               NOT NULL,
        Activo            BIT               NOT NULL CONSTRAINT DF_Inscripcion_Activo DEFAULT (1),
        FechaInscripcion  DATETIMEOFFSET    NOT NULL CONSTRAINT DF_Inscripcion_FechaInscripcion DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00')),
        FechaModificacion DATETIMEOFFSET    NULL,
        CONSTRAINT PK_Inscripcion PRIMARY KEY (Id),
        CONSTRAINT FK_Inscripcion_Estudiante FOREIGN KEY (EstudianteId) REFERENCES Academico.Estudiante (Id),
        CONSTRAINT FK_Inscripcion_Materia FOREIGN KEY (MateriaId) REFERENCES Academico.Materia (Id)
    );
    PRINT 'Tabla [Academico.Inscripcion] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Inscripcion] ya existe.';
GO

-- 7. Esquema Logs
IF OBJECT_ID('Logs.TipoEvento', 'U') IS NULL
BEGIN
    CREATE TABLE Logs.TipoEvento (
        Id     TINYINT IDENTITY(1,1) NOT NULL,
        Nombre NVARCHAR(20)          NOT NULL,
        CONSTRAINT PK_TipoEvento PRIMARY KEY (Id),
        CONSTRAINT UQ_TipoEvento_Nombre UNIQUE (Nombre)
    );
    PRINT 'Tabla [Logs.TipoEvento] creada.';
END
ELSE
    PRINT 'La tabla [Logs.TipoEvento] ya existe.';
GO

IF OBJECT_ID('Logs.Aplicacion', 'U') IS NULL
BEGIN
    CREATE TABLE Logs.Aplicacion (
        Id     TINYINT IDENTITY(1,1) NOT NULL,
        Nombre NVARCHAR(50)          NOT NULL,
        CONSTRAINT PK_Aplicacion PRIMARY KEY (Id),
        CONSTRAINT UQ_Aplicacion_Nombre UNIQUE (Nombre)
    );
    PRINT 'Tabla [Logs.Aplicacion] creada.';
END
ELSE
    PRINT 'La tabla [Logs.Aplicacion] ya existe.';
GO

IF OBJECT_ID('Logs.RegistroEvento', 'U') IS NULL
BEGIN
    CREATE TABLE Logs.RegistroEvento (
        Id           INT IDENTITY(1,1) NOT NULL,
        FechaEvento  DATETIMEOFFSET    NOT NULL CONSTRAINT DF_RegistroEvento_FechaEvento DEFAULT (TODATETIMEOFFSET(SYSUTCDATETIME(), '+00:00')),
        TipoEventoId TINYINT           NOT NULL,
        AplicacionId TINYINT           NOT NULL,
        Mensaje      NVARCHAR(MAX)     NULL,
        Excepcion    NVARCHAR(MAX)     NULL,
        CONSTRAINT PK_RegistroEvento PRIMARY KEY (Id),
        CONSTRAINT FK_RegistroEvento_TipoEvento FOREIGN KEY (TipoEventoId) REFERENCES Logs.TipoEvento (Id),
        CONSTRAINT FK_RegistroEvento_Aplicacion FOREIGN KEY (AplicacionId) REFERENCES Logs.Aplicacion (Id)
    );
    PRINT 'Tabla [Logs.RegistroEvento] creada.';
END
ELSE
    PRINT 'La tabla [Logs.RegistroEvento] ya existe.';
GO

-- 8. Índices
-- Una inscripción activa por estudiante y materia; las canceladas (Activo = 0) quedan como historial.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Inscripcion_Estudiante_Materia_Activa' AND object_id = OBJECT_ID('Academico.Inscripcion'))
    CREATE UNIQUE NONCLUSTERED INDEX UX_Inscripcion_Estudiante_Materia_Activa
        ON Academico.Inscripcion (EstudianteId, MateriaId)
        WHERE Activo = 1;
GO

-- Compañeros de clase: inscripciones activas de una materia.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Inscripcion_Materia_Activa' AND object_id = OBJECT_ID('Academico.Inscripcion'))
    CREATE NONCLUSTERED INDEX IX_Inscripcion_Materia_Activa
        ON Academico.Inscripcion (MateriaId)
        INCLUDE (EstudianteId)
        WHERE Activo = 1;
GO

-- Validaciones de inscripción del estudiante (máximo de materias y profesor repetido).
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Inscripcion_Estudiante_Activa' AND object_id = OBJECT_ID('Academico.Inscripcion'))
    CREATE NONCLUSTERED INDEX IX_Inscripcion_Estudiante_Activa
        ON Academico.Inscripcion (EstudianteId)
        INCLUDE (MateriaId)
        WHERE Activo = 1;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Materia_ProfesorId' AND object_id = OBJECT_ID('Academico.Materia'))
    CREATE NONCLUSTERED INDEX IX_Materia_ProfesorId ON Academico.Materia (ProfesorId);
GO

-- Listado de cuentas paginado y ordenado por nombre (ORDER BY NombreCompleto, Id): entrega las filas ya ordenadas.
-- El Id no se declara porque todo índice no agrupado ya incluye la clave agrupada.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Usuario_NombreCompleto' AND object_id = OBJECT_ID('Seguridad.Usuario'))
    CREATE NONCLUSTERED INDEX IX_Usuario_NombreCompleto ON Seguridad.Usuario (NombreCompleto);
GO

-- Listado filtrado por rol y ordenado por nombre; también atiende la clave foránea FK_Usuario_Rol.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Usuario_RoleId_NombreCompleto' AND object_id = OBJECT_ID('Seguridad.Usuario'))
    CREATE NONCLUSTERED INDEX IX_Usuario_RoleId_NombreCompleto ON Seguridad.Usuario (RoleId, NombreCompleto);
GO

-- La PK (RolId, PermisoId) cubre "permisos de un rol"; este índice cubre "roles que tienen un permiso".
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RolPermiso_PermisoId' AND object_id = OBJECT_ID('Seguridad.RolPermiso'))
    CREATE NONCLUSTERED INDEX IX_RolPermiso_PermisoId ON Seguridad.RolPermiso (PermisoId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Estudiante_ProgramaId' AND object_id = OBJECT_ID('Academico.Estudiante'))
    CREATE NONCLUSTERED INDEX IX_Estudiante_ProgramaId ON Academico.Estudiante (ProgramaId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Profesor_CategoriaId' AND object_id = OBJECT_ID('Academico.Profesor'))
    CREATE NONCLUSTERED INDEX IX_Profesor_CategoriaId ON Academico.Profesor (CategoriaId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Programa_NivelId' AND object_id = OBJECT_ID('Academico.Programa'))
    CREATE NONCLUSTERED INDEX IX_Programa_NivelId ON Academico.Programa (NivelId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RegistroEvento_FechaEvento' AND object_id = OBJECT_ID('Logs.RegistroEvento'))
    CREATE NONCLUSTERED INDEX IX_RegistroEvento_FechaEvento
        ON Logs.RegistroEvento (FechaEvento DESC)
        INCLUDE (TipoEventoId, AplicacionId);
GO

PRINT 'DDL finalizado.';
GO
