FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConversionSystemReportService/ConversionSystemReportService.csproj", "ConversionSystemReportService/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "ConversionSystemReportService/ConversionSystemReportService.csproj"
COPY . .
WORKDIR "/src/ConversionSystemReportService"
RUN dotnet build "ConversionSystemReportService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ConversionSystemReportService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 50051

ENTRYPOINT ["dotnet", "ConversionSystemReportService.dll"]