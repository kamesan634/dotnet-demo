FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DotnetDemo/DotnetDemo.csproj", "DotnetDemo/"]
RUN dotnet restore "DotnetDemo/DotnetDemo.csproj"
COPY src/DotnetDemo/. DotnetDemo/
WORKDIR "/src/DotnetDemo"
RUN dotnet build "DotnetDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DotnetDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# 安裝 curl 供健康檢查使用
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "DotnetDemo.dll"]
