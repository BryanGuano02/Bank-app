# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto para restaurar dependencias de forma cacheable
COPY ["Fast-Bank.sln", "./"]
COPY ["src/API/Fast-Bank.API.csproj", "src/API/"]
COPY ["src/Application/Fast-Bank.Application.csproj", "src/Application/"]
COPY ["src/Domain/Fast-Bank.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Fast-Bank.Infrastructure.csproj", "src/Infrastructure/"]

# Restaurar dependencias solo para el proyecto de API
RUN dotnet restore "src/API/Fast-Bank.API.csproj"

# Copiar el resto del código fuente
COPY . .

# Publicar la aplicación desde la carpeta correcta
WORKDIR "/src/src/API"
RUN dotnet publish "Fast-Bank.API.csproj" -c Release -o /app/publish --no-restore


# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar los archivos publicados
COPY --from=build /app/publish .

# Crear directorio para la base de datos SQLite
RUN mkdir -p /app/data

# Establecer el puerto como variable de entorno
ENV ASPNETCORE_URLS=http://+:8082
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8082

# Punto de entrada
ENTRYPOINT ["dotnet", "Fast-Bank.API.dll"]
