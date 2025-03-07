FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BuildingBlocks/","BuildingBlocks/"]
COPY ["src/Services/EShop.RiskEvaluator/*.csproj","Services/EShop.RiskEvaluator/"]
WORKDIR /src/Services/EShop.RiskEvaluator
RUN dotnet restore
COPY ["src/Services/EShop.RiskEvaluator/","./"]

RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EShop.RiskEvaluator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT docker
ENV ASPNETCORE_URLS=http://+:80  
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShop.RiskEvaluator.dll"]