# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
WORKDIR /app
EXPOSE 7015
EXPOSE 5152


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CashFlow.Consolidated.Api/CashFlow.Consolidated.Api.csproj", "CashFlow.Consolidated.Api/"]
COPY ["CashFlow.Consolidated.Application/CashFlow.Consolidated.Application.csproj", "CashFlow.Consolidated.Application/"]
COPY ["CashFlow.Consolidated.Domain/CashFlow.Consolidated.Domain.csproj", "CashFlow.Consolidated.Domain/"]
COPY ["CashFlow.Consolidated.Infra.Data/CashFlow.Consolidated.Infra.Data.csproj", "CashFlow.Consolidated.Infra.Data/"]
RUN dotnet restore "./CashFlow.Consolidated.Api/CashFlow.Consolidated.Api.csproj"
COPY . .
WORKDIR "/src/CashFlow.Consolidated.Api"
RUN dotnet build "./CashFlow.Consolidated.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CashFlow.Consolidated.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CashFlow.Consolidated.Api.dll"]