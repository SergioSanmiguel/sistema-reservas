# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["src/Reserva.Api/Reserva.Api.csproj", "Reserva.Api/"]
RUN dotnet restore "Reserva.Api/Reserva.Api.csproj"

# Copiar todo el código fuente
COPY . .
WORKDIR "/src/Reserva.Api"
RUN dotnet publish "Reserva.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final: imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Reserva.Api.dll"]


