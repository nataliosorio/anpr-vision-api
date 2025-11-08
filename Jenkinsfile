pipeline {
    agent any
    environment {
        DOCKER_CLI_HINTS = "off"
        DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {
    
        stage('Permisos workspace') {
          steps {
            sh '''
              echo "🔧 Corrigiendo permisos del workspace..."
              sudo chmod -R 777 $WORKSPACE || chmod -R 777 $WORKSPACE
            '''
          }
        }


        // =====================================================
        // 1️⃣ Leer entorno desde Api/.env
        // =====================================================
        stage('Leer entorno desde Api/.env') {
            steps {
                dir('Api') {
                    sh '''
                        echo "📂 Leyendo entorno desde Api/.env..."

                        ENVIRONMENT=$(grep '^ENVIRONMENT=' .env | cut -d '=' -f2 | tr -d '\\r\\n')

                        if [ -z "$ENVIRONMENT" ]; then
                            echo "❌ No se encontró ENVIRONMENT en Api/.env"
                            exit 1
                        fi

                        echo "✅ Entorno detectado: $ENVIRONMENT"
                        echo "ENVIRONMENT=$ENVIRONMENT" > ../env.properties
                        echo "ENV_DIR=Api/DevOps/$ENVIRONMENT" >> ../env.properties
                        echo "COMPOSE_FILE=Api/DevOps/$ENVIRONMENT/docker-compose.yml" >> ../env.properties
                        echo "ENV_FILE=Api/DevOps/$ENVIRONMENT/.env" >> ../env.properties
                        echo "DB_COMPOSE_FILE=ANPR-DB/docker-compose.yml" >> ../env.properties
                    '''
                }

                script {
                    def props = readProperties file: 'env.properties'
                    env.ENVIRONMENT = props['ENVIRONMENT']
                    env.ENV_DIR = props['ENV_DIR']
                    env.COMPOSE_FILE = props['COMPOSE_FILE']
                    env.ENV_FILE = props['ENV_FILE']
                    env.DB_COMPOSE_FILE = props['DB_COMPOSE_FILE']

                    echo """
                    ✅ Entorno detectado: ${env.ENVIRONMENT}
                    📄 Compose API: ${env.COMPOSE_FILE}
                    📁 Env file: ${env.ENV_FILE}
                    🗄️ DB Compose: ${env.DB_COMPOSE_FILE}
                    """
                }
            }
        }


        // =====================================================
        // 4️⃣ Construir imagen Docker
        // =====================================================
        stage('Construir imagen Docker') {
            steps {
                dir('Api') {
                    sh '''
                        echo "🐳 Construyendo imagen Docker para ANPR Vision ($ENVIRONMENT)..."
                        docker image prune -f || true
                        COMMIT_HASH=$(git rev-parse --short HEAD)
                        docker build -t anprvision-api-$ENVIRONMENT:$COMMIT_HASH -t anprvision-api-$ENVIRONMENT:latest -f Dockerfile .
                    '''
                }
            }
        }

        // =====================================================
        // 5️⃣ Preparar red y base de datos
        // =====================================================
        stage('Preparar red y base de datos') {
            steps {
                script {
                    echo "🌐 Verificando red anpr-net-${env.ENVIRONMENT} ..."
                    sh "docker network create anpr-net-${env.ENVIRONMENT} || echo '✅ Red ya existe'"

                    if (env.ENVIRONMENT == 'develop' || env.ENVIRONMENT == 'qa' || env.ENVIRONMENT == 'staging') {
                        sh '''
                            echo "🗄️ Levantando stack local de base de datos para entorno $ENVIRONMENT..."
                            docker compose -f $DB_COMPOSE_FILE up -d anprvision-postgres-$ENVIRONMENT
                        '''
                    } else {
                        echo "🛑 Saltando base de datos (usa RDS en producción)"
                    }
                }
            }
        }

        // =====================================================
        // 6️⃣ Desplegar API
        // =====================================================
        stage('Desplegar API') {
            steps {
                    script {
                        if (env.ENVIRONMENT == 'prod') {
                            echo "🚀 Despliegue remoto en AWS (producción)"

                            withCredentials([
                                sshUserPrivateKey(credentialsId: 'aws_ssh_key', keyFileVariable: 'SSH_KEY'),
                                string(credentialsId: 'aws_prod_ip', variable: 'PROD_IP')
                            ]) {
                                sh '''
                                    echo "🌍 Conectando al servidor AWS en $PROD_IP"
                                    ssh -o StrictHostKeyChecking=no -i $SSH_KEY ubuntu@$PROD_IP "
                                        set -e
                                        echo '📦 Actualizando repositorio...'
                                        cd /srv/anprvision-backend || exit 1
                                        git pull

                                        echo '🐳 Desplegando stack Docker en red anpr-net-prod...'
                                        docker network create anpr-net-prod || echo 'Red ya existente'
                                        docker compose -f Api/DevOps/prod/docker-compose.yml --env-file Api/DevOps/prod/.env up -d --build --force-recreate --remove-orphans
                                    "
                                '''
                            }
                        } else {
                            echo "🚀 Despliegue local (${env.ENVIRONMENT})"
                            sh '''
                                docker compose -f $COMPOSE_FILE --env-file $ENV_FILE up -d --build --remove-orphans
                            '''
                        }
                    }
            }
        }
    }

    // =========================================================
    // Post actions
    // =========================================================
    post {
        success {
            echo "🎉 Despliegue completado correctamente para ${env.ENVIRONMENT}"
        }
        failure {
            echo "💥 Error durante el despliegue en ${env.ENVIRONMENT}"
        }
    }
}

