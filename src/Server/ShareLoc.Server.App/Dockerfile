#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Server/ShareLoc.Server.App/ShareLoc.Server.App.csproj", "Server/ShareLoc.Server.App/"]
COPY ["Shared/ShareLoc.Shared.Common/ShareLoc.Shared.Common.csproj", "Shared/ShareLoc.Shared.Common/"]
COPY ["Server/ShareLoc.Server.DAL/ShareLoc.Server.DAL.csproj", "Server/ShareLoc.Server.DAL/"]
RUN dotnet restore "Server/ShareLoc.Server.App/ShareLoc.Server.App.csproj"
COPY . .
WORKDIR "/src/Server/ShareLoc.Server.App"
RUN dotnet build "ShareLoc.Server.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShareLoc.Server.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShareLoc.Server.App.dll"]
