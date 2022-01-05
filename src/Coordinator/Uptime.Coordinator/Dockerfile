FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
COPY .build/bin/Uptime.Coordinator/Release/netcoreapp3.1/publish/ app/
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "Uptime.Coordinator.dll"]