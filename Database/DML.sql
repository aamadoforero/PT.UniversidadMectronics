USE UniversidadMectronics;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
-- Requeridos para escribir en tablas con índices filtrados (Academico.Inscripcion).
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Roles. TipoPerfil: 0 = Ninguno, 1 = Estudiante, 2 = Profesor.
INSERT INTO Seguridad.Rol (Nombre, TipoPerfil)
SELECT origen.Nombre, origen.TipoPerfil
FROM (VALUES
    (N'Administrador', 0),
    (N'Profesor',      2),
    (N'Estudiante',    1)
) AS origen (Nombre, TipoPerfil)
WHERE NOT EXISTS (SELECT 1 FROM Seguridad.Rol r WHERE r.Nombre = origen.Nombre);
PRINT 'Roles verificados.';
GO

-- 1.1 Permisos. Los códigos deben coincidir con las constantes de cada microservicio.
INSERT INTO Seguridad.Permiso (Codigo, Descripcion)
SELECT origen.Codigo, origen.Descripcion
FROM (VALUES
    (N'usuarios.consultar',              N'Listar y consultar cuentas de usuario.'),
    (N'usuarios.crear',                  N'Crear cuentas de usuario.'),
    (N'usuarios.editar',                 N'Editar los datos y el perfil académico de una cuenta.'),
    (N'usuarios.cambiar-estado',         N'Activar o desactivar cuentas.'),
    (N'usuarios.restablecer-contrasena', N'Generar una contraseña temporal para una cuenta.'),
    (N'materias.gestionar',              N'Crear, editar y desactivar materias.'),
    (N'materias.dictadas',               N'Ver las materias que dicta y los estudiantes inscritos en ellas.'),
    (N'inscripciones.propias',           N'Inscribir y cancelar las materias propias.'),
    (N'inscripciones.consultar',         N'Ver las inscripciones de otros estudiantes y los compañeros de cada clase.')
) AS origen (Codigo, Descripcion)
WHERE NOT EXISTS (SELECT 1 FROM Seguridad.Permiso p WHERE p.Codigo = origen.Codigo);
PRINT 'Permisos verificados.';
GO

-- 1.2 Permisos por rol.
INSERT INTO Seguridad.RolPermiso (RolId, PermisoId)
SELECT r.Id, p.Id
FROM (VALUES
    (N'Administrador', N'usuarios.consultar'),
    (N'Administrador', N'usuarios.crear'),
    (N'Administrador', N'usuarios.editar'),
    (N'Administrador', N'usuarios.cambiar-estado'),
    (N'Administrador', N'usuarios.restablecer-contrasena'),
    (N'Administrador', N'materias.gestionar'),
    (N'Administrador', N'inscripciones.consultar'),
    (N'Profesor',      N'materias.dictadas'),
    (N'Estudiante',    N'inscripciones.propias'),
    (N'Estudiante',    N'inscripciones.consultar')
) AS asignacion (Rol, Permiso)
JOIN Seguridad.Rol r     ON r.Nombre = asignacion.Rol
JOIN Seguridad.Permiso p ON p.Codigo = asignacion.Permiso
WHERE NOT EXISTS (SELECT 1 FROM Seguridad.RolPermiso rp WHERE rp.RolId = r.Id AND rp.PermisoId = p.Id);
PRINT 'Permisos por rol verificados.';
GO

-- 2. Niveles académicos
INSERT INTO Academico.Nivel (Nombre)
SELECT origen.Nombre
FROM (VALUES
    (N'Técnico'),
    (N'Tecnólogo'),
    (N'Profesional')
) AS origen (Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Academico.Nivel n WHERE n.Nombre = origen.Nombre);
PRINT 'Niveles académicos verificados.';
GO

-- 3. Categorías de profesor
INSERT INTO Academico.CategoriaProfesor (Nombre)
SELECT origen.Nombre
FROM (VALUES
    (N'Planta'),
    (N'Cátedra'),
    (N'Ocasional')
) AS origen (Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Academico.CategoriaProfesor c WHERE c.Nombre = origen.Nombre);
PRINT 'Categorías de profesor verificadas.';
GO

-- 4. Catálogos de logs
INSERT INTO Logs.TipoEvento (Nombre)
SELECT origen.Nombre
FROM (VALUES
    (N'Informativo'),
    (N'Advertencia'),
    (N'Error')
) AS origen (Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Logs.TipoEvento t WHERE t.Nombre = origen.Nombre);
PRINT 'Tipos de evento verificados.';
GO

INSERT INTO Logs.Aplicacion (Nombre)
SELECT origen.Nombre
FROM (VALUES
    (N'Servicio-Seguridad'),
    (N'Servicio-Academico'),
    (N'Api-Gateway')
) AS origen (Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Logs.Aplicacion a WHERE a.Nombre = origen.Nombre);
PRINT 'Aplicaciones verificadas.';
GO

-- 5. Programa académico
IF NOT EXISTS (SELECT 1 FROM Academico.Programa WHERE Nombre = N'Ingeniería de Sistemas')
BEGIN
    INSERT INTO Academico.Programa (Nombre, NivelId)
    SELECT N'Ingeniería de Sistemas', n.Id
    FROM Academico.Nivel n
    WHERE n.Nombre = N'Profesional';
    PRINT 'Programa académico insertado.';
END
ELSE
    PRINT 'El programa académico ya existe.';
GO

-- 6. Administrador inicial (cuenta sin perfil académico)
IF NOT EXISTS (SELECT 1 FROM Seguridad.Usuario WHERE Identificacion = N'1000000000')
BEGIN
    INSERT INTO Seguridad.Usuario (NombreCompleto, Identificacion, Email, Contrasena, RoleId, DebeCambiarContrasena)
    SELECT N'Administrador',
           N'1000000000',
           N'admin@universidadmectronics.edu',
           N'AQAAAAIAAYagAAAAEK/tcj2e6K1nhPwqBLPuQ9pEITUm0x2+Hs8/0Xb0DhRbnjMV3FG6ECSnj72IF9I1zg==',
           r.Id,
           1
    FROM Seguridad.Rol r
    WHERE r.Nombre = N'Administrador';
    PRINT 'Administrador inicial insertado.';
END
ELSE
    PRINT 'El Administrador inicial ya existe.';
GO

-- 7. Profesores (usuario + perfil) y sus materias
BEGIN TRANSACTION;

DECLARE @Profesores TABLE (
    Identificacion NVARCHAR(50)  NOT NULL,
    NombreCompleto NVARCHAR(150) NOT NULL,
    Email          NVARCHAR(150) NOT NULL,
    Categoria      NVARCHAR(150) NOT NULL,
    Contrasena     NVARCHAR(MAX) NOT NULL
);

INSERT INTO @Profesores (Identificacion, NombreCompleto, Email, Categoria, Contrasena)
VALUES
    (N'1000000001', N'Laura Gómez Restrepo',   N'laura.gomez@universidadmectronics.edu',    N'Planta',
     N'AQAAAAIAAYagAAAAEOdCbEECL7+H3QjyF/S0BfeTzZshiZhjPiniAEMyY6DWPMKzS2TOa0cIvPxJxzBK0A=='),
    (N'1000000002', N'Andrés Martínez Rojas',  N'andres.martinez@universidadmectronics.edu', N'Planta',
     N'AQAAAAIAAYagAAAAEH4wmXAlRuVQZoxIFfApE34gnNH6V1x+hkk/k4Kbm30vTbdKdJU9LBKQBxfPz1HrZg=='),
    (N'1000000003', N'Catalina Herrera Díaz',  N'catalina.herrera@universidadmectronics.edu', N'Cátedra',
     N'AQAAAAIAAYagAAAAEEoCPwfHy8mO+4YYQTO7PMfteC32cJ+fWgXm8teqttKxx6r7vOxpvBNVDD99l5mXTg=='),
    (N'1000000004', N'Jorge Castillo Peña',    N'jorge.castillo@universidadmectronics.edu',  N'Cátedra',
     N'AQAAAAIAAYagAAAAEItuvlbjek/wsFR9mm/o7oLN84KgvVmprDXdBCOtCSF3suINSnaAukmlDGxK/sx7qA=='),
    (N'1000000005', N'Mariana Vargas Ortiz',   N'mariana.vargas@universidadmectronics.edu',  N'Ocasional',
     N'AQAAAAIAAYagAAAAEP1oTb3ONpjqC66kznzzPGt15R6Iv23q+/eZQpr2FvxM8bM5kxc4je3BWDnmPgE1CA==');

DECLARE @RolProfesorId TINYINT = (SELECT Id FROM Seguridad.Rol WHERE Nombre = N'Profesor');

INSERT INTO Seguridad.Usuario (NombreCompleto, Identificacion, Email, Contrasena, RoleId, DebeCambiarContrasena)
SELECT p.NombreCompleto, p.Identificacion, p.Email, p.Contrasena, @RolProfesorId, 1
FROM @Profesores p
WHERE NOT EXISTS (SELECT 1 FROM Seguridad.Usuario u WHERE u.Identificacion = p.Identificacion);

INSERT INTO Academico.Profesor (UsuarioId, CategoriaId)
SELECT u.Id, c.Id
FROM @Profesores p
JOIN Seguridad.Usuario u ON u.Identificacion = p.Identificacion
JOIN Academico.CategoriaProfesor c ON c.Nombre = p.Categoria
WHERE NOT EXISTS (SELECT 1 FROM Academico.Profesor pr WHERE pr.UsuarioId = u.Id);

PRINT 'Profesores verificados.';

-- 10 materias de 3 créditos; cada profesor dicta exactamente 2.
DECLARE @Materias TABLE (
    Nombre                 NVARCHAR(100) NOT NULL,
    IdentificacionProfesor NVARCHAR(50)  NOT NULL
);

INSERT INTO @Materias (Nombre, IdentificacionProfesor)
VALUES
    (N'Cálculo Diferencial',      N'1000000001'),
    (N'Álgebra Lineal',           N'1000000001'),
    (N'Programación I',           N'1000000002'),
    (N'Estructuras de Datos',     N'1000000002'),
    (N'Bases de Datos',           N'1000000003'),
    (N'Ingeniería de Software',   N'1000000003'),
    (N'Redes de Computadores',    N'1000000004'),
    (N'Sistemas Operativos',      N'1000000004'),
    (N'Física Mecánica',          N'1000000005'),
    (N'Probabilidad y Estadística', N'1000000005');

DECLARE @ProgramaId INT = (SELECT Id FROM Academico.Programa WHERE Nombre = N'Ingeniería de Sistemas');

INSERT INTO Academico.Materia (Nombre, Creditos, ProgramaId, ProfesorId)
SELECT m.Nombre, 3, @ProgramaId, pr.Id
FROM @Materias m
JOIN Seguridad.Usuario u ON u.Identificacion = m.IdentificacionProfesor
JOIN Academico.Profesor pr ON pr.UsuarioId = u.Id
WHERE NOT EXISTS (
    SELECT 1 FROM Academico.Materia ma
    WHERE ma.ProgramaId = @ProgramaId AND ma.Nombre = m.Nombre
);

PRINT 'Materias verificadas.';

COMMIT TRANSACTION;
GO

-- 8. Estudiantes de ejemplo (usuario + perfil). Misma contraseña temporal que las demás cuentas: Mectronics#2026.
BEGIN TRANSACTION;

DECLARE @Estudiantes TABLE (
    Identificacion     NVARCHAR(50)  NOT NULL,
    NombreCompleto     NVARCHAR(150) NOT NULL,
    Email              NVARCHAR(150) NOT NULL,
    UbicacionSemestral INT           NOT NULL,
    Contrasena         NVARCHAR(MAX) NOT NULL
);

INSERT INTO @Estudiantes (Identificacion, NombreCompleto, Email, UbicacionSemestral, Contrasena)
VALUES
    (N'2000000001', N'Sofía Ramírez Castro',    N'sofia.ramirez@universidadmectronics.edu',      5,
     N'AQAAAAIAAYagAAAAEGQLZU8lDbs7SgdQdTyodu3mGbug5kTPEokDA2qlLt4X881I+pDJGoRg/A/Ae40PZA=='),
    (N'2000000002', N'Daniel Torres Mejía',     N'daniel.torres@universidadmectronics.edu',      3,
     N'AQAAAAIAAYagAAAAELk6gshIKkhghhk26uhDU8dfmP89fnGmoLs0dDaURa/EzxnhyYoU5r4C7W2i1/cspw=='),
    (N'2000000003', N'Valentina López Ríos',    N'valentina.lopez@universidadmectronics.edu',    4,
     N'AQAAAAIAAYagAAAAEPl9NG2vUlA/QZRKCAzDNujfSwqukvEMxZVtnhJ3bmhRJhqIfo4RLcIa1k/IRnGGfw=='),
    (N'2000000004', N'Santiago Moreno Gil',     N'santiago.moreno@universidadmectronics.edu',    2,
     N'AQAAAAIAAYagAAAAEIY6vPLgRhFznoq5jArg8PjzZRHlKaSwLfVSJFZ89ffxcDxUOIAWD3pFJoRCDzZE/w=='),
    (N'2000000005', N'Isabella Cárdenas Ruiz',  N'isabella.cardenas@universidadmectronics.edu',  6,
     N'AQAAAAIAAYagAAAAEGXMZoPaWkr9xF16skQUKJgakZcLqL3Jc9LAhoAUKEn5/6cRG6XK5UYrWcwvKqMMBA=='),
    (N'2000000006', N'Mateo Rojas Silva',       N'mateo.rojas@universidadmectronics.edu',        1,
     N'AQAAAAIAAYagAAAAEIXGqp1gn+EtkzRJTS+fxWUVE5RNMgzmuN0ISoc+kkSP6BPB3RhEf9Gmgr0ShsMukQ=='),
    (N'2000000007', N'Camila Ortega Pardo',     N'camila.ortega@universidadmectronics.edu',      2,
     N'AQAAAAIAAYagAAAAEANNQxgTutAYCwQyom7rbommuQ1y8pd10aHQXYvKGQ5LoxcBrgqis2uZb36IyyWE3g=='),
    (N'2000000008', N'Nicolás Herrera Vega',    N'nicolas.herrera@universidadmectronics.edu',    1,
     N'AQAAAAIAAYagAAAAEKk08+xDFU0IKd7F7UI2avJpnLUZHbCqcbEP8DnaZ7QjixojKlWJscayo1X4XDARJg==');

DECLARE @RolEstudianteId TINYINT = (SELECT Id FROM Seguridad.Rol WHERE Nombre = N'Estudiante');
DECLARE @ProgramaEstudiantesId INT = (SELECT Id FROM Academico.Programa WHERE Nombre = N'Ingeniería de Sistemas');

INSERT INTO Seguridad.Usuario (NombreCompleto, Identificacion, Email, Contrasena, RoleId, DebeCambiarContrasena)
SELECT e.NombreCompleto, e.Identificacion, e.Email, e.Contrasena, @RolEstudianteId, 1
FROM @Estudiantes e
WHERE NOT EXISTS (SELECT 1 FROM Seguridad.Usuario u WHERE u.Identificacion = e.Identificacion);

INSERT INTO Academico.Estudiante (UsuarioId, ProgramaId, UbicacionSemestral)
SELECT u.Id, @ProgramaEstudiantesId, e.UbicacionSemestral
FROM @Estudiantes e
JOIN Seguridad.Usuario u ON u.Identificacion = e.Identificacion
WHERE NOT EXISTS (SELECT 1 FROM Academico.Estudiante es WHERE es.UsuarioId = u.Id);

PRINT 'Estudiantes verificados.';

-- 9. Inscripciones de ejemplo. Cumplen las reglas: máximo 3 materias (9 créditos) y sin repetir profesor.
-- Camila Ortega y Nicolás Herrera quedan sin inscripciones para probar el flujo desde cero.
-- Solo se cargan a estudiantes que aún no tienen ninguna inscripción (ni vigente ni cancelada), así volver a
-- ejecutar el script no reinscribe materias que el estudiante canceló desde la aplicación.
DECLARE @Inscripciones TABLE (
    IdentificacionEstudiante NVARCHAR(50)  NOT NULL,
    Materia                  NVARCHAR(100) NOT NULL
);

INSERT INTO @Inscripciones (IdentificacionEstudiante, Materia)
VALUES
    (N'2000000001', N'Cálculo Diferencial'),
    (N'2000000001', N'Programación I'),
    (N'2000000001', N'Bases de Datos'),
    (N'2000000002', N'Cálculo Diferencial'),
    (N'2000000002', N'Programación I'),
    (N'2000000002', N'Redes de Computadores'),
    (N'2000000003', N'Álgebra Lineal'),
    (N'2000000003', N'Bases de Datos'),
    (N'2000000003', N'Física Mecánica'),
    (N'2000000004', N'Programación I'),
    (N'2000000004', N'Ingeniería de Software'),
    (N'2000000005', N'Cálculo Diferencial'),
    (N'2000000005', N'Sistemas Operativos'),
    (N'2000000006', N'Estructuras de Datos');

INSERT INTO Academico.Inscripcion (EstudianteId, MateriaId)
SELECT es.Id, m.Id
FROM @Inscripciones i
JOIN Seguridad.Usuario u     ON u.Identificacion = i.IdentificacionEstudiante
JOIN Academico.Estudiante es ON es.UsuarioId = u.Id
JOIN Academico.Materia m     ON m.Nombre = i.Materia AND m.ProgramaId = es.ProgramaId
WHERE NOT EXISTS (SELECT 1 FROM Academico.Inscripcion ins WHERE ins.EstudianteId = es.Id);

PRINT 'Inscripciones de ejemplo verificadas.';

COMMIT TRANSACTION;
GO

PRINT 'DML finalizado.';
GO
