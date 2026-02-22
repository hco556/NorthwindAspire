# View Mappers and Models Update Summary

## Overview

Updated the `VIEWS_VIEWMODELS_AND_MAPPERS.md` documentation and `NorthwindModels.cs` to provide comprehensive guidance on mapping database views to ViewModels using the existing model classes.

## Changes Made

### 1. Updated VIEWS_VIEWMODELS_AND_MAPPERS.md

#### Database Views List
- Added source model mappings for each view
- Clarified which model classes each view maps to
- Distinguished between single-entity, multi-entity, and aggregate views

Example:
```
1. **Alphabetical list of products** ? `Product`
4. **Invoices** ? `Order`, `OrderDetail`, `Product` (complex)
6. **Order Subtotals** ? `OrderDetail` (aggregate)
```

#### View-Specific Implementation Notes
Replaced generic patterns with four specific patterns:

1. **Pattern 1: Single Entity Views** (Product-based)
   - Alphabetical list of products
   - Current Product List
   - Products Above Average Price
   - Products by Category
   - Direct mapping from Product model

2. **Pattern 2: Multi-Entity Denormalized Views**
   - Orders Qry
   - Customer and Suppliers by City
   - Denormalize Order + Customer + Employee
   - Handle null references safely

3. **Pattern 3: Invoice/Complex Detail Views**
   - Invoices
   - Order Details Extended
   - Map from OrderDetail with navigation chains
   - Include denormalized order and customer data

4. **Pattern 4: Aggregate/Summary Views**
   - Order Subtotals
   - Product Sales for 1997
   - Summary of Sales by Quarter/Year
   - Requires intermediate DTO classes

#### Detailed View-to-Model Mapping Reference

Added comprehensive table showing:
- View number and name
- Source model(s)
- Mapper class name
- ViewModel class name
- Implementation notes

#### Implementation Guidelines Table

Shows mapping strategies by view type:
- Simple Entity
- Filtered Entity
- Related Entities
- Complex Multi-Entity
- Aggregate

#### Complete Example Implementation

Provided full working example of OrderDetailsExtendedMapper showing:
- Proper null handling
- Calculated fields
- XML documentation
- NotSupportedException for MapToModel

### 2. Added Intermediate DTO Classes to NorthwindModels.cs

Created four new intermediate DTO classes for mapping views that don't map directly to entities:

```csharp
public class OrderSubtotal
{
    public int OrderId { get; set; }
    public decimal Subtotal { get; set; }
}

public class SalesAggregate
{
    public int? Year { get; set; }
    public int? Quarter { get; set; }
    public decimal SaleAmount { get; set; }
}

public class CategorySalesAggregate
{
    public int? CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductSales { get; set; }
}

public class CustomerSupplierByCity
{
    public string City { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
}
```

### 3. Added Quick Reference Sections

#### Implementation Priority
- **High Priority (5):** Orders Qry, Invoices, Order Details Extended, Summary of Sales by Year, Current Product List
- **Medium Priority (5):** Summary of Sales by Quarter, Product Sales for 1997, Sales by Category, Products by Category, Sales Totals by Amount
- **Lower Priority (6):** Supporting and specialized views

#### Implementation Workflow
Step-by-step process:
1. Create Intermediate DTOs (if needed)
2. Create ViewModels
3. Create Mappers
4. Register Mappers
5. Create Tests

#### Code Generation Tips
- Entity Framework view mapping example
- PowerShell template for batch generation
- Sample DbSet configuration

## Model Class to View Mapping Matrix

### Direct Entity Mappings

| Model | Views |
|-------|-------|
| **Product** | Alphabetical list of products, Current Product List, Products Above Average Price, Products by Category |
| **Order** | Orders Qry, Quarterly Orders |
| **OrderDetail** | Order Details Extended (base), Invoices (base) |
| **Category** | Products by Category, Category Sales for 1997, Sales by Category |

### Intermediate DTO Mappings

| DTO | Views |
|-----|-------|
| **OrderSubtotal** | Order Subtotals, Sales Totals by Amount |
| **SalesAggregate** | Summary of Sales by Quarter, Summary of Sales by Year, Sales Totals by Amount |
| **CategorySalesAggregate** | Product Sales for 1997, Category Sales for 1997, Sales by Category |
| **CustomerSupplierByCity** | Customer and Suppliers by City |

## Benefits of These Updates

1. **Clarity**: Clear mapping between views and models
2. **Consistency**: Established patterns for different view types
3. **Guidance**: Detailed examples and implementation strategies
4. **Reusability**: Intermediate DTOs for aggregate views
5. **Testability**: Clear null-handling and error patterns
6. **Maintainability**: Well-documented mapper structure

## Next Steps

1. Create ViewModels for each of the 16 views following the templates
2. Create Mapper classes using the patterns provided
3. Register mappers in MapperRegistry
4. Add unit tests for null handling and property mapping
5. Create integration tests to verify view ? ViewModel ? UI flow

## Files Modified

- ? `VIEWS_VIEWMODELS_AND_MAPPERS.md` - Updated with mapping patterns
- ? `src/NorthwindAspire.Backend/Models/NorthwindModels.cs` - Added intermediate DTOs
- ? Build successful - No compilation errors

## Build Status

? **Build Successful**
- No compilation errors
- All existing models intact
- New DTOs properly integrated
- Ready for mapper implementation
