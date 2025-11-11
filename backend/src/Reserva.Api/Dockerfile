# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copiar los proyectos y restaurar
COPY Reserva.Core/Reserva.Core.csproj Reserva.Core/
COPY Reserva.Infrastructure/Reserva.Infrastructure.csproj Reserva.Infrastructure/
COPY Reserva.Api/Reserva.Api.csproj Reserva.Api/

RUN dotnet restore Reserva.Api/Reserva.Api.csproj

# Copiar todo el código
COPY . .

# Compilar la aplicación en Release
RUN dotnet build Reserva.Api/Reserva.Api.csproj -c Release -o /app/build

# Publicar la aplicación
RUN dotnet publish Reserva.Api/Reserva.Api.csproj -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Copiar la publicación desde la etapa de build
COPY --from=build /app/publish .

# Configurar punto de entrada
ENTRYPOINT ["dotnet", "Reserva.Api.dll"]

