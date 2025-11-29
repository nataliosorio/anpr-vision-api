# Manual de Instalación y Despliegue - ANPR Vision API

Este manual proporciona instrucciones completas para clonar, instalar, configurar y ejecutar el proyecto **anpr-vision-api**, una API REST desarrollada en .NET 9.0 para el sistema de reconocimiento automático de placas vehiculares (ANPR).

## 📋 Tabla de Contenidos

- [Requisitos Previos](#requisitos-previos)
- [Instalación y Configuración](#instalación-y-configuración)
- [Ejecución Local](#ejecución-local)
- [Despliegue con Docker](#despliegue-con-docker)
- [CI/CD con Jenkins](#cicd-con-jenkins)
- [Documentación de la API](#documentación-de-la-api)
- [Casos de Error Comunes](#casos-de-error-comunes)
- [Recomendaciones y Mejores Prácticas](#recomendaciones-y-mejores-prácticas)
- [Soporte](#soporte)

## 🔧 Requisitos Previos

### Software Obligatorio

1. **.NET SDK** (versión 9.0 o superior)
   - Descárgalo desde: https://dotnet.microsoft.com/download
   - Verifica la instalación: `dotnet --version`

2. **Docker** (versión 20.10 o superior)
   - Descárgalo desde: https://www.docker.com/get-started
   - Verifica la instalación: `docker --version`
   - Docker Compose incluido

3. **Git**
   - Descárgalo desde: https://git-scm.com/
   - Verifica la instalación: `git --version`

### Software Opcional

4. **Jenkins** (para CI/CD)
   - Descárgalo desde: https://www.jenkins.io/
   - Solo necesario para despliegues automatizados

5. **Visual Studio 2022** o **VS Code** (para desarrollo)
   - Recomendado para desarrollo local

### Dependencias Externas

- **PostgreSQL** (base de datos)
  - Para entornos locales: se incluye en contenedor Docker
  - Para producción: AWS RDS PostgreSQL

- **Kafka** (sistema de mensajería)
  - Para entornos locales: debe estar disponible en la red
  - Para producción: cluster Kafka en AWS

- **Nginx** (proxy reverso en producción)
  - Solo para entorno de producción

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
# Clona el repositorio
git clone https://github.com/nataliosorio/anpr-vision-api.git

# Navega al directorio del proyecto
cd anpr-vision-api

# Verifica que estés en la rama correcta (generalmente main o master)
git branch -a
```

### 2. Configurar el Entorno

El proyecto soporta múltiples entornos: `develop`, `qa`, `staging`, y `prod`.

```bash
# Navega al directorio de la API
cd Api

# Elige el entorno modificando el archivo .env
# Por defecto está configurado para 'prod'
# Cambia según necesites: develop, qa, staging, prod
echo "ENVIRONMENT=develop" > .env
```

### 3. Restaurar Dependencias

```bash
# Desde el directorio Api/
dotnet restore

# Verifica que todas las dependencias se instalaron correctamente
dotnet list package
```

### 4. Configurar Variables de Entorno

Cada entorno tiene su propio archivo de configuración en `Api/DevOps/{entorno}/.env`.

**Para desarrollo local:**
- Copia y modifica `Api/DevOps/develop/.env`
- Asegúrate de que las variables de PostgreSQL apunten a tu instancia local

**Variables críticas:**
- `POSTGRES_HOST`: Host de la base de datos
- `POSTGRES_PORT`: Puerto (5432 por defecto)
- `POSTGRES_USER`: Usuario de BD
- `POSTGRES_PASSWORD`: Contraseña
- `POSTGRES_DB`: Nombre de la base de datos
- `Kafka__BootstrapServers`: URL del servidor Kafka

### 5. Configurar la Base de Datos

**Opción A: Usando Docker (Recomendado para desarrollo)**

```bash
# Desde la raíz del proyecto
docker compose -f ANPR-DB/docker-compose.yml up -d anprvision-postgres-develop

# Verifica que el contenedor esté corriendo
docker ps | grep postgres
```

**Opción B: Base de datos externa**

Asegúrate de tener PostgreSQL corriendo y configura las variables en el archivo `.env` correspondiente.

## 🏃‍♂️ Ejecución Local

### Ejecutar la API

```bash
# Desde el directorio Api/
dotnet run --project Web/Web.csproj

# O usando el comando simplificado
dotnet run
```

La API estará disponible en:
- **Desarrollo**: http://localhost:5100
- **QA**: http://localhost:5200
- **Staging**: http://localhost:5300
- **Producción**: http://localhost:8080 (o a través de Nginx en prod)

### Verificar el Estado

```bash
# Health check básico
curl http://localhost:5100/health

# O abre en el navegador
# http://localhost:5100/swagger
```

## 🐳 Despliegue con Docker

### Despliegue Local Completo

```bash
# 1. Crear las redes Docker necesarias
docker network create anpr-net-develop
docker network create anpr-net-qa
docker network create anpr-net-staging
docker network create anpr-net-prod

# 2. Levantar la base de datos
cd ANPR-DB
docker compose up -d anprvision-postgres-develop
cd ..

# 3. Construir y ejecutar la API
cd Api
docker compose -f DevOps/develop/docker-compose.yml --env-file DevOps/develop/.env up -d --build

# 4. Verificar que todo esté corriendo
docker ps
```

### Despliegue por Entorno

**Desarrollo:**
```bash
cd Api
docker compose -f DevOps/develop/docker-compose.yml --env-file DevOps/develop/.env up -d --build
```

**QA:**
```bash
cd Api
docker compose -f DevOps/qa/docker-compose.yml --env-file DevOps/qa/.env up -d --build
```

**Staging:**
```bash
cd Api
docker compose -f DevOps/staging/docker-compose.yml --env-file DevOps/staging/.env up -d --build
```

**Producción:**
```bash
cd Api
docker compose -f DevOps/prod/docker-compose.yml --env-file DevOps/prod/.env up -d --build
```

### Monitoreo de Contenedores

```bash
# Ver logs en tiempo real
docker logs -f anprvision-api-develop

# Ver estado de todos los contenedores
docker ps -a

# Ver uso de recursos
docker stats
```

## 🔄 CI/CD con Jenkins

### Configuración Inicial

1. **Instalar plugins necesarios en Jenkins:**
   - Docker Pipeline
   - Git
   - SSH Agent

2. **Configurar credenciales:**
   - AWS SSH Key (para despliegue en producción)
   - Git credentials

3. **Configurar el pipeline:**
   - El `Jenkinsfile` está incluido en la raíz del proyecto
   - Configura el webhook de GitHub para triggers automáticos

### Despliegue Automático

```bash
# El pipeline se ejecuta automáticamente en:
# - Push a ramas principales
# - Pull requests
# - Tags de release

# Para ejecutar manualmente desde Jenkins:
# 1. Abre el job correspondiente
# 2. Haz clic en "Build with Parameters"
# 3. Selecciona el entorno (develop, qa, staging, prod)
```

### Variables de Jenkins

Asegúrate de configurar estas variables en Jenkins:
- `aws_ssh_key`: Clave privada para SSH a AWS
- `aws_prod_ip`: IP del servidor de producción
- Credenciales de Git

## 📚 Documentación de la API

### Swagger UI

Una vez que la API esté corriendo, accede a la documentación interactiva:

```
http://localhost:{puerto}/swagger
```

### Endpoints Principales

- `GET /health` - Health check
- `POST /api/auth/login` - Autenticación
- `GET /api/vehicles` - Lista de vehículos
- `POST /api/vehicles/validate` - Validación de placas

### Autenticación

La API utiliza JWT Bearer tokens. Obtén un token mediante:

```bash
curl -X POST http://localhost:5100/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "password"}'
```

## 🚨 Casos de Error Comunes

### Error: "No se puede conectar a PostgreSQL"

**Síntomas:**
```
Npgsql.NpgsqlException: Failed to connect to database
```

**Soluciones:**
1. Verifica que PostgreSQL esté corriendo: `docker ps | grep postgres`
2. Revisa las variables de conexión en `.env`
3. Para desarrollo: `docker compose -f ANPR-DB/docker-compose.yml up -d`
4. Verifica la red Docker: `docker network ls`

### Error: "Puerto ya en uso"

**Síntomas:**
```
System.Net.Sockets.SocketException: Address already in use
```

**Soluciones:**
1. Cambia el puerto en `docker-compose.yml`
2. Mata procesos usando el puerto: `netstat -ano | findstr :5100`
3. `taskkill /PID <PID> /F` (Windows) o `kill -9 <PID>` (Linux)

### Error: "No se puede acceder a Kafka"

**Síntomas:**
```
Confluent.Kafka.KafkaException: Broker transport failure
```

**Soluciones:**
1. Verifica que Kafka esté disponible en la URL configurada
2. Para desarrollo local, configura un broker Kafka
3. Revisa las variables `Kafka__BootstrapServers` en `.env`

### Error: "Falta la red Docker"

**Síntomas:**
```
ERROR: Network anpr-net-develop not found
```

**Soluciones:**
```bash
# Crear la red manualmente
docker network create anpr-net-develop

# O ejecutar el script de inicialización
./scripts/init-networks.sh
```

### Error: "Permisos insuficientes en Linux"

**Síntomas:**
```
Permission denied
```

**Soluciones:**
```bash
# Otorgar permisos al workspace
sudo chmod -R 777 $WORKSPACE

# O agregar usuario al grupo docker
sudo usermod -aG docker $USER
```

## 💡 Recomendaciones y Mejores Prácticas

### Desarrollo

1. **Usa siempre contenedores para desarrollo** - Asegura consistencia entre entornos
2. **Configura hot-reload** - Para desarrollo ágil: `dotnet watch run`
3. **Usa User Secrets** - Para credenciales sensibles en desarrollo local
4. **Implementa logging estructurado** - Usa Serilog configurado en el proyecto

### Producción

1. **Usa HTTPS siempre** - Configura SSL/TLS en Nginx
2. **Implementa rate limiting** - Protege contra ataques DoS
3. **Configura monitoring** - Usa herramientas como Prometheus + Grafana
4. **Backups automáticos** - De base de datos y configuraciones
5. **Logs centralizados** - Usa ELK stack o similar

### Seguridad

1. **Cambia contraseñas por defecto** - Especialmente en producción
2. **Usa secrets management** - AWS Secrets Manager o similar
3. **Implementa CORS correctamente** - Solo permite orígenes confiables
4. **Auditoría de logs** - Monitorea accesos y cambios

### Performance

1. **Configura connection pooling** - Para PostgreSQL
2. **Implementa caching** - Redis para datos frecuentemente accedidos
3. **Optimiza queries** - Usa índices apropiados en BD
4. **Configura horizontal scaling** - Para alta disponibilidad

## 🆘 Soporte

### Recursos Adicionales

- **Repositorio**: https://github.com/nataliosorio/anpr-vision-api
- **Documentación técnica**: Ver carpeta `docs/`
- **Issues**: Reporta bugs en GitHub Issues

### Contacto

Para soporte técnico:
- Email: soporte@anpr-vision.com
- Slack: #anpr-backend

---

**Última actualización**: Noviembre 2025
**Versión**: 1.0.0
