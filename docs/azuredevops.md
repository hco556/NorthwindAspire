# Azure DevOps CI/CD Pipeline - Northwind Aspire

Complete CI/CD setup for building, testing, and deploying the NorthwindAspire backend and frontend projects to Azure App Service across test, staging, and production environments.

---

## Prerequisites

### 1. Azure Resources
Create the following App Service instances:
- `northwind-backend-test` and `northwind-frontend-test`
- `northwind-backend-staging` and `northwind-frontend-staging`
- `northwind-backend-prod` and `northwind-frontend-prod`

Create a Resource Group: `northwind-rg`

### 2. Azure DevOps Service Connection
1. Go to **Project Settings** → **Service connections**
2. Click **New service connection** → **Azure Resource Manager**
3. Select **Service principal (automatic)**
4. Choose your subscription and resource group
5. Name it: `AzureServiceConnection`

### 3. Create Variable Groups in Azure DevOps
Navigate to **Pipelines** → **Library** and create these groups:

**northwind-vars-test** (linked to test environment):
```
APP_SERVICE_RESOURCE_GROUP=northwind-rg
BACKEND_APP_SERVICE=northwind-backend-test
FRONTEND_APP_SERVICE=northwind-frontend-test
```

**northwind-vars-staging** (linked to staging environment):
```
APP_SERVICE_RESOURCE_GROUP=northwind-rg
BACKEND_APP_SERVICE=northwind-backend-staging
FRONTEND_APP_SERVICE=northwind-frontend-staging
```

**northwind-vars-prod** (linked to production environment):
```
APP_SERVICE_RESOURCE_GROUP=northwind-rg
BACKEND_APP_SERVICE=northwind-backend-prod
FRONTEND_APP_SERVICE=northwind-frontend-prod
```

### 4. Configure Environments
1. Go to **Pipelines** → **Environments**
2. Create three environments: `test`, `staging`, `production`
3. For `staging` and `production`, add approval checks:
   - Click **⋯** → **Approvals and checks** → **Approvals**
   - Add your team members as approvers

---

## Setup Instructions

### Step 1: Create the Pipeline File
1. In your repository root, create `azure-pipelines.yml`
2. Paste the YAML content below
3. Commit and push to your repository

### Step 2: Create Pipeline in Azure DevOps
1. Go to **Pipelines** → **New pipeline**
2. Select **Azure Repos Git**
3. Select your repository
4. Choose **Existing Azure Pipelines YAML file**
5. Select `azure-pipelines.yml` and click **Continue**
6. Click **Save and run**

### Step 3: Configure Pipeline
1. Edit pipeline: **Pipeline** → **Edit**
2. Click **⋯ More options** → **Pipeline settings**
3. Set **Build job timeout** to 60 minutes
4. Enable **Make secrets available to builds of forks** if needed

### Step 4: Link Variable Groups
1. Edit the pipeline YAML
2. Verify variable groups are linked to environments:
   - `northwind-vars-test` → `test` environment
   - `northwind-vars-staging` → `staging` environment
   - `northwind-vars-prod` → `production` environment

---

## Pipeline YAML Content

Create `azure-pipelines.yml` in your repository root with the following:

```yaml
trigger:
  branches:
    include:
    - main
    - develop
  paths:
    include:
    - src/**
    - azure-pipelines.yml

pr:
  branches:
    include:
    - main
    - develop

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '10.0.x'
  artifactName: 'drop'

stages:
  - stage: BuildAndTest
    displayName: 'Build & Test'
    jobs:
      - job: Build
        displayName: 'Build Projects'
        pool:
          vmImage: 'windows-latest'
        
        steps:
          - task: UseDotNet@2
            displayName: 'Install .NET 10'
            inputs:
              version: $(dotnetVersion)
              packageType: sdk

          - task: DotNetCoreCLI@2
            displayName: 'Restore packages'
            inputs:
              command: 'restore'
              projects: '**/*.csproj'

          - task: DotNetCoreCLI@2
            displayName: 'Build Backend'
            inputs:
              command: 'build'
              projects: 'src/NorthwindAspire.Backend/**/*.csproj'
              arguments: '--configuration $(buildConfiguration) --no-restore'

          - task: DotNetCoreCLI@2
            displayName: 'Build Frontend'
            inputs:
              command: 'build'
              projects: 'src/NorthwindAspire.Frontend/**/*.csproj'
              arguments: '--configuration $(buildConfiguration) --no-restore'

          - task: DotNetCoreCLI@2
            displayName: 'Run Tests'
            inputs:
              command: 'test'
              projects: 'NorthwindAspire.Tests/**/*.csproj'
              arguments: '--configuration $(buildConfiguration) --no-build --logger trx --collect:"XPlat Code Coverage"'
              publishTestResults: true

          - task: PublishCodeCoverageResults@1
            displayName: 'Publish Code Coverage'
            inputs:
              codeCoverageTool: 'Cobertura'
              summaryFileLocation: '$(Agent.TempDirectory)/**/*coverage.cobertura.xml'

          - task: DotNetCoreCLI@2
            displayName: 'Publish Backend'
            inputs:
              command: 'publish'
              publishWebProjects: false
              projects: 'src/NorthwindAspire.Backend/**/*.csproj'
              arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/backend'
              zipAfterPublish: true

          - task: DotNetCoreCLI@2
            displayName: 'Publish Frontend'
            inputs:
              command: 'publish'
              publishWebProjects: false
              projects: 'src/NorthwindAspire.Frontend/**/*.csproj'
              arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)/frontend'
              zipAfterPublish: true

          - task: PublishBuildArtifacts@1
            displayName: 'Publish Artifacts'
            inputs:
              PathtoPublish: '$(Build.ArtifactStagingDirectory)'
              ArtifactName: $(artifactName)
              publishLocation: 'Container'

  - stage: DeployTest
    displayName: 'Deploy to Test'
    dependsOn: BuildAndTest
    condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/develop'))
    variables:
      - group: northwind-vars-test
    
    jobs:
      - deployment: DeployBackendTest
        displayName: 'Deploy Backend to Test'
        environment: 'test'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@0
                  inputs:
                    buildType: 'current'
                    downloadType: 'single'
                    artifactName: $(artifactName)
                    downloadPath: '$(Pipeline.Workspace)'

                - task: AzureWebApp@1
                  displayName: 'Deploy Backend'
                  inputs:
                    azureSubscription: 'AzureServiceConnection'
                    appType: 'webAppLinux'
                    appName: $(BACKEND_APP_SERVICE)
                    package: '$(Pipeline.Workspace)/$(artifactName)/backend/'
                    deploymentMethod: 'zipDeploy'

      - deployment: DeployFrontendTest
        displayName: 'Deploy Frontend to Test'
        dependsOn: DeployBackendTest
        environment: 'test'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@0
                  inputs:
                    buildType: 'current'
                    downloadType: 'single'
                    artifactName: $(artifactName)
                    downloadPath: '$(Pipeline.Workspace)'

                - task: AzureWebApp@1
                  displayName: 'Deploy Frontend'
                  inputs:
                    azureSubscription: 'AzureServiceConnection'
                    appType: 'webAppLinux'
                    appName: $(FRONTEND_APP_SERVICE)
                    package: '$(Pipeline.Workspace)/$(artifactName)/frontend/'
                    deploymentMethod: 'zipDeploy'

  - stage: DeployStaging
    displayName: 'Deploy to Staging'
    dependsOn: BuildAndTest
    condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
    variables:
      - group: northwind-vars-staging
    
    jobs:
      - deployment: DeployBackendStaging
        displayName: 'Deploy Backend to Staging'
        environment: 'staging'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@0
                  inputs:
                    buildType: 'current'
                    downloadType: 'single'
                    artifactName: $(artifactName)
                    downloadPath: '$(Pipeline.Workspace)'

                - task: AzureWebApp@1
                  displayName: 'Deploy Backend'
                  inputs:
                    azureSubscription: 'AzureServiceConnection'
                    appType: 'webAppLinux'
                    appName: $(BACKEND_APP_SERVICE)
                    package: '$(Pipeline.Workspace)/$(artifactName)/backend/'
                    deploymentMethod: 'zipDeploy'

      - deployment: DeployFrontendStaging
        displayName: 'Deploy Frontend to Staging'
        dependsOn: DeployBackendStaging
        environment: 'staging'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@0
                  inputs:
                    buildType: 'current'
                    downloadType: 'single'
                    artifactName: $(artifactName)
                    downloadPath: '$(Pipeline.Workspace)'

                - task: AzureWebApp@1
                  displayName: 'Deploy Frontend'
                  inputs:
                    azureSubscription: 'AzureServiceConnection'
                    appType: 'webAppLinux'
                    appName: $(FRONTEND_APP_SERVICE)
                    package: '$(Pipeline.Workspace)/$(artifactName)/frontend/'
                    deploymentMethod: 'zipDeploy'

  - stage: DeployProduction
    displayName: 'Deploy to Production'
    dependsOn: DeployStaging
    condition: succeeded()
    variables:
      - group: northwind-vars-prod
    
    jobs:
      - deployment: DeployBackendProd
        displayName: 'Deploy Backend to Production'
        environment: 'production'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@0
                  inputs:
                    buildType: 'current'
                    downloadType: 'single'
                    artifactName: $(artifactName)
                    downloadPath: '$(Pipeline.Workspace)'

                - task: AzureWebApp@1
                  displayName: 'Deploy Backend'
                  inputs:
                    azureSubscription: 'AzureServiceConnection'
                    appType: 'webAppLinux'
                    appName: $(BACKEND_APP_SERVICE)
                    package: '$(Pipeline.Workspace)/$(artifactName)/backend/'
                    deploymentMethod: 'zipDeploy'

      - deployment: DeployFrontendProd
        displayName: 'Deploy Frontend to Production'
        dependsOn: DeployBackendProd
        environment: 'production'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@0
                  inputs:
                    buildType: 'current'
                    downloadType: 'single'
                    artifactName: $(artifactName)
                    downloadPath: '$(Pipeline.Workspace)'

                - task: AzureWebApp@1
                  displayName: 'Deploy Frontend'
                  inputs:
                    azureSubscription: 'AzureServiceConnection'
                    appType: 'webAppLinux'
                    appName: $(FRONTEND_APP_SERVICE)
                    package: '$(Pipeline.Workspace)/$(artifactName)/frontend/'
                    deploymentMethod: 'zipDeploy'
```

---

## Pipeline Architecture

```
┌─────────────────────────────┐
│    Build & Test Stage       │
│  • .NET 10 SDK              │
│  • NuGet Restore            │
│  • Build Backend & Frontend │
│  • Run Unit Tests           │
│  • Code Coverage Report     │
│  • Publish Artifacts        │
└──────────────┬──────────────┘
               │
      ┌────────┴────────┐
      │                 │
┌─────▼─────┐    ┌─────▼──────┐
│ Test      │    │ Staging    │
│ Deploy    │    │ Deploy     │
│ (develop) │    │ (main)     │
└─────┬─────┘    └─────┬──────┘
      │                │
      │          ┌─────▼──────┐
      │          │Production  │
      │          │Deploy      │
      │          │(manual)    │
      │          └────────────┘
      │
Each stage:
• Deploy Backend
• Deploy Frontend
```

---

## Deployment Flow

| Branch | Trigger | Target | Approval |
|--------|---------|--------|----------|
| `develop` | Push | Test | Auto |
| `main` | Push | Staging | Auto |
| `staging` | Success | Production | Manual |

---

## Key Features

✅ **Multi-stage deployment**: test → staging → production  
✅ **Automatic test deployment**: triggered from develop branch  
✅ **Automatic staging deployment**: triggered from main branch  
✅ **Manual production approval**: requires team approval  
✅ **Comprehensive testing**: unit tests with code coverage  
✅ **Artifact management**: published artifacts cached between stages  
✅ **Dependency ordering**: backend deploys before frontend  
✅ **Environment isolation**: separate variable groups per environment  
✅ **Service connection**: secured Azure Resource Manager integration  

---

## Troubleshooting

**"App Service not found" error**
- Verify App Service names in variable groups match Azure resources
- Check service connection has correct subscription selected
- Ensure service principal has Contributor role

**"Unauthorized" or auth errors**
- Regenerate publish profile from Azure Portal
- Verify credentials in variable groups
- Check service connection permissions

**Tests failing during build**
- Run locally: `dotnet test NorthwindAspire.Tests`
- Check test project dependencies
- Verify test configuration and data setup

**Deployment timeout**
- Increase build timeout in pipeline settings (set to 60+ minutes)
- Check App Service deployment logs
- Consider parallel build optimization

**Variable group not found**
- Verify variable group names match pipeline YAML
- Ensure groups are linked to correct environments
- Check pipeline permissions for variable group access

---

## Post-Setup Verification

1. **Push to develop branch** - should trigger test deployment
2. **Push to main branch** - should trigger staging deployment
3. **Approve production stage** - manual step to production
4. **Monitor deployments** - watch in Pipelines > Recent runs
5. **Check App Services** - verify apps running in Azure Portal

---

## Next Steps

1. Create `azure-pipelines.yml` in repository root
2. Create service connection in Azure DevOps
3. Create three variable groups
4. Create three environments with approval gates
5. Create pipeline from YAML file
6. Test with pull request to develop/main branches
7. Monitor and optimize deployment process
