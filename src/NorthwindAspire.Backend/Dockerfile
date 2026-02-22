# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj", "NorthwindAspire.Backend/"]
COPY ["NorthwindAspire.ServiceDefaults/NorthwindAspire.ServiceDefaults.csproj", "NorthwindAspire.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "NorthwindAspire.Backend/NorthwindAspire.Backend.csproj"

# Copy source code
COPY . .

# Build application
RUN dotnet build "src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8443

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "NorthwindAspire.Backend.dll"]
