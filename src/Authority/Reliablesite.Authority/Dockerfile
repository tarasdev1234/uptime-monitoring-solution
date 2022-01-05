FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
COPY .build/publish/Reliablesite.Authority/Release/netcoreapp3.1/ app/
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "Reliablesite.Authority.dll"]