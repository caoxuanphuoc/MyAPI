# syntax=docker/dockerfile:1

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers for better caching
COPY MyAPI/MyAPI.csproj MyAPI/
RUN dotnet restore MyAPI/MyAPI.csproj

# Copy everything else and publish
COPY MyAPI/ MyAPI/
RUN dotnet publish MyAPI/MyAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Use the default ASP.NET Core port mapping
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MyAPI.dll"]
