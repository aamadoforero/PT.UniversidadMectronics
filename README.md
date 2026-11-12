# Universidad Mectronics

Backend en microservicios .NET detrás de un API Gateway, front en React y base de datos SQL Server.

| Componente | Responsabilidad | Puerto |
|---|---|---|
| Front | Aplicación web (React) | 5173 |
| API Gateway | Punto de entrada de las APIs (YARP) | 5080 |
| Seguridad | Sesión, cuentas de usuario y claves públicas (JWKS) | 5090 |
| Académico | Materias e inscripciones | 5091 |
| Logs | Auditoría y errores; solo lo llaman los otros microservicios | 5092 |

**Stack:** .NET 10, ASP.NET Core, YARP, Entity Framework Core 10, FluentValidation, xUnit, SQL Server 2022, React 19, TypeScript, Vite y MUI.

## Estructura

```
Back/
├── Universidad.Mectronics.Seguridad/    Dominio, Aplicacion, Infraestructura, Api y PruebasUnitarias
├── Universidad.Mectronics.Academico/    Dominio, Aplicacion, Infraestructura, Api y PruebasUnitarias
├── Universidad.Mectronics.Logs/         Dominio, Aplicacion, Infraestructura y Api
├── Universidad.Mectronics.Gateway/      Api.Gateway
└── Universidad.Mectronics.Transversal/  Código compartido: Resultado/Error, paginación, auditoría, cliente entre microservicios con resiliencia
Front/                 Aplicación React
Database/              DDL.sql, DML.sql y Metadata.sql
docker/                Script que genera los secretos del entorno Docker
scripts/               Configuración de secretos para ejecutar sin Docker
docker-compose.yml
ColeccionPostman.json
```

## 1. Base de datos

Se necesita un SQL Server 2019 o superior. Ejecute los scripts de `Database/` en este orden: `DDL.sql`, `DML.sql` y `Metadata.sql`. Crean la base `UniversidadMectronics` con sus datos iniciales y se pueden volver a ejecutar sin perder datos.

Con `sqlcmd`, desde la raíz del repositorio:

```bash
sqlcmd -S localhost,1433 -U sa -P '<Contraseña>' -C -i Database/DDL.sql
sqlcmd -S localhost,1433 -U sa -P '<Contraseña>' -C -i Database/DML.sql
sqlcmd -S localhost,1433 -U sa -P '<Contraseña>' -C -i Database/Metadata.sql
```

También se pueden abrir y ejecutar desde SQL Server Management Studio o la extensión SQL Server de VS Code.

## 2. Ejecutar con Docker (recomendado)

Requisitos: Docker con Docker Compose. Funciona en Linux, macOS y Windows (Docker Desktop con contenedores Linux).

El SQL Server debe aceptar conexiones por TCP con autenticación de SQL Server (usuario y contraseña); los contenedores no pueden usar autenticación de Windows. En un SQL Server instalado en Windows, habilite TCP/IP en el puerto 1433 desde SQL Server Configuration Manager y la autenticación mixta, y permita el puerto en el firewall.

1. Copie `.env.example` como `.env` y ponga la cadena de conexión de su SQL Server. Si SQL Server corre en el mismo equipo (instalado o en un contenedor con el puerto 1433 publicado), use `host.docker.internal` como servidor:

   ```
   CADENA_CONEXION=Server=host.docker.internal,1433;Database=UniversidadMectronics;User Id=sa;Password=<Contraseña>;TrustServerCertificate=True
   ```

2. Levante todo:

   ```bash
   docker compose up -d --build
   ```

3. Abra http://localhost:5173.

No hay que configurar más secretos: al primer arranque el servicio `secretos` genera las claves JWT y las claves de Logs, y las guarda en el volumen `secretos`. Los microservicios corren en modo `Development`, así que Swagger queda disponible en http://localhost:5090/swagger y http://localhost:5091/swagger.

| Comando | Qué hace |
|---|---|
| `docker compose logs -f seguridad` | Muestra el log de un servicio |
| `docker compose down` | Detiene todo |
| `docker compose down -v` | Detiene todo y borra las claves generadas (las sesiones abiertas dejan de valer) |
| `docker compose exec logs cat /secretos/logs.env` | Muestra las claves de Logs (para Postman) |

## 3. Ejecutar sin Docker

Requisitos: SDK de .NET 10, Node 20 o superior y `openssl` (Linux/macOS) o PowerShell 7 (Windows).

### Secretos

Los secretos no están en el repositorio; se guardan con `dotnet user-secrets`. El script los genera y los configura en Seguridad, Académico y Logs; solo necesita la cadena de conexión:

```bash
# Linux / macOS
./scripts/configurar-secretos.sh 'Server=localhost,1433;Database=UniversidadMectronics;User Id=sa;Password=<Contraseña>;TrustServerCertificate=True'
```

```powershell
# Windows (PowerShell 7)
pwsh -ExecutionPolicy Bypass -File .\scripts\configurar-secretos.ps1 `
  -CadenaConexion 'Server=localhost,1433;Database=UniversidadMectronics;User Id=sa;Password=<Contraseña>;TrustServerCertificate=True'
```

| Microservicio | Clave | Contenido |
|---|---|---|
| Seguridad | `ConnectionStrings:UniversidadMectronics` | Cadena de conexión |
| Seguridad | `Jwt:ClavePrivadaAcceso` | Clave RSA 2048 en PKCS#8 y Base64 |
| Seguridad | `Jwt:ClaveRenovacion` | 32 bytes aleatorios en Base64 |
| Seguridad | `Jwt:ClaveHuella` | 32 bytes aleatorios en Base64 |
| Seguridad | `Logs:ClaveAplicacion` | Clave de Seguridad ante Logs |
| Académico | `ConnectionStrings:UniversidadMectronics` | Cadena de conexión |
| Académico | `Logs:ClaveAplicacion` | Clave de Académico ante Logs |
| Logs | `ConnectionStrings:UniversidadMectronics` | Cadena de conexión |
| Logs | `ClavesAplicacion:Servicio-Seguridad` | La misma clave de Seguridad |
| Logs | `ClavesAplicacion:Servicio-Academico` | La misma clave de Académico |

Fuera de `Development` los mismos valores se pasan como variables de entorno, con `__` en lugar de `:` (por ejemplo `Jwt__ClaveHuella`).

### Microservicios

Desde VS Code (extensión C#): en *Ejecutar y depurar* elija **Todos los microservicios** y presione F5.

Desde la terminal, cada uno en su propia ventana y desde la raíz del repositorio:

```bash
dotnet run --project Back/Universidad.Mectronics.Logs/Api.Logs --launch-profile http
dotnet run --project Back/Universidad.Mectronics.Seguridad/Api.Seguridad --launch-profile http
dotnet run --project Back/Universidad.Mectronics.Academico/Api.Academico --launch-profile http
dotnet run --project Back/Universidad.Mectronics.Gateway/Api.Gateway --launch-profile http
```

### Front

```bash
cd Front
npm install
npm run dev
```

Queda en http://localhost:5173. Vite redirige `/api` al gateway (http://localhost:5080, configurable con la variable `URL_API_GATEWAY`).

| Comando | Qué hace |
|---|---|
| `npm run dev` | Servidor de desarrollo |
| `npm run build` | Verifica tipos y genera `dist/` |
| `npm run lint` | Revisa el código con oxlint |

## 4. Cuentas iniciales

Todas las cuentas que crea `DML.sql` tienen la contraseña temporal `Mectronics#2026` y piden cambiarla en el primer inicio de sesión.

| Rol | Correo |
|---|---|
| Administrador | `admin@universidadmectronics.edu` |
| Profesor | `laura.gomez@universidadmectronics.edu` (y 4 más) |
| Estudiante | `sofia.ramirez@universidadmectronics.edu` (y 7 más) |

Después de 5 intentos fallidos la cuenta se bloquea 15 minutos (3 minutos al ejecutar sin Docker en `Development`).

## 5. Pruebas

```bash
dotnet test Back/Universidad.Mectronics.Seguridad/PruebasUnitarias.Seguridad
dotnet test Back/Universidad.Mectronics.Academico/PruebasUnitarias.Academico
```

Corren en memoria: no necesitan base de datos ni secretos.

## 6. Postman

Importe [ColeccionPostman.json](ColeccionPostman.json). Tiene una petición por endpoint, organizadas por microservicio.

- Las peticiones van al gateway (`baseUrl`). Postman guarda las cookies de sesión y la colección agrega sola la cabecera `X-CSRF-TOKEN`.
- Empiece por *Iniciar sesión* y *Cambiar contraseña*: con la contraseña temporal los demás endpoints responden 403. Después actualice la variable `contrasena`.
- Para probar otro rol, cambie `email` y `contrasena` y vuelva a iniciar sesión.
- Logs se llama directo (`baseUrlLogs`) con la variable `claveLogs`: el valor de `ClavesAplicacion__Servicio-Seguridad` (Docker) o de `ClavesAplicacion:Servicio-Seguridad` en `dotnet user-secrets list --project Back/Universidad.Mectronics.Logs/Api.Logs`.

## Problemas comunes

| Problema | Solución |
|---|---|
| `CADENA_CONEXION` no definida al levantar Docker | Falta el archivo `.env` (paso 2.1) |
| `Login failed for user` o el servicio no conecta a la base | Revise la cadena de conexión; en Docker el servidor debe ser `host.docker.internal`, no `localhost` |
| Windows: no conecta a un SQL Server Express (`.\SQLEXPRESS`) | Las instancias con nombre usan un puerto dinámico; fije el puerto 1433 en SQL Server Configuration Manager (TCP/IP) |
| Windows: el servicio `secretos` falla con `\r` o `illegal option` | El repositorio se clonó con saltos de línea CRLF; vuelva a clonarlo (el `.gitattributes` fuerza LF en los `.sh`) |
| Error de `certificate chain` | Agregue `TrustServerCertificate=True` a la cadena |
| *Falta la configuración '...'* al arrancar sin Docker | Ejecute el script de secretos |
| Académico responde 401 con una sesión válida | Seguridad no está arriba o se regeneraron las claves; inicie sesión de nuevo |
| Un puerto ya está en uso | Detenga la ejecución local (o la de Docker) antes de iniciar la otra |
| Windows: `ExportPkcs8PrivateKey` no existe | Use PowerShell 7 (`pwsh`), no Windows PowerShell 5.1 |
