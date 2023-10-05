# Используйте образ с .NET 7.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Копирование проекта и восстановление зависимостей
COPY *.csproj ./
RUN dotnet restore

# Копирование остальных файлов и сборка приложения
COPY . ./
RUN dotnet publish -c Release -o out

# Этап для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Task3.dll"]