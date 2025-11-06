# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar primero solo archivos de proyecto para aprovechar cache del restore
COPY SecurityModel.sln ./
COPY Web/Web.csproj Web/
COPY Business/Business.csproj Business/
COPY Data/Data.csproj Data/
COPY Entity/Entity.csproj Entity/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Utilities/Utilities.csproj Utilities/
COPY Diagram/Diagram.csproj Diagram/

# Restaurar dependencias (esto se cachea)
RUN dotnet restore Web/Web.csproj

# Luego copiar el resto del código
COPY . .

# Compilar y publicar
RUN dotnet publish Web/Web.csproj -c Release -o /app/publish

# ---------- runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "Web.dll"]
