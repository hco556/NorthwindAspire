# Creating NorthwindAspire.Shared Project - Refactoring Guide

## Overview

This guide provides step-by-step instructions to create a new shared class library project that will contain the common models used by both the Backend and Frontend projects. This refactoring improves code organization and eliminates circular dependencies.

## Table of Contents

1. [Project Structure](#project-structure)
2. [Step-by-Step Instructions](#step-by-step-instructions)
3. [Verification](#verification)
4. [Next Steps](#next-steps)

---

## Project Structure

### Current Structure
```
NorthwindAspire/
??? NorthwindAspire.AppHost/
??? NorthwindAspire.ServiceDefaults/
??? NorthwindAspire.Tests/
??? src/
?   ??? NorthwindAspire.Backend/
?   ?   ??? Models/  (will be moved)
?   ?   ??? Controllers/
?   ?   ??? Data/
?   ?   ??? Program.cs
?   ??? NorthwindAspire.Frontend/
?       ??? Models/
?       ?   ??? ViewModels/
?       ?   ??? Mappers/
?       ??? Components/
?       ??? Program.cs
??? NorthwindAspire.sln
```

### Target Structure
```
NorthwindAspire/
??? NorthwindAspire.AppHost/
??? NorthwindAspire.ServiceDefaults/
??? NorthwindAspire.Shared/  (NEW)
?   ??? Models/  (moved from Backend)
??? NorthwindAspire.Tests/
??? src/
?   ??? NorthwindAspire.Backend/
?   ?   ??? Controllers/
?   ?   ??? Data/
?   ?   ??? Program.cs
?   ??? NorthwindAspire.Frontend/
?       ??? Models/
?       ?   ??? ViewModels/
?       ?   ??? Mappers/
?       ??? Components/
?       ??? Program.cs
??? NorthwindAspire.sln
```

---

## Step-by-Step Instructions

### Step 1: Create the NorthwindAspire.Shared Project

#### Option A: Using Visual Studio

1. **Right-click on the solution** in Solution Explorer
2. Select **Add ? New Project**
3. Search for and select **Class Library**
4. Click **Next**
5. **Configure the new project:**
   - Project name: `NorthwindAspire.Shared`
   - Location: Root directory (same level as `NorthwindAspire.AppHost`)
   - Solution: `Add to solution`
6. Click **Next**
7. **Additional information:**
   - Framework: `.NET 10` (match your other projects)
   - Do **NOT** check "Use top-level statements"
8. Click **Create**

#### Option B: Using .NET CLI

```bash
# Navigate to the solution root directory
cd NorthwindAspire

# Create the new class library
dotnet new classlib -n NorthwindAspire.Shared -f net10.0

# Add the project to the solution
dotnet sln add NorthwindAspire.Shared/NorthwindAspire.Shared.csproj
```

#### Option C: Using PowerShell/Command Line

```powershell
# Navigate to the solution root
cd NorthwindAspire

# Create the project directory
mkdir NorthwindAspire.Shared

# Create the project file
# Copy the following .csproj content to NorthwindAspire.Shared\NorthwindAspire.Shared.csproj:
```

**NorthwindAspire.Shared.csproj content:**
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

Then add to solution:
```bash
dotnet sln add NorthwindAspire.Shared/NorthwindAspire.Shared.csproj
```

---

### Step 2: Delete the Class1.cs File

The new project will contain a default `Class1.cs` file that should be removed.

**In Visual Studio:**
1. Expand `NorthwindAspire.Shared` project in Solution Explorer
2. Right-click **Class1.cs**
3. Select **Delete**
4. Confirm the deletion

**Using Command Line:**
```bash
# Remove the default class file
Remove-Item NorthwindAspire.Shared\Class1.cs -Force
```

---

### Step 3: Move the Models Folder from Backend to Shared

#### Option A: Using Visual Studio (Recommended)

1. **In Solution Explorer**, navigate to `NorthwindAspire.Backend ? Models`
2. **Right-click** the `Models` folder
3. Select **Cut**
4. **Right-click** on the `NorthwindAspire.Shared` project
5. Select **Paste**
6. Visual Studio will prompt to confirm - click **Yes**

#### Option B: Using File Explorer

1. Open **File Explorer**
2. Navigate to `src\NorthwindAspire.Backend\Models`
3. **Cut** the entire `Models` folder
4. Navigate to `NorthwindAspire.Shared`
5. **Paste** the folder
6. In Visual Studio, right-click `NorthwindAspire.Shared` and select **Add ? Existing Folder** (if the folder doesn't appear automatically)
7. Select the `Models` folder and click **Open**

---

### Step 4: Update Backend Project References

Edit **`src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj`**

Replace the project references section with:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\NorthwindAspire.Shared\NorthwindAspire.Shared.csproj" />
  <ProjectReference Include="..\..\NorthwindAspire.ServiceDefaults\NorthwindAspire.ServiceDefaults.csproj" />
</ItemGroup>
```

**From:**
```xml
<ItemGroup>
  <ProjectReference Include="..\..\NorthwindAspire.ServiceDefaults\NorthwindAspire.ServiceDefaults.csproj" />
</ItemGroup>
```

---

### Step 5: Update Frontend Project References

Edit **`src/NorthwindAspire.Frontend/NorthwindAspire.Frontend.csproj`**

Replace the project references section with:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\NorthwindAspire.Shared\NorthwindAspire.Shared.csproj" />
  <ProjectReference Include="..\..\NorthwindAspire.ServiceDefaults\NorthwindAspire.ServiceDefaults.csproj" />
</ItemGroup>
```

**Remove the reference to Backend:**
```xml
<!-- Remove this line -->
<ProjectReference Include="..\NorthwindAspire.Backend\NorthwindAspire.Backend.csproj" />
```

---

### Step 6: Update Namespace References in Backend

The Backend project will now reference the shared models from `NorthwindAspire.Shared.Models` instead of the local `NorthwindAspire.Backend.Models`.

**Update Backend files:**

Edit any file in the Backend project that references models. Change:

**From:**
```csharp
using NorthwindAspire.Backend.Models;
```

**To:**
```csharp
using NorthwindAspire.Shared.Models;
```

**Files to update:**
- `src/NorthwindAspire.Backend/Data/NorthwindContext.cs`
- `src/NorthwindAspire.Backend/Controllers/*.cs` (all controller files)
- `src/NorthwindAspire.Backend/Program.cs` (if it references models)
- Any other files that import from models

---

### Step 7: Update Namespace References in Frontend Mappers

Edit Frontend mapper files to reference the shared models:

**From:**
```csharp
using NorthwindAspire.Backend.Models;
```

**To:**
```csharp
using NorthwindAspire.Shared.Models;
```

**Files to update:**
- `src/NorthwindAspire.Frontend/Models/Mappers/*.cs` (all mapper files)
- `src/NorthwindAspire.Frontend/Models/Mappers/MapperRegistry.cs`

---

### Step 8: Update Test Project References (if needed)

If your test project references backend models, update it similarly.

Edit **`NorthwindAspire.Tests/NorthwindAspire.Tests.csproj`**

Add a reference to the Shared project:

```xml
<ItemGroup>
  <ProjectReference Include="..\NorthwindAspire.Shared\NorthwindAspire.Shared.csproj" />
  <ProjectReference Include="..\src\NorthwindAspire.Backend\NorthwindAspire.Backend.csproj" />
  <ProjectReference Include="..\src\NorthwindAspire.Frontend\NorthwindAspire.Frontend.csproj" />
</ItemGroup>
```

Update test files:
```csharp
// From:
using NorthwindAspire.Backend.Models;

// To:
using NorthwindAspire.Shared.Models;
```

---

## Verification

### Step 1: Check Solution Structure

Verify that the solution structure matches the target structure:

```
NorthwindAspire.sln
??? NorthwindAspire.AppHost/
??? NorthwindAspire.ServiceDefaults/
??? NorthwindAspire.Shared/  ?
?   ??? Models/
??? NorthwindAspire.Tests/
??? src/
    ??? NorthwindAspire.Backend/
    ??? NorthwindAspire.Frontend/
```

### Step 2: Build the Solution

```bash
# Clean the solution
dotnet clean

# Rebuild the solution
dotnet build
```

**Expected result:** ? Build succeeds with no errors

### Step 3: Verify Project References

**Backend project** should reference:
- `NorthwindAspire.Shared`
- `NorthwindAspire.ServiceDefaults`

**Frontend project** should reference:
- `NorthwindAspire.Shared`
- `NorthwindAspire.ServiceDefaults`

**Test project** should reference:
- `NorthwindAspire.Shared`
- `NorthwindAspire.Backend`
- `NorthwindAspire.Frontend`

### Step 4: Verify Namespace Usage

**In Backend files:**
```csharp
using NorthwindAspire.Shared.Models;  // ? Correct
```

**In Frontend files:**
```csharp
using NorthwindAspire.Shared.Models;  // ? Correct
```

### Step 5: Run Tests

```bash
dotnet test
```

**Expected result:** ? All tests pass

---

## Troubleshooting

### Issue: Models folder not visible after paste in Visual Studio

**Solution:**
1. Right-click `NorthwindAspire.Shared` project
2. Select **Add ? Existing Folder**
3. Navigate to and select the `Models` folder
4. Click **Add**

### Issue: Build errors about missing models

**Solution:**
1. Check that the `Models` folder is in `NorthwindAspire.Shared`
2. Update all `using` statements to use `NorthwindAspire.Shared.Models`
3. Clear the solution cache: `dotnet clean`
4. Rebuild: `dotnet build`

### Issue: Circular reference errors

**Solution:**
This should not occur if you followed the steps correctly. If it does:
1. Verify the Frontend no longer references Backend directly
2. Check that only Shared references are in the project files
3. Run `dotnet build` with verbose output: `dotnet build -v diagnostic`

### Issue: Files still show old namespaces

**Solution:**
1. Use Find and Replace (Ctrl+H) in Visual Studio
2. Find: `using NorthwindAspire.Backend.Models`
3. Replace with: `using NorthwindAspire.Shared.Models`
4. Replace all occurrences
5. Rebuild the solution

---

## Next Steps

After completing this refactoring:

1. **Commit your changes:**
   ```bash
   git add .
   git commit -m "Refactor: Create NorthwindAspire.Shared project and move Models"
   ```

2. **Update documentation:**
   - Update README.md with the new project structure
   - Update any architecture documentation
   - Update IMPLEMENTATION_SUMMARY.md files

3. **Verify deployment:**
   - Ensure Docker builds work correctly
   - Verify Azure deployment (if applicable)
   - Run end-to-end tests

4. **Optional enhancements:**
   - Move additional shared code to the Shared project (e.g., DTOs, constants, utilities)
   - Add shared extension methods
   - Move validation logic to the Shared project

---

## Benefits of This Refactoring

? **Eliminates circular dependencies** - Frontend no longer needs to reference Backend  
? **Improves code organization** - Shared models are in a dedicated project  
? **Enhances reusability** - Tests and other projects can easily reference models  
? **Cleaner architecture** - Clear separation of concerns  
? **Easier maintenance** - Models are in one place, not duplicated  

---

## Quick Reference: Files to Update

| File | Change |
|------|--------|
| `src/NorthwindAspire.Backend/NorthwindAspire.Backend.csproj` | Add Shared reference |
| `src/NorthwindAspire.Frontend/NorthwindAspire.Frontend.csproj` | Add Shared reference, remove Backend reference |
| `src/NorthwindAspire.Backend/Data/NorthwindContext.cs` | Update using statements |
| `src/NorthwindAspire.Backend/Controllers/*.cs` | Update using statements |
| `src/NorthwindAspire.Frontend/Models/Mappers/*.cs` | Update using statements |
| `NorthwindAspire.Tests/NorthwindAspire.Tests.csproj` | Add Shared reference |
| Test files | Update using statements |

---

## Summary

This refactoring creates a cleaner architecture by:
1. Creating a dedicated `NorthwindAspire.Shared` project for common models
2. Moving the `Models` folder from Backend to Shared
3. Updating project references so Backend and Frontend both reference Shared
4. Updating all namespace imports throughout the solution

The result is a more maintainable and scalable solution structure.
