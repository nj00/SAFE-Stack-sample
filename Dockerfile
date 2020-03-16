# FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster-slim
COPY /deploy /
WORKDIR /Server
EXPOSE 8085
ENTRYPOINT [ "dotnet", "Server.dll" ]
