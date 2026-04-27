# Docker Orchestration Implementation Guide

Complete implementation guide for Docker containerization and orchestration in NorthwindAspire projects.

---

## Files Created

1. **`src/NorthwindAspire.Backend/Dockerfile`**
   - Multi-stage .NET 10 build
   - Optimized production image
   - Health checks included

2. **`src/NorthwindAspire.Frontend/Dockerfile`**
   - Multi-stage .NET 10 build
   - Blazor Server runtime
   - Health checks included

3. **`docker-compose.yml`**
   - Production orchestration
   - Service networking
   - Health monitoring

4. **`docker-compose.dev.yml`**
   - Development orchestration
   - Hot-reload with `dotnet watch`
   - Volume mounts for source code

5. **`.dockerignore`**
   - Excludes unnecessary files from build

6. **`NorthwindAspire.AppHost/AppHost.cs`** (Updated)
   - Dual-mode support: Local projects or Docker containers
   - Environment variable detection for Docker mode

---

## ?? Quick Start Commands

### Production Deployment

```bash
# Build images
docker-compose build

# Start services
docker-compose up -d

# View status
docker-compose ps

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Development with Hot-Reload

```bash
# Start with live reload
docker-compose -f docker-compose.dev.yml up

# In another terminal, edit files in src/
# Changes automatically rebuild and reload
```

---

## ??? Architecture

```
???????????????????????????????
?   docker-compose.yml        ?
???????????????????????????????
?                             ?
?  ???????????????????????   ?
?  ? northwind-backend   ?   ?
?  ? Port: 5000:8080     ?   ?
?  ? Health: /health     ?   ?
?  ???????????????????????   ?
?           ?                ?
?           ? depends_on     ?
?           ?                ?
?  ???????????????????????   ?
?  ?northwind-frontend   ?   ?
?  ? Port: 3000:8080     ?   ?
?  ? Health: /health     ?   ?
?  ? API_URL: backend:80 ?   ?
?  ???????????????????????   ?
?                             ?
?  Network: northwind-network ?
???????????????????????????????
```

---

## ?? Configuration Details

### Backend Configuration

**Port Mapping:**
- Host: `5000:8080` (HTTP)
- Host: `5001:8443` (HTTPS)

**Environment Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080;https://+:8443
```

**Health Check:**
```
GET http://localhost:8080/health
Interval: 30 seconds
Timeout: 10 seconds
Retries: 3
```

### Frontend Configuration

**Port Mapping:**
- Host: `3000:8080` (HTTP)
- Host: `3001:8443` (HTTPS)

**Environment Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080;https://+:8443
API_URL=http://backend:8080
```

**Service Dependencies:**
- Depends on: `backend`
- Waits for: Backend health check to pass

---

## ?? Usage Scenarios

### Scenario 1: Local Development

**Approach:** Use Aspire + Docker Compose for development

```bash
# Run in one terminal
docker-compose -f docker-compose.dev.yml up

# This starts services with hot-reload
# Access at:
# - Frontend: http://localhost:3000
# - Backend: http://localhost:5000
```

**Benefits:**
- Full Aspire Dashboard integration
- Rich debugging capabilities
- Fast development cycle with hot-reload

### Scenario 2: Local Testing (Production-like)

**Approach:** Use Docker Compose for testing production setup

```bash
# Build production images
docker-compose build

# Start services
docker-compose up -d

# Test at:
# - Frontend: http://localhost:3000
# - Backend: http://localhost:5000
```

**Benefits:**
- Exact production environment
- Test containerization
- Verify health checks
- Test networking

### Scenario 3: CI/CD Pipeline Integration

**Approach:** Build and push images in Azure DevOps

```yaml
# In azure-pipelines.yml
- task: Docker@2
  inputs:
    command: build
    Dockerfile: 'src/NorthwindAspire.Backend/Dockerfile'
    tags: 'northwind-backend:$(Build.BuildId)'
    
- task: Docker@2
  inputs:
    command: push
    containerRegistry: 'myregistry'
    repository: 'northwind/backend'
    tags: '$(Build.BuildId)'
```

### Scenario 4: Production Deployment

**Approach:** Deploy to Azure Container Instances or App Service

```bash
# Push to Azure Container Registry
az acr build --registry myregistry \
  --image northwind-backend:latest \
  --file src/NorthwindAspire.Backend/Dockerfile .

# Deploy to App Service (Windows/Linux)
az webapp deployment container config \
  --name northwind-backend-prod \
  --resource-group northwind-rg \
  --docker-registry-server-url https://myregistry.azurecr.io
```

---

## ?? Integration with Aspire AppHost

The `AppHost.cs` now supports both modes:

### Mode 1: Local Development (Default)

```csharp
// Uses project references
builder.AddProject<Projects.NorthwindAspire_Backend>("apiservice")
builder.AddProject<Projects.NorthwindAspire_Frontend>("webfrontend")
```

**Run with:**
```bash
dotnet run --project NorthwindAspire.AppHost
```

### Mode 2: Docker Containers

```csharp
// Uses container references
builder.AddContainer("backend", "northwind-backend")
builder.AddContainer("frontend", "northwind-frontend")
```

**Enable with:**
```bash
DOCKER_DEPLOYMENT=true dotnet run --project NorthwindAspire.AppHost
```

Or create `.env` file:
```ini
DOCKER_DEPLOYMENT=true
```

---

## ?? Health Checks

Both services include health checks:

**Check status:**
```bash
# View container health
docker-compose ps

# Manual health check
curl http://localhost:5000/health
curl http://localhost:3000/health
```

**Health check details:**
```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
  interval: 30s      # Check every 30 seconds
  timeout: 10s       # Wait max 10 seconds for response
  retries: 3         # Unhealthy after 3 consecutive failures
  start_period: 10s  # Grace period before first check
```

---

## ?? Networking

Services communicate within Docker network `northwind-network`:

**Service-to-service communication:**
```
Backend:  http://backend:8080
Frontend: http://frontend:8080
```

**Host machine access:**
```
Backend:  http://localhost:5000
Frontend: http://localhost:3000
```

**Frontend ? Backend:**
```csharp
// Inside frontend container
var apiUrl = Environment.GetEnvironmentVariable("API_URL") 
             ?? "http://backend:8080";
```

---

## ?? Security Considerations

### HTTPS/SSL Setup

For production, configure certificates:

```yaml
environment:
  - ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/aspnetapp.pfx
  - ASPNETCORE_Kestrel__Certificates__Default__Password=${ASPNETCORE_CERT_PASSWORD}
volumes:
  - ./https:/root/.aspnet/https:ro
```

### Network Isolation

- Services only accessible via network
- No direct container-to-container port access
- Host firewall controls external access

### Environment Secrets

```bash
# Create .env file (not in git)
ASPNETCORE_CERT_PASSWORD=mysecurepassword

# Docker Compose reads .env automatically
```

---

## ?? Monitoring and Logging

### View Logs

```bash
# All services
docker-compose logs

# Specific service
docker-compose logs backend
docker-compose logs frontend

# Follow logs (live)
docker-compose logs -f

# Last 100 lines
docker-compose logs --tail=100
```

### Resource Usage

```bash
# View CPU/Memory usage
docker stats

# Check specific container
docker stats northwind-backend
```

### Health Status

```bash
# Container details including health
docker inspect northwind-backend

# Extract just health
docker inspect --format='{{.State.Health}}' northwind-backend
```

---

## ?? Troubleshooting

### Issue: "Cannot connect to Docker daemon"

```bash
# Windows/macOS: Start Docker Desktop
# Linux: Start Docker service
sudo systemctl start docker

# Verify Docker is running
docker ps
```

### Issue: "Port already in use"

```bash
# Find process using port
netstat -ano | findstr :5000          # Windows
lsof -i :5000                          # macOS/Linux

# Change port in docker-compose.yml
# Or stop conflicting service
docker stop <container-name>
```

### Issue: "Health check failing"

```bash
# Check service logs
docker-compose logs backend

# Test health endpoint
docker-compose exec backend curl http://localhost:8080/health

# Increase start_period in docker-compose.yml
start_period: 30s  # More time for startup
```

### Issue: "Cannot reach backend from frontend"

```bash
# Verify network connectivity
docker-compose exec frontend ping backend

# Check service name in API_URL environment variable
# Should be: http://backend:8080 (not localhost)

# Verify backend is healthy
docker-compose exec frontend curl http://backend:8080/health
```

### Issue: "Disk space full"

```bash
# Clean up Docker resources
docker system prune -a

# Remove specific image
docker rmi northwind-backend:latest

# Remove unused volumes
docker volume prune
```

---

## ?? Updating Containers

### Rebuild images with latest code

```bash
# Rebuild all images
docker-compose build --no-cache

# Rebuild specific service
docker-compose build --no-cache backend

# Start with new images
docker-compose up -d
```

### Update without downtime

```bash
# Build new image
docker-compose build frontend

# Update running container (zero-downtime)
docker-compose up -d frontend
```

---

## ?? Production Deployment Checklist

- [ ] Build Docker images locally
- [ ] Test with `docker-compose up -d`
- [ ] Push images to Azure Container Registry
- [ ] Update environment variables for production
- [ ] Configure health checks appropriately
- [ ] Set up monitoring/logging
- [ ] Configure auto-restart policies
- [ ] Test backup/recovery procedures
- [ ] Document rollback procedures
- [ ] Monitor resource usage in production

---

## ?? Related Documentation

- See `DOCKER.md` for detailed Docker commands
- See `azure-pipelines.md` for CI/CD integration
- See `aspire.md` for Aspire orchestration
- See `azure-pipelines.md` for cloud deployment

---

## Next Steps

1. **Test locally:**
   ```bash
   docker-compose up -d
   ```

2. **Verify services:**
   ```bash
   docker-compose ps
   curl http://localhost:5000/health
   curl http://localhost:3000/health
   ```

3. **Push to registry:**
   ```bash
   docker tag northwind-backend myregistry.azurecr.io/northwind-backend:latest
   docker push myregistry.azurecr.io/northwind-backend:latest
   ```

4. **Update CI/CD pipeline** to build and push Docker images

5. **Deploy to production** using container registry images
