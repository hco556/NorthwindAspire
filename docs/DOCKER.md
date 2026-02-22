# Docker Orchestration - NorthwindAspire

Complete Docker setup for containerizing and orchestrating the NorthwindAspire backend and frontend applications.

---

## Overview

This setup provides:

✅ **Multi-stage Docker builds** for optimized image sizes  
✅ **Docker Compose orchestration** for local development and production  
✅ **Service health checks** for container monitoring  
✅ **Network isolation** between services  
✅ **Volume management** for development hot-reload  
✅ **.NET 10 runtime** for both services  

---

## Docker Files Included

### 1. `src/NorthwindAspire.Backend/Dockerfile`
- **Multi-stage build** for optimized production image
- **Build stage**: Compiles .NET 10 backend
- **Runtime stage**: ASP.NET 10 runtime
- **Health check**: HTTP endpoint verification
- **Exposed ports**: 8080 (HTTP), 8443 (HTTPS)

### 2. `src/NorthwindAspire.Frontend/Dockerfile`
- **Multi-stage build** for optimized production image
- **Build stage**: Compiles Blazor Server frontend
- **Runtime stage**: ASP.NET 10 runtime
- **Health check**: HTTP endpoint verification
- **Exposed ports**: 8080 (HTTP), 8443 (HTTPS)

### 3. `docker-compose.yml` (Production)
Production-ready orchestration with:
- Backend service on ports 5000/5001
- Frontend service on ports 3000/3001
- Service networking and health checks
- Automatic restart policies

### 4. `docker-compose.dev.yml` (Development)
Development orchestration with:
- `dotnet watch` for hot-reload
- Volume mounts for source code
- Development environment settings
- Debug-friendly configuration

### 5. `.dockerignore`
Excludes unnecessary files from Docker build context

---

## Prerequisites

- **Docker Desktop** installed and running
  - Windows: https://www.docker.com/products/docker-desktop
  - macOS: https://www.docker.com/products/docker-desktop
  - Linux: `sudo apt-get install docker.io`

- **Docker Compose** (included with Docker Desktop)

- **.NET 10 SDK** (optional, only needed for local development)

---

## Quick Start

### Production Build and Run

```bash
# Navigate to repository root
cd /path/to/NorthwindAspire

# Build Docker images
docker-compose build

# Start services
docker-compose up -d

# View logs
docker-compose logs -f
```

**Access applications:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- OpenAPI docs: http://localhost:5000/openapi/v1.json

### Development with Hot-Reload

```bash
# Build development images
docker-compose -f docker-compose.dev.yml build

# Start with hot-reload
docker-compose -f docker-compose.dev.yml up

# Press Ctrl+C to stop
```

### Stop Services

```bash
# Stop running containers
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Stop specific service
docker-compose stop backend
docker-compose stop frontend
```

---

## Building Individual Images

### Build Backend Image Only

```bash
docker build -f src/NorthwindAspire.Backend/Dockerfile -t northwind-backend:latest .
```

### Build Frontend Image Only

```bash
docker build -f src/NorthwindAspire.Frontend/Dockerfile -t northwind-frontend:latest .
```

### Run Backend Container

```bash
docker run -d \
  --name northwind-backend \
  -p 5000:8080 \
  -p 5001:8443 \
  --health-cmd="curl -f http://localhost:8080/health || exit 1" \
  northwind-backend:latest
```

### Run Frontend Container

```bash
docker run -d \
  --name northwind-frontend \
  -p 3000:8080 \
  -p 3001:8443 \
  -e API_URL=http://host.docker.internal:5000 \
  --health-cmd="curl -f http://localhost:8080/health || exit 1" \
  northwind-frontend:latest
```

---

## Docker Compose Commands

### Common Commands

```bash
# Start all services in background
docker-compose up -d

# Start specific service
docker-compose up -d backend
docker-compose up -d frontend

# View running services
docker-compose ps

# View logs
docker-compose logs              # All services
docker-compose logs -f           # Follow mode
docker-compose logs backend      # Specific service
docker-compose logs -f frontend  # Specific service, follow

# Execute command in running container
docker-compose exec backend sh
docker-compose exec frontend sh

# Rebuild images
docker-compose build --no-cache

# Stop services
docker-compose stop

# Remove stopped containers
docker-compose rm

# Restart services
docker-compose restart
```

---

## Environment Variables

### Backend Service

```ini
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080;https://+:8443
ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/aspnetapp.pfx
```

### Frontend Service

```ini
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080;https://+:8443
API_URL=http://backend:8080
```

**To override variables:**

```bash
# Create .env file in project root
ASPNETCORE_ENVIRONMENT=Development
API_URL=http://localhost:5000

# Or pass via command line
docker-compose run -e ASPNETCORE_ENVIRONMENT=Development frontend
```

---

## Health Checks

Each service includes a health check:

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
  interval: 30s      # Check every 30 seconds
  timeout: 10s       # Wait 10 seconds for response
  retries: 3         # Fail after 3 failures
  start_period: 10s  # Wait 10s before first check
```

**Check container health:**

```bash
# View health status
docker-compose ps

# Output example:
# NAME               STATUS              PORTS
# northwind-backend  Up 2 minutes (healthy)  5000->8080/tcp
# northwind-frontend Up 1 minute (healthy)   3000->8080/tcp
```

---

## Networking

Services communicate via Docker network `northwind-network`:

- **Backend**: Accessible as `http://backend:8080` within network
- **Frontend**: Accessible as `http://frontend:8080` within network
- **From host**: Use localhost with mapped ports (5000, 3000)

**Custom network example:**

```bash
# Create custom network
docker network create northwind-net

# Run container on network
docker run --network northwind-net -d northwind-backend:latest
```

---

## Production Deployment

### Push to Container Registry

```bash
# Tag images
docker tag northwind-backend:latest myregistry.azurecr.io/northwind-backend:latest
docker tag northwind-frontend:latest myregistry.azurecr.io/northwind-frontend:latest

# Push to Azure Container Registry
docker push myregistry.azurecr.io/northwind-backend:latest
docker push myregistry.azurecr.io/northwind-frontend:latest
```

### Deploy to Azure Container Instances (ACI)

```bash
# Create resource group
az group create --name northwind-rg --location eastus

# Deploy backend
az container create \
  --resource-group northwind-rg \
  --name northwind-backend \
  --image myregistry.azurecr.io/northwind-backend:latest \
  --port 8080 \
  --registry-login-server myregistry.azurecr.io \
  --registry-username <username> \
  --registry-password <password>

# Deploy frontend
az container create \
  --resource-group northwind-rg \
  --name northwind-frontend \
  --image myregistry.azurecr.io/northwind-frontend:latest \
  --port 8080 \
  --environment-variables API_URL=http://northwind-backend:8080
```

### Deploy to Kubernetes

```bash
# Create deployment manifests
kubectl create deployment northwind-backend \
  --image=myregistry.azurecr.io/northwind-backend:latest

kubectl create deployment northwind-frontend \
  --image=myregistry.azurecr.io/northwind-frontend:latest

# Expose services
kubectl expose deployment northwind-backend --port=8080 --type=LoadBalancer
kubectl expose deployment northwind-frontend --port=8080 --type=LoadBalancer
```

---

## Troubleshooting

### Container won't start

```bash
# Check logs
docker-compose logs backend
docker-compose logs frontend

# Common issues:
# - Port already in use: Change port mapping in docker-compose.yml
# - Out of memory: Increase Docker memory limits
# - Network issues: Check firewall settings
```

### Health check failing

```bash
# Test health endpoint manually
docker-compose exec backend curl http://localhost:8080/health
docker-compose exec frontend curl http://localhost:8080/health

# Increase start_period if startup is slow
# Or increase timeout if service is slow to respond
```

### Can't reach backend from frontend

```bash
# Verify network connectivity
docker-compose exec frontend ping backend

# Check service names in docker-compose.yml
# Frontend should use: http://backend:8080
# Not: http://localhost:5000 (that's only from host machine)
```

### Port conflicts

```bash
# Find what's using port 5000
netstat -ano | findstr :5000          # Windows
lsof -i :5000                          # macOS/Linux

# Change ports in docker-compose.yml
# Or stop conflicting service: docker stop <container-name>
```

### Docker disk space full

```bash
# Clean up unused images and containers
docker system prune -a

# Remove specific image
docker rmi northwind-backend:latest

# Reclaim disk space
docker system prune -a --volumes
```

---

## Integration with Aspire

The Docker setup complements Aspire orchestration:

**With Aspire (Development):**
- `AppHost.cs` orchestrates services
- Full debugging capabilities
- Aspire Dashboard for monitoring

**With Docker Compose (Local/Production):**
- Lightweight containerized environment
- No Aspire dependencies
- Closer to production setup
- CI/CD pipeline compatible

**Recommendation:**
- Use **Aspire** during development for rich debugging
- Use **Docker Compose** for containerized testing
- Use **Docker images** in CI/CD pipelines
- Use **Docker/Kubernetes** for production deployment

---

## Next Steps

1. Build Docker images: `docker-compose build`
2. Start services: `docker-compose up -d`
3. Test applications at localhost:3000 and localhost:5000
4. Integrate into CI/CD pipeline (Azure DevOps)
5. Push images to container registry (ACR)
6. Deploy to production (App Service, ACI, or Kubernetes)
