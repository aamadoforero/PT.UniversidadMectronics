-- 1. Verificar si la base de datos ya existe, y si no, crearla
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UniversidadMectronics')
BEGIN
    CREATE DATABASE UniversidadMectronics;
    PRINT 'Base de datos [UniversidadMectronics] creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos [UniversidadMectronics] ya existe.';
END
GO


USE UniversidadMectronics;
GO

- 2. Creación de esquemas
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Autenticacion')
BEGIN
    EXEC('CREATE SCHEMA Autenticacion;');
    PRINT 'Esquema [Autenticacion] creado.';
END
ELSE
    PRINT 'El esquema [Autenticacion] ya existe.';
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

-- 3. Creación de tablas Negocio
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Autenticacion' AND t.name = 'Rol')
BEGIN
    CREATE TABLE Autenticacion.Rol (
        Id TINYINT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL    
    );
    PRINT 'Tabla [Autenticacion.Rol] creada.';
END
ELSE
    PRINT 'La tabla [Autenticacion.Rol] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'Nivel')
BEGIN
    CREATE TABLE Academico.Nivel ( 
        Id TINYINT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL   
    );
    PRINT 'Tabla [Academico.Nivel] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Nivel] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'Programa')
BEGIN
    CREATE TABLE Academico.Programa (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        NivelId TINYINT NOT NULL FOREIGN KEY REFERENCES Academico.Nivel(Id),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    );
    PRINT 'Tabla [Academico.Programa] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Programa] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'CategoriaProfesor')
BEGIN
    CREATE TABLE Academico.CategoriaProfesor (
        Id TINYINT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    );
    PRINT 'Tabla [Academico.CategoriaProfesor] creada.';
END
ELSE
    PRINT 'La tabla [Academico.CategoriaProfesor] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Autenticacion' AND t.name = 'Usuario')
BEGIN
    CREATE TABLE Autenticacion.Usuario (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NombreCompleto NVARCHAR(150) NOT NULL,
        Identificacion NVARCHAR(50) UNIQUE NOT NULL,
        Email NVARCHAR(150) UNIQUE NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        RoleId TINYINT NOT NULL FOREIGN KEY REFERENCES Autenticacion.Rol(Id),
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    );
    PRINT 'Tabla [Autenticacion.Usuario] creada.';
END
ELSE
    PRINT 'La tabla [Autenticacion.Usuario] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'Estudiante')
BEGIN
    CREATE TABLE Academico.Estudiante (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Autenticacion.Usuario(Id),    
        UbicacionSemestral INT NOT NULL DEFAULT 3 CHECK (UbicacionSemestral BETWEEN 1 AND 10),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    );
    PRINT 'Tabla [Academico.Estudiante] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Estudiante] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'Profesores')
BEGIN
    CREATE TABLE Academico.Profesor (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NULL UNIQUE FOREIGN KEY REFERENCES Autenticacion.Usuario(Id),
        CategoriaId TINYINT NOT NULL FOREIGN KEY REFERENCES Academico.CategoriaProfesor(Id),
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    );
    PRINT 'Tabla [Academico.Profesores] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Profesores] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'ProgramaEstudiante')
BEGIN
    CREATE TABLE Academico.ProgramaEstudiante (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        EstudianteId INT NOT NULL FOREIGN KEY REFERENCES Academico.Estudiante(Id),    
        ProgramaId INT NOT NULL FOREIGN KEY REFERENCES Academico.Programa(Id),    
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL,
        CONSTRAINT UQ_Estudiante_Programa UNIQUE (EstudianteId, ProgramaId)
    );
    PRINT 'Tabla [Academico.ProgramaEstudiante] creada.';
END
ELSE
    PRINT 'La tabla [Academico.ProgramaEstudiante] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'Materia')
BEGIN
    CREATE TABLE Academico.Materia (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Creditos INT NOT NULL DEFAULT 3,    
        Activo BIT NOT NULL DEFAULT 1,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL
    );
    PRINT 'Tabla [Academico.Materia] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Materia] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'MateriaPrograma')
BEGIN
    CREATE TABLE Academico.MateriaPrograma (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MateriaId INT NOT NULL FOREIGN KEY REFERENCES Academico.Materia(Id),    
        ProgramaId INT NOT NULL FOREIGN KEY REFERENCES Academico.Programa(Id),    
        ProfesorId INT NOT NULL FOREIGN KEY REFERENCES Academico.Profesor(Id),
        FechaCreacion DATETIME DEFAULT GETDATE(),
        FechaModificacion DATETIME NULL,
        CONSTRAINT UQ_Materia_Programa UNIQUE (MateriaId, ProgramaId)
    );
    PRINT 'Tabla [Academico.MateriaPrograma] creada.';
END
ELSE
    PRINT 'La tabla [Academico.MateriaPrograma] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Academico' AND t.name = 'Inscripcion')
BEGIN
    CREATE TABLE Academico.Inscripcion (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        EstudianteId INT NOT NULL FOREIGN KEY REFERENCES Academico.Estudiante(Id),
        MateriaProgramaId INT NOT NULL FOREIGN KEY REFERENCES Academico.MateriaPrograma(Id),
        FechaInscripcion DATETIME DEFAULT GETDATE(),
        Activo BIT NOT NULL DEFAULT 1,
        FechaModificacion DATETIME NULL,
        CONSTRAINT UQ_Estudiante_MateriaPrograma UNIQUE (EstudianteId, MateriaProgramaId)
    );
    PRINT 'Tabla [Academico.Inscripcion] creada.';
END
ELSE
    PRINT 'La tabla [Academico.Inscripcion] ya existe.';
GO

-- 4. Creación de tablas Logs
IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Logs' AND t.name = 'TipoEvento')
BEGIN
    CREATE TABLE Logs.TipoEvento (
        Id TINYINT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(20) NOT NULL 
    );
    PRINT 'Tabla [Logs.TipoEvento] creada.';
END
ELSE
    PRINT 'La tabla [Logs.TipoEvento] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Logs' AND t.name = 'Aplicacion')
BEGIN
    CREATE TABLE Logs.Aplicacion (
        Id TINYINT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(50) NOT NULL 
    );
    PRINT 'Tabla [Logs.Aplicacion] creada.';
END
ELSE
    PRINT 'La tabla [Logs.Aplicacion] ya existe.';
GO

IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'Logs' AND t.name = 'RegistroEvento')
BEGIN
    CREATE TABLE Logs.RegistroEvento (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FechaEvento DATETIME NOT NULL DEFAULT GETDATE(),
        TipoEventoId TINYINT NOT NULL FOREIGN KEY REFERENCES Logs.TipoEvento(Id),
        AplicacionId TINYINT NOT NULL FOREIGN KEY REFERENCES Logs.Aplicacion(Id),
        Mensaje NVARCHAR(MAX) NULL,
        Excepcion NVARCHAR(MAX) NULL
    );
    PRINT 'Tabla [Logs.RegistroEvento] creada.';
END
ELSE
    PRINT 'La tabla [Logs.RegistroEvento] ya existe.';
GO