#Requires -Version 7.0
<#
.SYNOPSIS
    Configura los secretos de desarrollo del servicio de Seguridad con "dotnet user-secrets" (Windows, PowerShell 7).

.DESCRIPTION
    Guarda, fuera del repositorio:
      ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
      Jwt:ClavePrivadaAcceso                   clave privada RSA 2048 (PKCS#8, Base64)
      Jwt:ClaveRenovacion                      32 bytes aleatorios (Base64)
      Jwt:ClaveHuella                          32 bytes aleatorios (Base64)
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
$proyecto = Join-Path $raiz 'Back/Universidad.Mectronics.Seguridad/Api.Seguridad/Api.Seguridad.csproj'

function Guardar-Secreto([string] $Nombre, [string] $Valor) {
    dotnet user-secrets set $Nombre $Valor --project $proyecto | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "No se pudo guardar el secreto '$Nombre'."
    }
    Write-Host "  ✔ $Nombre"
}

function Obtener-BytesAleatoriosBase64([int] $Cantidad) {
    return [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes($Cantidad))
}

Write-Host 'Configurando los secretos de desarrollo de Api.Seguridad...'
Guardar-Secreto 'ConnectionStrings:UniversidadMectronics' $CadenaConexion

$rsa = [System.Security.Cryptography.RSA]::Create(2048)
try {
    Guardar-Secreto 'Jwt:ClavePrivadaAcceso' ([Convert]::ToBase64String($rsa.ExportPkcs8PrivateKey()))
}
finally {
    $rsa.Dispose()
}

Guardar-Secreto 'Jwt:ClaveRenovacion' (Obtener-BytesAleatoriosBase64 32)
Guardar-Secreto 'Jwt:ClaveHuella' (Obtener-BytesAleatoriosBase64 32)

Write-Host ''
Write-Host 'Secretos configurados (los valores se ocultan):'
dotnet user-secrets list --project $proyecto | ForEach-Object { $_ -replace ' = .*', ' = ********' }
