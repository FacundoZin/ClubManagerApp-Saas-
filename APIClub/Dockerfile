# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["APIClub.csproj", "./"]
RUN dotnet restore "APIClub.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "APIClub.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "APIClub.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIClub.dll"]
