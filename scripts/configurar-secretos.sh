#!/usr/bin/env bash
# =============================================================================
# Configura los secretos de desarrollo del servicio de Seguridad con
# "dotnet user-secrets" (Linux, macOS o Git Bash en Windows).
#
# Uso:
#   ./scripts/configurar-secretos.sh '<cadena de conexión de SQL Server>'
#
# Guarda, fuera del repositorio:
#   ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
#   Jwt:ClavePrivadaAcceso                   clave privada RSA 2048 (PKCS#8, Base64)
#   Jwt:ClaveRenovacion                      32 bytes aleatorios (Base64)
#   Jwt:ClaveHuella                          32 bytes aleatorios (Base64)
#
# Volver a ejecutarlo genera claves nuevas: las sesiones abiertas dejan de valer.
# Detalle completo: README.md (sección Secretos)
# =============================================================================
set -euo pipefail

if [[ $# -ne 1 || -z "$1" ]]; then
    echo "Uso: $0 '<cadena de conexión de SQL Server>'" >&2
    exit 1
fi

for herramienta in dotnet openssl base64; do
    if ! command -v "$herramienta" >/dev/null 2>&1; then
        echo "Error: no se encontró '$herramienta'. Instálelo y vuelva a intentarlo." >&2
        exit 1
    fi
done

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROYECTO="$RAIZ/Back/Universidad.Mectronics.Seguridad/Api.Seguridad/Api.Seguridad.csproj"

guardar_secreto() {
    dotnet user-secrets set "$1" "$2" --project "$PROYECTO" >/dev/null
    echo "  ✔ $1"
}

echo "Configurando los secretos de desarrollo de Api.Seguridad..."
guardar_secreto "ConnectionStrings:UniversidadMectronics" "$1"
# "openssl pkcs8 -topk8" garantiza el formato PKCS#8 en cualquier versión de OpenSSL (algunas entregan PKCS#1 en DER).
# "base64 | tr -d '\n'" deja la clave en una sola línea y funciona igual en Linux y macOS.
guardar_secreto "Jwt:ClavePrivadaAcceso" \
    "$(openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048 2>/dev/null \
        | openssl pkcs8 -topk8 -nocrypt -outform DER | base64 | tr -d '\n')"
guardar_secreto "Jwt:ClaveRenovacion" "$(openssl rand -base64 32)"
guardar_secreto "Jwt:ClaveHuella" "$(openssl rand -base64 32)"

echo
echo "Secretos configurados (los valores se ocultan):"
dotnet user-secrets list --project "$PROYECTO" | sed -E 's/ = .*/ = ********/'
