# =========================
# 🧩 Etapa 1: Build y Migrations
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar proyecto y restaurar dependencias
COPY ["Reserva.Api/Reserva.Api.csproj", "Reserva.Api/"]
RUN dotnet restore "Reserva.Api/Reserva.Api.csproj"

# Copiar todo el código
COPY . .

# Instalar dotnet-ef en esta etapa
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Instalar pg_isready para migraciones
RUN apt-get update && apt-get install -y postgresql-client libpq-dev && rm -rf /var/lib/apt/lists/*

# Publicar la app
RUN dotnet publish "Reserva.Api/Reserva.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# 🚀 Etapa 2: Runtime (solo app)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Instalar cliente postgres para pg_isready
RUN apt-get update && apt-get install -y postgresql-client libpq-dev && rm -rf /var/lib/apt/lists/*

# Copiar la app publicada (sin SDK)
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Reserva.Api.dll"]
