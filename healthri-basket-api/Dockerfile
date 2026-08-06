FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
COPY ["healthri-basket-api.csproj", "/src/healthri-basket-api/"]

WORKDIR "/src/healthri-basket-api"
RUN dotnet restore "healthri-basket-api.csproj"
COPY . .
RUN dotnet build "healthri-basket-api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "healthri-basket-api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "healthri-basket-api.dll"]