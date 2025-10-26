# ---- Build .NET 9 ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia .csproj por proyecto
COPY ./Entity/Entity.csproj ./Entity/
COPY ./Data/Data.csproj ./Data/
COPY ./Business/Business.csproj ./Business/
COPY ./Utilities/Utilities.csproj ./Utilities/
COPY ./Diagram/Diagram.csproj ./Diagram/
COPY ./Web/Web.csproj ./Web/

RUN dotnet restore ./Web/Web.csproj

# Copia el resto del código
COPY ./Entity ./Entity
COPY ./Data ./Data
COPY ./Business ./Business
COPY ./Utilities ./Utilities
COPY ./Diagram ./Diagram
COPY ./Web ./Web

RUN dotnet publish ./Web/Web.csproj -c Release -o /out

# ---- Runtime .NET 9 ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out/ ./

# Copiar el entrypoint (espera DB) y hacerlo ejecutable
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000

# Use el entrypoint en lugar de lanzar dotnet directamente
ENTRYPOINT ["/entrypoint.sh"]
