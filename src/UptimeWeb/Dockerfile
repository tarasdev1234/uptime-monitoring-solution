FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
COPY .build/publish/UptimeWeb/Release/netcoreapp3.1/ app/
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "UptimeWeb.dll"]