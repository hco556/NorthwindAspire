# Quick Start Guide - Running OData API Tests

## Prerequisites

- .NET 10.0 SDK installed
- NorthwindAspire solution cloned
- Visual Studio, VS Code, or terminal available

---

## Running Tests - Quick Commands

### 1. **Run All Tests**
```bash
dotnet test
```

### 2. **Run Tests with Verbose Output**
```bash
dotnet test --verbosity normal
```

### 3. **Run Specific Test Class**
```bash
dotnet test --filter "FullyQualifiedName~CategoriesControllerTests"
```

### 4. **Run Tests Matching Pattern**
```bash
# All Category tests
dotnet test --filter "FullyQualifiedName~Categories"

# All GET tests
dotnet test --filter "Name~GetAll"

# All Create tests
dotnet test --filter "Name~Create"
```

### 5. **Run Tests in Parallel**
```bash
dotnet test -p:ParallelizeTestCollections=true
```

### 6. **Run Tests with Coverage**
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### 7. **Run Specific Test Method**
```bash
dotnet test --filter "Name=GetAllCategories_ReturnsOkStatus"
```

### 8. **Watch Mode (Auto-run on changes)**
```bash
dotnet watch test
```

---

## Expected Output

```
Starting test execution, please wait...

A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    52, Skipped:     0, Duration: 8.5s

Test Run Successful.
Build: Successful.
```

---

## Test Files Overview

| File | Tests | Coverage |
|------|-------|----------|
| CategoriesControllerTests | 12 | GET, POST, PATCH, DELETE, Filters, Sorting |
| CustomersControllerTests | 10 | String keys, Contains filter, Expand |
| ProductsControllerTests | 10 | Price filtering, Sorting, Category expand |
| OrdersControllerTests | 10 | Date filtering, Customer expand |
| EmployeesControllerTests | 10 | Title filtering, Relationships |

**Total: 52+ Tests**

---

## Test Names by Category

### **Categories Controller**
```
GetAllCategories_ReturnsOkStatus
GetAllCategories_ReturnsCollectionWithItems
GetCategoryById_WithValidId_ReturnsCategory
GetCategoryById_WithInvalidId_ReturnsNotFound
CreateCategory_WithValidData_ReturnsCreated
CreateCategory_WithInvalidData_ReturnsBadRequest
UpdateCategory_WithValidId_ReturnsNoContent
UpdateCategory_WithInvalidId_ReturnsNotFound
DeleteCategory_WithValidId_ReturnsNoContent
DeleteCategory_WithInvalidId_ReturnsNotFound
GetCategories_WithFilter_ReturnsFilteredResults
GetCategories_WithSelect_ReturnsOnlySelectedFields
GetCategories_WithTop_LimitResults
GetCategories_WithOrderBy_ReturnsSortedResults
```

### **Customers Controller**
```
GetAllCustomers_ReturnsOkStatus
GetCustomerById_WithValidId_ReturnsCustomer
CreateCustomer_WithValidData_ReturnsCreated
UpdateCustomer_WithValidId_UpdatesData
DeleteCustomer_WithValidId_ReturnsNoContent
GetCustomers_WithContainsFilter_ReturnsMatches
GetCustomers_WithExpandOrders_IncludesRelatedData
GetCustomers_WithPagination_ReturnsPaginatedResults
```

### **Products Controller**
```
GetAllProducts_ReturnsOkStatus
GetProductById_WithValidId_ReturnsProduct
CreateProduct_WithValidData_ReturnsCreated
UpdateProduct_WithValidId_UpdatesPrice
DeleteProduct_WithValidId_ReturnsNoContent
GetProducts_WithPriceFilter_ReturnsFilteredResults
GetProducts_OrderByPrice_ReturnsSortedByPrice
GetProducts_WithCategoryExpand_IncludesCategory
```

### **Orders Controller**
```
GetAllOrders_ReturnsOkStatus
GetOrderById_WithValidId_ReturnsOrder
CreateOrder_WithValidData_ReturnsCreated
UpdateOrder_WithValidId_UpdatesFreight
DeleteOrder_WithValidId_ReturnsNoContent
GetOrders_WithDateFilter_ReturnsFilteredResults
GetOrders_WithExpandCustomer_IncludesCustomerData
```

### **Employees Controller**
```
GetAllEmployees_ReturnsOkStatus
GetEmployeeById_WithValidId_ReturnsEmployee
CreateEmployee_WithValidData_ReturnsCreated
UpdateEmployee_WithValidId_UpdatesTitle
DeleteEmployee_WithValidId_ReturnsNoContent
GetEmployees_FilterByTitle_ReturnsMatchingEmployees
GetEmployees_ExpandOrders_IncludesRelatedOrders
```

---

## Common Test Patterns

### Run All Tests for One Entity
```bash
dotnet test --filter "FullyQualifiedName~CategoriesController"
```

### Run All CRUD Tests
```bash
dotnet test --filter "Name~Create|Name~Update|Name~Delete"
```

### Run All GET Tests
```bash
dotnet test --filter "Name~GetAll|Name~GetById"
```

### Run All OData Query Tests
```bash
dotnet test --filter "Name~Filter|Name~Select|Name~OrderBy|Name~Top"
```

---

## Visual Studio Integration

### **Test Explorer**
1. Open **Test Explorer** (Ctrl+E, T)
2. See all tests organized by class
3. Run by clicking "Run" next to any test or class
4. View results with detailed pass/fail info

### **Debug a Test**
1. Right-click test in Test Explorer
2. Select "Debug Selected Tests"
3. Set breakpoints as needed
4. Inspect variables in Watch window

---

## Azure DevOps / GitHub Actions

### **View CI/CD Test Results**
- **GitHub Actions**: Go to repository ? Actions tab ? test-odata-api workflow
- **Azure DevOps**: Go to Pipelines ? test-odata-api ? Recent runs

### **Download Test Artifacts**
- Both CI systems publish test result files as artifacts
- Download `.trx` files for detailed analysis

---

## Troubleshooting

### **Tests won't run**
```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test
```

### **Timeout errors**
- Increase timeout in NorthwindAspire.Tests/.nunit file
- Or use longer waits in slow test environments

### **Database errors**
- Clear build cache: `dotnet clean`
- Ensure SQLite InMemory package is installed
- Check that Backend project builds successfully

### **Missing test file**
- Ensure all files are in `NorthwindAspire.Tests/` directory
- Check `NorthwindAspire.Tests.csproj` references

---

## Code Coverage Report

### Generate Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### View Coverage
- Coverage report generated in `coverage/` directory
- Open `coverage/index.html` in browser
- See line-by-line coverage analysis

---

## Best Practices

? **DO:**
- Run tests before committing
- Run full test suite before pushing to main branch
- Check test results in CI/CD pipeline
- Keep test data small and isolated
- Use meaningful test names

? **DON'T:**
- Modify test data during test execution
- Create interdependent tests
- Use hardcoded timeouts
- Skip test cleanup
- Ignore test failures

---

## Performance Tips

1. **Run tests in parallel** (careful with shared resources)
   ```bash
   dotnet test -p:ParallelizeTestCollections=true
   ```

2. **Run only changed tests**
   ```bash
   dotnet test --filter "Name~ClassName"
   ```

3. **Skip code coverage for faster runs**
   ```bash
   dotnet test
   ```

4. **Use SSD for better disk performance**

---

## Additional Targets

Run tests for specific test classes:
```bash
# Categories only
dotnet test --filter "FullyQualifiedName~CategoriesControllerTests"

# Customers only
dotnet test --filter "FullyQualifiedName~CustomersControllerTests"

# Products only
dotnet test --filter "FullyQualifiedName~ProductsControllerTests"

# Orders only
dotnet test --filter "FullyQualifiedName~OrdersControllerTests"

# Employees only
dotnet test --filter "FullyQualifiedName~EmployeesControllerTests"
```

---

## Support

For issues or questions:
1. Check TESTING_IMPLEMENTATION_SUMMARY.md
2. Review ODATA-TESTING.md for detailed documentation
3. Check test files for examples
4. Review error messages in test output

---

## Next Steps

1. Run the test suite: `dotnet test`
2. Verify all tests pass
3. Generate code coverage report
4. Commit test files to repository
5. Monitor CI/CD pipeline runs
6. Extend tests as needed

---

**Happy Testing!** ???
