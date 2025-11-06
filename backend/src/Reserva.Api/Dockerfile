# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["Reserva.Api.csproj", "./"]
RUN dotnet restore "Reserva.Api.csproj"

# Copiar el resto del código fuente
COPY . .
RUN dotnet publish "Reserva.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Reserva.Api.dll"]



