USE UniversidadMectronics;
GO

-- Roles
IF NOT EXISTS (SELECT 1 FROM Autenticacion.Rol WHERE Nombre = 'Estudiante')
BEGIN
    INSERT INTO Autenticacion.Rol (Nombre) VALUES ('Estudiante');
    PRINT 'Rol [Estudiante] insertado exitosamente.';
END
ELSE
    PRINT 'El rol [Estudiante] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Autenticacion.Rol WHERE Nombre = 'Docente')
BEGIN
    INSERT INTO Autenticacion.Rol (Nombre) VALUES ('Docente');
    PRINT 'Rol [Docente] insertado exitosamente.';
END
ELSE
    PRINT 'El rol [Docente] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Autenticacion.Rol WHERE Nombre = 'Administrador')
BEGIN
    INSERT INTO Autenticacion.Rol (Nombre) VALUES ('Administrador');
    PRINT 'Rol [Administrador] insertado exitosamente.';
END
ELSE
    PRINT 'El rol [Administrador] ya existe.';
GO

-- Nivel de estudio
IF NOT EXISTS (SELECT 1 FROM Academico.Nivel WHERE Nombre = 'Técnico')
BEGIN
    INSERT INTO Academico.Nivel (Nombre) VALUES ('Técnico');
    PRINT 'Nivel académico [Técnico] insertado exitosamente.';
END
ELSE
    PRINT 'El nivel académico [Técnico] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Academico.Nivel WHERE Nombre = 'Tecnólogo')
BEGIN
    INSERT INTO Academico.Nivel (Nombre) VALUES ('Tecnólogo');
    PRINT 'Nivel académico [Tecnólogo] insertado exitosamente.';
END
ELSE
    PRINT 'El nivel académico [Tecnólogo] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Academico.Nivel WHERE Nombre = 'Profesional')
BEGIN
    INSERT INTO Academico.Nivel (Nombre) VALUES ('Profesional');
    PRINT 'Nivel académico [Profesional] insertado exitosamente.';
END
ELSE
    PRINT 'El nivel académico [Profesional] ya existe.';
GO

-- Categorias Profesor
IF NOT EXISTS (SELECT 1 FROM Academico.CategoriaProfesor WHERE Nombre = 'Planta')
BEGIN
    INSERT INTO Academico.CategoriaProfesor (Nombre) VALUES ('Planta');
    PRINT 'Categoría de profesor [Planta] insertada exitosamente.';
END
ELSE
    PRINT 'La categoría de profesor [Planta] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Academico.CategoriaProfesor WHERE Nombre = 'Cátedra')
BEGIN
    INSERT INTO Academico.CategoriaProfesor (Nombre) VALUES ('Cátedra');
    PRINT 'Categoría de profesor [Cátedra] insertada exitosamente.';
END
ELSE
    PRINT 'La categoría de profesor [Cátedra] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Academico.CategoriaProfesor WHERE Nombre = 'Ocasional')
BEGIN
    INSERT INTO Academico.CategoriaProfesor (Nombre) VALUES ('Ocasional');
    PRINT 'Categoría de profesor [Ocasional] insertada exitosamente.';
END
ELSE
    PRINT 'La categoría de profesor [Ocasional] ya existe.';
GO

-- Tipos de eventos en logs
IF NOT EXISTS (SELECT 1 FROM Logs.TipoEvento WHERE Nombre = 'Informativo')
BEGIN
    INSERT INTO Logs.TipoEvento (Nombre) VALUES ('Informativo');
    PRINT 'Tipo de evento [Informativo] insertado exitosamente.';
END
ELSE
    PRINT 'El tipo de evento [Informativo] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Logs.TipoEvento WHERE Nombre = 'Advertencia')
BEGIN
    INSERT INTO Logs.TipoEvento (Nombre) VALUES ('Advertencia');
    PRINT 'Tipo de evento [Advertencia] insertado exitosamente.';
END
ELSE
    PRINT 'El tipo de evento [Advertencia] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Logs.TipoEvento WHERE Nombre = 'Error')
BEGIN
    INSERT INTO Logs.TipoEvento (Nombre) VALUES ('Error');
    PRINT 'Tipo de evento [Error] insertado exitosamente.';
END
ELSE
    PRINT 'El tipo de evento [Error] ya existe.';
GO

-- Aplicaciones
IF NOT EXISTS (SELECT 1 FROM Logs.Aplicacion WHERE Nombre = 'Servicio-Autenticacion')
BEGIN
    INSERT INTO Logs.Aplicacion (Nombre) VALUES ('Servicio-Autenticacion');
    PRINT 'Aplicación [Servicio-Autenticacion] insertada exitosamente.';
END
ELSE
    PRINT 'La aplicación [Servicio-Autenticacion] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Logs.Aplicacion WHERE Nombre = 'Servicio-Academico')
BEGIN
    INSERT INTO Logs.Aplicacion (Nombre) VALUES ('Servicio-Academico');
    PRINT 'Aplicación [Servicio-Academico] insertada exitosamente.';
END
ELSE
    PRINT 'La aplicación [Servicio-Academico] ya existe.';
GO

IF NOT EXISTS (SELECT 1 FROM Logs.Aplicacion WHERE Nombre = 'Api-Gateway')
BEGIN
    INSERT INTO Logs.Aplicacion (Nombre) VALUES ('Api-Gateway');
    PRINT 'Aplicación [Api-Gateway] insertada exitosamente.';
END
ELSE
    PRINT 'La aplicación [Api-Gateway] ya existe.';
GO