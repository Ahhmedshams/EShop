FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/EShop.User.Api/EShop.User.Api.csproj", "./"]
RUN dotnet restore "EShop.User.Api.csproj"
COPY src/Services/EShop.User.Api .
WORKDIR "/src"
RUN dotnet build "EShop.User.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EShop.User.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT docker
ENV ASPNETCORE_URLS=http://+:80  
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EShop.User.Api.dll"]