# Universidad.Mectronics.Seguridad

Microservicio de autenticación, sesión y administración de cuentas.

## Stack

- .NET 10 / ASP.NET Core (controladores)
- Entity Framework Core 10 + SQL Server 2022
- FluentValidation 12
- xUnit
- JWT RS256 para el token de acceso, HMAC-SHA256 para el de renovación

## Estructura

```
Back/
├── Universidad.Mectronics.Seguridad/
│   ├── Universidad.Mectronics.Seguridad.slnx
│   ├── Dominio.Seguridad            Entidades, objetos de valor y reglas del dominio
│   ├── Aplicacion.Seguridad         Casos de uso, DTOs, validadores e interfaces
│   ├── Infraestructura.Seguridad    EF Core, repositorios, JWT, hash de contraseñas, auditoría
│   ├── Api.Seguridad                Controladores, autenticación, CSRF, manejo de errores
│   └── PruebasUnitarias.Seguridad   Pruebas unitarias de los casos de uso
└── Universidad.Mectronics.Transversal   Resultado/Error, paginación, nombres de claims y cookies
Database/              Scripts DDL, DML y Metadata
scripts/               Configuración de secretos (Linux/macOS y Windows)
ColeccionPostman.json  Colección de Postman
```

Las dependencias van hacia adentro: `Api → Aplicacion → Dominio`, e `Infraestructura` implementa las interfaces de `Aplicacion`.

## Requisitos

- SDK de .NET 10
- Docker (o un SQL Server propio)
- Linux/macOS: `openssl`. Windows: PowerShell 7

## 1. Base de datos

Levantar SQL Server:

```bash
docker run -d --name sql-server-mectronics -e ACCEPT_EULA=Y -e 'MSSQL_SA_PASSWORD=<ContrasenaSa>' \
  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

Ejecutar los scripts de `Database/` en este orden: `DDL.sql`, `DML.sql`, `Metadata.sql`. Desde la raíz del repositorio:


## 2. Secretos

No hay secretos en el repositorio. Se guardan con `dotnet user-secrets` (solo se cargan en `Development`):

| Clave | Contenido |
|---|---|
| `ConnectionStrings:UniversidadMectronics` | Cadena de conexión |
| `Jwt:ClavePrivadaAcceso` | Clave RSA 2048, PKCS#8 en Base64 (una línea) |
| `Jwt:ClaveRenovacion` | 32 bytes aleatorios en Base64 |
| `Jwt:ClaveHuella` | 32 bytes aleatorios en Base64 |

El script los genera y guarda todos:

```bash
# Linux / macOS
./scripts/configurar-secretos.sh 'Server=localhost,1433;Database=UniversidadMectronics;User Id=sa;Password=<ContrasenaSa>;TrustServerCertificate=True'
```

```powershell
# Windows (PowerShell 7)
pwsh -ExecutionPolicy Bypass -File .\scripts\configurar-secretos.ps1 `
  -CadenaConexion 'Server=localhost,1433;Database=UniversidadMectronics;User Id=sa;Password=<ContrasenaSa>;TrustServerCertificate=True'
```

Las claves JWT solo las usa la API internamente; no hay que conocerlas ni configurarlas en ningún cliente. Para verlas: `dotnet user-secrets list --project Back/Universidad.Mectronics.Seguridad/Api.Seguridad`. Volver a ejecutar el script las cambia y cierra las sesiones abiertas.

Si falta algún secreto la API no arranca y el error dice cuál. Quedan en `~/.microsoft/usersecrets/2718dd79-12d1-4fc8-a6f6-bc9d6bbbaa70/secrets.json` (Windows: `%APPDATA%\Microsoft\UserSecrets\2718dd79-12d1-4fc8-a6f6-bc9d6bbbaa70\secrets.json`).

| Problema | Solución |
|---|---|
| *Falta la configuración 'Jwt:...'* al arrancar | Ejecutar el script |
| Error con `ClavePrivadaAcceso` | La clave no está en PKCS#8 o tiene saltos de línea; regenerarla con el script |
| `Permission denied` al ejecutar el script | `chmod +x scripts/configurar-secretos.sh` |
| Windows: `ExportPkcs8PrivateKey` o `GetBytes` no existe | Usar PowerShell 7 (`pwsh`), no Windows PowerShell 5.1 |
| Windows: la ejecución de scripts está deshabilitada | Ejecutarlo con `pwsh -ExecutionPolicy Bypass -File ...` |
| `Login failed for user 'sa'` | Revisar la contraseña y pasarla entre comillas simples |
| Error de `certificate chain` | Agregar `TrustServerCertificate=True` a la cadena |

Fuera de `Development` los mismos valores se pasan como variables de entorno, con `__` en lugar de `:` (`Jwt__ClaveHuella`).

## 3. Ejecutar

```bash
dotnet run --project Back/Universidad.Mectronics.Seguridad/Api.Seguridad --launch-profile http
```

| | URL |
|---|---|
| API | http://localhost:5090 |
| Swagger | http://localhost:5090/swagger |
| OpenAPI | http://localhost:5090/openapi/v1.json |
| JWKS | http://localhost:5090/.well-known/jwks.json |

Usuario inicial: `superadmin@universidadmectronics.edu` / `Mectronics#2026`. La contraseña es temporal; hasta cambiarla con `PUT /api/seguridad/cuenta/contrasena` el resto de endpoints responde 403.

## 4. Pruebas

```bash
dotnet test Back/Universidad.Mectronics.Seguridad/PruebasUnitarias.Seguridad
```

No usan base de datos ni secretos; todo corre en memoria.

## 5. Postman

Colección en [ColeccionPostman.json](ColeccionPostman.json). Maneja las cookies y el token CSRF automáticamente; las carpetas están numeradas en el orden de ejecución.

## Notas técnicas

- **Sesión en cookies.** `universidad-acceso` y `universidad-renovacion` son `HttpOnly` y `SameSite=Strict`; los tokens nunca van en el cuerpo de la respuesta. El cliente debe enviar las peticiones con credenciales (`credentials: 'include'`).
- **CSRF.** Todo `POST`, `PUT` o `DELETE` con cookies de sesión debe llevar la cabecera `X-CSRF-TOKEN` con el valor de la cookie `universidad-csrf` (double-submit cookie). El inicio de sesión no la necesita.
- **Tokens.** El de acceso dura 15 minutos y se firma con RS256; otros servicios lo validan con la clave pública de `/.well-known/jwks.json`. El de renovación dura 7 días y no se guarda en base de datos: lleva una huella del hash de la contraseña, así que cambiarla invalida todas las sesiones anteriores.
- **Validación por petición.** En cada request autenticado se verifica en base de datos que la cuenta siga activa, con el mismo rol y la misma contraseña.
- **Errores.** Todas las respuestas de error son Problem Details (RFC 9457) con `codigo`, `errores` (por campo) y `traceId`.
- **Límite de intentos.** 10 inicios de sesión por minuto por IP; después responde 429.
- **Auditoría.** Los eventos y errores se guardan en `Logs.RegistroEvento` con la aplicación `Servicio-Seguridad`.
- **Fechas.** Columnas `DATETIMEOFFSET`, siempre en UTC.