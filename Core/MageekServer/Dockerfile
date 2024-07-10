#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MageekServer/MageekServer.csproj", "MageekServer/"]#D:\PROJECTS\VS\Mageek\Core\MageekServer\MageekServer.csproj
COPY ["MageekCore/MageekCore.csproj", "MageekCore/"]#D:\PROJECTS\VS\Mageek\Core\MageekCore\MageekCore.csproj
COPY ["PlaziatTools/PlaziatCore.csproj", "PlaziatTools/"]#D:\PROJECTS\VS\Mageek\Core\PlaziatTools\PlaziatCore.csproj
COPY ["PlaziatIdentity/PlaziatIdentity.csproj", "PlaziatIdentity/"]#D:\PROJECTS\VS\MaGeek\Core\PlaziatIdentity\PlaziatIdentity.csproj
RUN dotnet restore "./MageekServer/./MageekServer.csproj"
COPY . .
WORKDIR /src/MageekServer
RUN dotnet build "./MageekServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MageekServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MageekServer.dll"]