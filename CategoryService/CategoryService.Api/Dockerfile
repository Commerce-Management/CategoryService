FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CategoryService.Api/CategoryService.Api.csproj", "CategoryService.Api/"]
COPY ["CategoryService.Application/CategoryService.Application.csproj", "CategoryService.Application/"]
COPY ["CategoryService.Core/CategoryService.Core.csproj", "CategoryService.Core/"]
COPY ["CategoryService.Shared/CategoryService.Shared.csproj", "CategoryService.Shared/"]
COPY ["CategoryService.Infrastructure/CategoryService.Infrastructure.csproj", "CategoryService.Infrastructure/"]
RUN dotnet restore "CategoryService.Api/CategoryService.Api.csproj"
COPY . .
WORKDIR "/src/CategoryService.Api"
RUN dotnet build "CategoryService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CategoryService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CategoryService.Api.dll"]
