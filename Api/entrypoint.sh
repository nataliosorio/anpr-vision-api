#!/usr/bin/env bash
set -euo pipefail

MAX_RETRIES=${MAX_RETRIES:-12}
SLEEP=${SLEEP:-5}
DB_HOST=${DB_HOST:-localhost}
DB_PORT=${DB_PORT:-5432}

echo "Entrypoint: esperando BD en ${DB_HOST}:${DB_PORT} (max ${MAX_RETRIES} intentos)..."

i=0
until (echo > /dev/tcp/${DB_HOST}/${DB_PORT}) >/dev/null 2>&1; do
  i=$((i+1))
  echo "DB no lista (intento $i/$MAX_RETRIES). Esperando ${SLEEP}s..."
  if [ "$i" -ge "$MAX_RETRIES" ]; then
    echo "DB no disponible tras $MAX_RETRIES intentos. Continuando para permitir diagnóstico (la app puede fallar al iniciar)."
    break
  fi
  sleep "$SLEEP"
done

echo "Lanzando la aplicación (.NET)…"
exec dotnet Web.dll
