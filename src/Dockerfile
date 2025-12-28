FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Egzotech.Api/Egzotech.Api.csproj", "Egzotech.Api/"]
COPY ["Egzotech.Application/Egzotech.Application.csproj", "Egzotech.Application/"]
COPY ["Egzotech.Domain/Egzotech.Domain.csproj", "Egzotech.Domain/"]
COPY ["Egzotech.Infrastructure/Egzotech.Infrastructure.csproj", "Egzotech.Infrastructure/"]

RUN dotnet restore "Egzotech.Api/Egzotech.Api.csproj"

COPY . .

# Build
WORKDIR /src/Egzotech.Api
RUN dotnet build "Egzotech.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Egzotech.Api.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
ENV ASPNETCORE_URLS=http://+:5000

WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

EXPOSE 5000
ENTRYPOINT [ "dotnet", "Egzotech.Api.dll" ]