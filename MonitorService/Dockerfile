FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["MonitorService/MonitorService.csproj", "MonitorService/"]
RUN dotnet restore "MonitorService/MonitorService.csproj"
COPY . .
WORKDIR "/src/MonitorService"
RUN dotnet build "MonitorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MonitorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MonitorService.dll"]