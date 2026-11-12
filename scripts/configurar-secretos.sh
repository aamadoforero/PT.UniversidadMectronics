#!/usr/bin/env bash
# =============================================================================
# Configura los secretos de desarrollo de los microservicios de Seguridad, Académico
# y Logs
# con "dotnet user-secrets" (Linux, macOS o Git Bash en Windows).
#
# Uso:
#   ./scripts/configurar-secretos.sh '<cadena de conexión de SQL Server>'
#
# Guarda, fuera del repositorio:
#   Api.Seguridad
#     ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
#     Jwt:ClavePrivadaAcceso                   clave privada RSA 2048 (PKCS#8, Base64)
#     Jwt:ClaveRenovacion                      32 bytes aleatorios (Base64)
#     Jwt:ClaveHuella                          32 bytes aleatorios (Base64)
#     Logs:ClaveAplicacion                     clave de Seguridad ante el microservicio de Logs
#   Api.Academico
#     ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
#     Logs:ClaveAplicacion                     clave de Académico ante el microservicio de Logs
#   Api.Logs
#     ConnectionStrings:UniversidadMectronics  cadena de conexión recibida
#     ClavesAplicacion:Servicio-Seguridad      la misma clave de Seguridad
#     ClavesAplicacion:Servicio-Academico      la misma clave de Académico
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
PROYECTO_SEGURIDAD="$RAIZ/Back/Universidad.Mectronics.Seguridad/Api.Seguridad/Api.Seguridad.csproj"
PROYECTO_ACADEMICO="$RAIZ/Back/Universidad.Mectronics.Academico/Api.Academico/Api.Academico.csproj"
PROYECTO_LOGS="$RAIZ/Back/Universidad.Mectronics.Logs/Api.Logs/Api.Logs.csproj"

guardar_secreto() {
    dotnet user-secrets set "$2" "$3" --project "$1" >/dev/null
    echo "  - $2"
}

# Las claves de Logs van en hexadecimal porque viajan en una cabecera HTTP.
CLAVE_LOGS_SEGURIDAD="$(openssl rand -hex 32)"
CLAVE_LOGS_ACADEMICO="$(openssl rand -hex 32)"

echo "Configurando los secretos de desarrollo de Api.Seguridad..."
guardar_secreto "$PROYECTO_SEGURIDAD" "ConnectionStrings:UniversidadMectronics" "$1"
guardar_secreto "$PROYECTO_SEGURIDAD" "Jwt:ClavePrivadaAcceso" \
    "$(openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048 2>/dev/null \
        | openssl pkcs8 -topk8 -nocrypt -outform DER | base64 | tr -d '\n')"
guardar_secreto "$PROYECTO_SEGURIDAD" "Jwt:ClaveRenovacion" "$(openssl rand -base64 32)"
guardar_secreto "$PROYECTO_SEGURIDAD" "Jwt:ClaveHuella" "$(openssl rand -base64 32)"
guardar_secreto "$PROYECTO_SEGURIDAD" "Logs:ClaveAplicacion" "$CLAVE_LOGS_SEGURIDAD"

echo
echo "Configurando los secretos de desarrollo de Api.Academico..."
guardar_secreto "$PROYECTO_ACADEMICO" "ConnectionStrings:UniversidadMectronics" "$1"
guardar_secreto "$PROYECTO_ACADEMICO" "Logs:ClaveAplicacion" "$CLAVE_LOGS_ACADEMICO"

echo
echo "Configurando los secretos de desarrollo de Api.Logs..."
guardar_secreto "$PROYECTO_LOGS" "ConnectionStrings:UniversidadMectronics" "$1"
guardar_secreto "$PROYECTO_LOGS" "ClavesAplicacion:Servicio-Seguridad" "$CLAVE_LOGS_SEGURIDAD"
guardar_secreto "$PROYECTO_LOGS" "ClavesAplicacion:Servicio-Academico" "$CLAVE_LOGS_ACADEMICO"

echo
echo "Secretos configurados (los valores se ocultan):"
for proyecto in "$PROYECTO_SEGURIDAD" "$PROYECTO_ACADEMICO" "$PROYECTO_LOGS"; do
    echo "  $(basename "$proyecto" .csproj)"
    dotnet user-secrets list --project "$proyecto" | sed -E 's/ = .*/ = ********/; s/^/    /'
done
