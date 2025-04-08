FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Auth/AuthServer/AuthServer.csproj", "Auth/AuthServer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Auth/AuthServer.Migrations.MySql/AuthServer.Migrations.MySql.csproj", "Auth/AuthServer.Migrations.MySql/"]
COPY ["Auth/AuthServer.Migrations.Sqlite/AuthServer.Migrations.Sqlite.csproj", "Auth/AuthServer.Migrations.Sqlite/"]
RUN dotnet restore "Auth/AuthServer/AuthServer.csproj"
COPY . .
WORKDIR "/src/Auth/AuthServer"
RUN dotnet build "AuthServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthServer.dll"]
