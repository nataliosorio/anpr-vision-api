pipeline {
    agent any

    environment {
        DOTNET_CLI_HOME = 'C:\\jenkins\\.dotnet'
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {

        // =======================================================
        // 1️⃣ Leer entorno desde Api/.env
        // =======================================================
        stage('Leer entorno desde Api/.env') {
            steps {
                dir('Api') {
                    script {
                        def envValue = powershell(
                            script: "(Get-Content .env | Where-Object { \$_ -match '^ENVIRONMENT=' }) -replace '^ENVIRONMENT=', ''",
                            returnStdout: true
                        ).trim()

                        if (!envValue) {
                            error "❌ No se encontró ENVIRONMENT en Api/.env"
                        }

                        env.ENVIRONMENT = envValue
                        env.ENV_DIR = "DevOps/${env.ENVIRONMENT}"
                        env.COMPOSE_FILE = "${env.ENV_DIR}/docker-compose.yml"
                        env.ENV_FILE = "${env.ENV_DIR}/.env"
                        env.DB_COMPOSE_FILE = "../ANPR-DB/docker-compose.yml"

                        echo """
                        ✅ Entorno detectado: ${env.ENVIRONMENT}
                        📄 Compose API: ${env.COMPOSE_FILE}
                        📁 Env file: ${env.ENV_FILE}
                        🗄️ Compose DB: ${env.DB_COMPOSE_FILE}
                        """
                    }
                }
            }
        }

        // =======================================================
        // 2️⃣ Restaurar dependencias
        // =======================================================
        stage('Restaurar dependencias') {
            steps {
                dir('Api') {
                    bat '''
                        echo 🔧 Restaurando dependencias .NET...
                        if not exist "C:\\jenkins\\dotnet" mkdir "C:\\jenkins\\dotnet"

                        echo 🧹 Limpiando caché NuGet...
                        dotnet nuget locals all --clear

                        echo 🚀 Ejecutando restore (sin procesos paralelos)...
                        dotnet restore Web\\Web.csproj --disable-parallel
                    '''
                }
            }
        }

        // =======================================================
        // 3️⃣ Compilar proyecto
        // =======================================================
        stage('Compilar proyecto') {
            steps {
                dir('Api') {
                    echo '⚙️ Compilando la solución ANPR Vision...'
                    bat 'dotnet build Web\\Web.csproj --configuration Release --no-restore'
                }
            }
        }

        // =======================================================
        // 4️⃣ Publicar y construir imagen Docker
        // =======================================================
        stage('Publicar y construir imagen Docker') {
            steps {
                dir('Api') {
                    echo "🐳 Construyendo imagen Docker para ANPR Vision (${env.ENVIRONMENT})"
                    bat "docker build -t anpr-vision-${env.ENVIRONMENT}:latest -f Dockerfile ."
                }
            }
        }

        // =======================================================
        // 5️⃣ Preparar red y base de datos
        // =======================================================
        stage('Preparar red y base de datos') {
            steps {
                dir('Api') {
                    bat """
                        echo 🌐 Creando red externa compartida (si no existe)...
                        docker network create anprvision_network || echo "red existente"

                        echo 🗄️ Levantando stack de bases de datos...
                        docker compose -f ${env.DB_COMPOSE_FILE} up -d
                    """
                }
            }
        }

        // =======================================================
        // 6️⃣ Desplegar API
        // =======================================================
        stage('Desplegar API') {
            steps {
                dir('Api') {
                    echo "🚀 Desplegando ANPR Vision para entorno: ${env.ENVIRONMENT}"
                    bat "docker compose -f ${env.COMPOSE_FILE} --env-file ${env.ENV_FILE} up -d --build"
                }
            }
        }
    }

    post {
        success {
            echo "🎉 Despliegue completado correctamente para ${env.ENVIRONMENT}"
        }
        failure {
            echo "💥 Error durante el despliegue en ${env.ENVIRONMENT}"
        }
    }
}
