#!/bin/sh
# Genera, solo la primera vez, las claves JWT y de Logs en el volumen /secretos.
set -eu

if [ -f /secretos/listo ]; then
    echo "Los secretos ya existen."
    exit 0
fi

apk add --no-cache openssl >/dev/null

CLAVE_LOGS_SEGURIDAD="$(openssl rand -hex 32)"
CLAVE_LOGS_ACADEMICO="$(openssl rand -hex 32)"
CLAVE_PRIVADA="$(openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:2048 2>/dev/null \
    | openssl pkcs8 -topk8 -nocrypt -outform DER | base64 | tr -d '\n')"

cat > /secretos/seguridad.env <<FIN
Jwt__ClavePrivadaAcceso=$CLAVE_PRIVADA
Jwt__ClaveRenovacion=$(openssl rand -base64 32)
Jwt__ClaveHuella=$(openssl rand -base64 32)
Logs__ClaveAplicacion=$CLAVE_LOGS_SEGURIDAD
FIN

cat > /secretos/academico.env <<FIN
Logs__ClaveAplicacion=$CLAVE_LOGS_ACADEMICO
FIN

cat > /secretos/logs.env <<FIN
ClavesAplicacion__Servicio-Seguridad=$CLAVE_LOGS_SEGURIDAD
ClavesAplicacion__Servicio-Academico=$CLAVE_LOGS_ACADEMICO
FIN

touch /secretos/listo
echo "Secretos generados."
