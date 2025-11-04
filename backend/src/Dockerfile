# Imagen base para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["Reservas.Api/Reservas.Api.csproj", "Reservas.Api/"]
RUN dotnet restore "Reservas.Api/Reservas.Api.csproj"

# Copiar todo y compilar
COPY . .
WORKDIR /src/Reservas.Api
RUN dotnet publish -c Release -o /app/publish

# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Reservas.Api.dll"]
