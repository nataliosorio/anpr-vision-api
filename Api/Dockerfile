# ---------- build stage ----------
FROM ubuntu:22.04 AS build

# Instalar dependencias básicas y .NET SDK 9.0
RUN apt-get update && apt-get install -y wget apt-transport-https software-properties-common \
    && wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update && apt-get install -y dotnet-sdk-9.0

WORKDIR /src

# Copiar la solución principal
COPY SecurityModel.sln ./

# Copiar los proyectos (cada .csproj debe existir en su respectiva carpeta)
COPY Web/Web.csproj Web/
COPY Business/Business.csproj Business/
COPY Data/Data.csproj Data/
COPY Entity/Entity.csproj Entity/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Utilities/Utilities.csproj Utilities/
COPY Diagram/Diagram.csproj Diagram/

# Restaurar dependencias del proyecto principal
RUN dotnet restore Web/Web.csproj

# Copiar el resto del código fuente
COPY . .

# Compilar y publicar el proyecto principal en modo Release
RUN dotnet publish Web/Web.csproj -c Release -o /app/publish

# ---------- runtime stage ----------
FROM ubuntu:22.04 AS final

# Instalar solo el runtime de .NET 9.0
RUN apt-get update && apt-get install -y wget apt-transport-https software-properties-common \
    && wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update && apt-get install -y aspnetcore-runtime-9.0

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 DOTNET_RUNNING_IN_CONTAINER=true

# Copiar los archivos publicados desde la fase anterior
COPY --from=build /app/publish .

# Exponer el puerto para el contenedor
EXPOSE 8080

# Punto de entrada
ENTRYPOINT ["dotnet", "Web.dll"]
