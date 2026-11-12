#Requires -Version 7.0
<#
.SYNOPSIS
    Configura los secretos de desarrollo de los microservicios de Seguridad, Académico y Logs con "dotnet user-secrets" (Windows, PowerShell 7).

.DESCRIPTION
    Guarda, fuera del repositorio:
      Api.Seguridad
        ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
        Jwt:ClavePrivadaAcceso                   clave privada RSA 2048 (PKCS#8, Base64)
        Jwt:ClaveRenovacion                      32 bytes aleatorios (Base64)
        Jwt:ClaveHuella                          32 bytes aleatorios (Base64)
        Logs:ClaveAplicacion                     clave de Seguridad ante el microservicio de Logs
      Api.Academico
        ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
        Logs:ClaveAplicacion                     clave de Académico ante el microservicio de Logs
      Api.Logs
        ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
        ClavesAplicacion:Servicio-Seguridad      la misma clave de Seguridad
        ClavesAplicacion:Servicio-Academico      la misma clave de Académico
    Volver a ejecutarlo genera claves nuevas: las sesiones abiertas dejan de valer.
    Detalle completo: README.md (sección Secretos)

.EXAMPLE
    pwsh -File .\scripts\configurar-secretos.ps1 -CadenaConexion 'Server=localhost,1433;Database=UniversidadMectronics;...'
#>
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string] $CadenaConexion
)

$ErrorActionPreference = 'Stop'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "No se encontró 'dotnet'. Instale el SDK de .NET 10 y vuelva a intentarlo."
}

$raiz = Split-Path -Parent $PSScriptRoot
$proyectoSeguridad = Join-Path $raiz 'Back/Universidad.Mectronics.Seguridad/Api.Seguridad/Api.Seguridad.csproj'
$proyectoAcademico = Join-Path $raiz 'Back/Universidad.Mectronics.Academico/Api.Academico/Api.Academico.csproj'
$proyectoLogs = Join-Path $raiz 'Back/Universidad.Mectronics.Logs/Api.Logs/Api.Logs.csproj'

function Guardar-Secreto([string] $Proyecto, [string] $Nombre, [string] $Valor) {
    dotnet user-secrets set $Nombre $Valor --project $Proyecto | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "No se pudo guardar el secreto '$Nombre'."
    }
    Write-Host "  - $Nombre"
}

function Obtener-BytesAleatoriosBase64([int] $Cantidad) {
    return [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes($Cantidad))
}

# Las claves de Logs van en hexadecimal porque viajan en una cabecera HTTP.
$claveLogsSeguridad = [Convert]::ToHexString([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32)).ToLowerInvariant()
$claveLogsAcademico = [Convert]::ToHexString([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(32)).ToLowerInvariant()

Write-Host 'Configurando los secretos de desarrollo de Api.Seguridad...'
Guardar-Secreto $proyectoSeguridad 'ConnectionStrings:UniversidadMectronics' $CadenaConexion

$rsa = [System.Security.Cryptography.RSA]::Create(2048)
try {
    Guardar-Secreto $proyectoSeguridad 'Jwt:ClavePrivadaAcceso' ([Convert]::ToBase64String($rsa.ExportPkcs8PrivateKey()))
}
finally {
    $rsa.Dispose()
}

Guardar-Secreto $proyectoSeguridad 'Jwt:ClaveRenovacion' (Obtener-BytesAleatoriosBase64 32)
Guardar-Secreto $proyectoSeguridad 'Jwt:ClaveHuella' (Obtener-BytesAleatoriosBase64 32)
Guardar-Secreto $proyectoSeguridad 'Logs:ClaveAplicacion' $claveLogsSeguridad

Write-Host ''
Write-Host 'Configurando los secretos de desarrollo de Api.Academico...'
Guardar-Secreto $proyectoAcademico 'ConnectionStrings:UniversidadMectronics' $CadenaConexion
Guardar-Secreto $proyectoAcademico 'Logs:ClaveAplicacion' $claveLogsAcademico

Write-Host ''
Write-Host 'Configurando los secretos de desarrollo de Api.Logs...'
Guardar-Secreto $proyectoLogs 'ConnectionStrings:UniversidadMectronics' $CadenaConexion
Guardar-Secreto $proyectoLogs 'ClavesAplicacion:Servicio-Seguridad' $claveLogsSeguridad
Guardar-Secreto $proyectoLogs 'ClavesAplicacion:Servicio-Academico' $claveLogsAcademico

Write-Host ''
Write-Host 'Secretos configurados (los valores se ocultan):'
foreach ($proyecto in @($proyectoSeguridad, $proyectoAcademico, $proyectoLogs)) {
    Write-Host "  $([System.IO.Path]::GetFileNameWithoutExtension($proyecto))"
    dotnet user-secrets list --project $proyecto | ForEach-Object { '    ' + ($_ -replace ' = .*', ' = ********') }
}
