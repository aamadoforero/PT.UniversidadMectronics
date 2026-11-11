/* =====================================================================
   Universidad Mectronics - DML
   Datos semilla: catálogos, un programa, un SuperAdministrador, 5 profesores
   y 10 materias (2 por profesor, 3 créditos cada una). El script es idempotente.

   Cuentas sembradas (SuperAdministrador y profesores):
   - Contraseña temporal de todas: Mectronics#2026
   - DebeCambiarContrasena = 1, así que deben cambiarla en el primer
     inicio de sesión.
   - Los hashes se generaron con PasswordHasher de ASP.NET Core
     (formato V3, PBKDF2-HMAC-SHA512, 100.000 iteraciones).
   ===================================================================== */

USE UniversidadMectronics;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
-- Requeridos para escribir en tablas con índices filtrados (Academico.Inscripcion).
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Roles. TipoPerfil: 0 = Ninguno, 1 = Estudiante, 2 = Profesor.
MERGE Seguridad.Rol AS destino
USING (VALUES
    (N'SuperAdministrador', 0),
    (N'Administrador',      0),
    (N'Profesor',           2),
    (N'Estudiante',         1)
) AS origen (Nombre, TipoPerfil)
ON destino.Nombre = origen.Nombre
WHEN MATCHED AND destino.TipoPerfil <> origen.TipoPerfil THEN
    UPDATE SET TipoPerfil = origen.TipoPerfil
WHEN NOT MATCHED THEN
    INSERT (Nombre, TipoPerfil) VALUES (origen.Nombre, origen.TipoPerfil);
PRINT 'Roles verificados.';
GO

-- 1.1 Permisos. Los códigos deben coincidir con las constantes de cada microservicio.
MERGE Seguridad.Permiso AS destino
USING (VALUES
    -- Servicio de Seguridad
    (N'usuarios.consultar',               N'Listar y consultar cuentas de usuario.'),
    (N'usuarios.crear',                   N'Crear cuentas de usuario.'),
    (N'usuarios.editar',                  N'Editar los datos y el perfil académico de una cuenta.'),
    (N'usuarios.cambiar-estado',          N'Activar o desactivar cuentas.'),
    (N'usuarios.restablecer-contrasena',  N'Generar una contraseña temporal para una cuenta.'),
    (N'usuarios.cambiar-rol',             N'Cambiar el rol de una cuenta.'),
    (N'superadministradores.gestionar',   N'Acceso total. Identifica el rol de superadministración: su única cuenta la crea este script, está protegida y el rol no se asigna desde la aplicación.'),
    -- Servicio Académico
    (N'materias.gestionar',               N'Crear, editar y desactivar materias.'),
    (N'inscripciones.propias',            N'Inscribir y cancelar las materias propias.'),
    (N'inscripciones.consultar',          N'Ver las inscripciones de otros estudiantes y los compañeros de cada clase.')
) AS origen (Codigo, Descripcion)
ON destino.Codigo = origen.Codigo
WHEN MATCHED AND destino.Descripcion <> origen.Descripcion THEN
    UPDATE SET Descripcion = origen.Descripcion
WHEN NOT MATCHED THEN
    INSERT (Codigo, Descripcion) VALUES (origen.Codigo, origen.Descripcion);
PRINT 'Permisos verificados.';
GO

-- 1.2 Permisos por rol. SuperAdministrador recibe todos los permisos existentes.
MERGE Seguridad.RolPermiso AS destino
USING (
    SELECT r.Id AS RolId, p.Id AS PermisoId
    FROM Seguridad.Rol r
    CROSS JOIN Seguridad.Permiso p
    WHERE r.Nombre = N'SuperAdministrador'

    UNION

    SELECT r.Id, p.Id
    FROM (VALUES
        (N'Administrador', N'usuarios.consultar'),
        (N'Administrador', N'usuarios.crear'),
        (N'Administrador', N'usuarios.editar'),
        (N'Administrador', N'usuarios.cambiar-estado'),
        (N'Administrador', N'usuarios.restablecer-contrasena'),
        (N'Administrador', N'usuarios.cambiar-rol'),
        (N'Administrador', N'materias.gestionar'),
        (N'Administrador', N'inscripciones.consultar'),
        (N'Profesor',      N'inscripciones.consultar'),
        (N'Estudiante',    N'inscripciones.propias'),
        (N'Estudiante',    N'inscripciones.consultar')
    ) AS asignacion (Rol, Permiso)
    JOIN Seguridad.Rol r     ON r.Nombre = asignacion.Rol
    JOIN Seguridad.Permiso p ON p.Codigo = asignacion.Permiso
) AS origen
ON destino.RolId = origen.RolId AND destino.PermisoId = origen.PermisoId
WHEN NOT MATCHED THEN
    INSERT (RolId, PermisoId) VALUES (origen.RolId, origen.PermisoId);
PRINT 'Permisos por rol verificados.';
GO

-- 2. Niveles académicos
MERGE Academico.Nivel AS destino
USING (VALUES
    (N'Técnico'),
    (N'Tecnólogo'),
    (N'Profesional')
) AS origen (Nombre)
ON destino.Nombre = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre) VALUES (origen.Nombre);
PRINT 'Niveles académicos verificados.';
GO

-- 3. Categorías de profesor
MERGE Academico.CategoriaProfesor AS destino
USING (VALUES
    (N'Planta'),
    (N'Cátedra'),
    (N'Ocasional')
) AS origen (Nombre)
ON destino.Nombre = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre) VALUES (origen.Nombre);
PRINT 'Categorías de profesor verificadas.';
GO

-- 4. Catálogos de logs
MERGE Logs.TipoEvento AS destino
USING (VALUES
    (N'Informativo'),
    (N'Advertencia'),
    (N'Error')
) AS origen (Nombre)
ON destino.Nombre = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre) VALUES (origen.Nombre);
PRINT 'Tipos de evento verificados.';
GO

MERGE Logs.Aplicacion AS destino
USING (VALUES
    (N'Servicio-Seguridad'),
    (N'Servicio-Academico'),
    (N'Api-Gateway')
) AS origen (Nombre)
ON destino.Nombre = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre) VALUES (origen.Nombre);
PRINT 'Aplicaciones verificadas.';
GO

-- 5. Programa académico
MERGE Academico.Programa AS destino
USING (
    SELECT N'Ingeniería de Sistemas' AS Nombre, n.Id AS NivelId
    FROM Academico.Nivel n
    WHERE n.Nombre = N'Profesional'
) AS origen
ON destino.Nombre = origen.Nombre
WHEN NOT MATCHED THEN
    INSERT (Nombre, NivelId) VALUES (origen.Nombre, origen.NivelId);
PRINT 'Programa académico verificado.';
GO

-- 6. SuperAdministrador inicial (cuenta sin perfil académico)
IF NOT EXISTS (SELECT 1 FROM Seguridad.Usuario WHERE Identificacion = N'1000000000')
BEGIN
    INSERT INTO Seguridad.Usuario (NombreCompleto, Identificacion, Email, Contrasena, RoleId, DebeCambiarContrasena)
    SELECT N'Super Administrador',
           N'1000000000',
           N'superadmin@universidadmectronics.edu',
           N'AQAAAAIAAYagAAAAEK/tcj2e6K1nhPwqBLPuQ9pEITUm0x2+Hs8/0Xb0DhRbnjMV3FG6ECSnj72IF9I1zg==',
           r.Id,
           1
    FROM Seguridad.Rol r
    WHERE r.Nombre = N'SuperAdministrador';
    PRINT 'SuperAdministrador inicial insertado.';
END
ELSE
    PRINT 'El SuperAdministrador inicial ya existe.';
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

PRINT 'DML finalizado.';
GO
