FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. 프로젝트 파일 복사
COPY ["GeoAsset.Api/GeoAsset.Api.csproj", "GeoAsset.Api/"]
RUN dotnet restore "GeoAsset.Api/GeoAsset.Api.csproj"

# 2. 전체 소스 복사
COPY . .

# 3. publish 실행 (단순하게 경로만 지정)
WORKDIR "/src/GeoAsset.Api"
RUN dotnet publish "GeoAsset.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore
    
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GeoAsset.Api.dll"]