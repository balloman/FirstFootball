﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["FirstFootball.Backend.csproj", "./"]
RUN dotnet restore "FirstFootball.Backend.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "FirstFootball.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FirstFootball.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FirstFootball.Backend.dll"]
