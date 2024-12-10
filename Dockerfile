FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["hoba-backend/hoba-backend.csproj", "hoba-backend/"]
COPY ["Auth/Auth.csproj", "Auth/"]
COPY ["DB/DB.csproj", "DB/"]
RUN dotnet restore "hoba-backend/hoba-backend.csproj"
COPY . .
WORKDIR "/src/hoba-backend"
RUN dotnet build "hoba-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "hoba-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "hoba-backend.dll"]
