# Northwind Database Views - SQL Definitions and Documentation

This document describes the 16 SQL views created for the Northwind database and their purposes.

## Overview

These views provide pre-built queries for common reporting and data analysis scenarios. All views are compatible with SQLite3 and are designed to work with the Northwind schema.

---

## View Definitions

### 1. Alphabetical list of products

**Purpose:** Displays all products sorted alphabetically by product name.

**Columns:**
- `ProductID` - Unique product identifier
- `ProductName` - Product name
- `SupplierID` - Supplier reference
- `CategoryID` - Category reference
- `QuantityPerUnit` - Package quantity
- `UnitPrice` - Product price
- `UnitsInStock` - Current inventory
- `UnitsOnOrder` - Pending orders
- `ReorderLevel` - Minimum stock level
- `Discontinued` - Discontinuation status

**Use Case:** Product browsing, alphabetical product listing, inventory management

---

### 2. Current Product List

**Purpose:** Returns only active (non-discontinued) products with their IDs and names.

**Columns:**
- `ProductID` - Unique product identifier
- `ProductName` - Product name

**Use Case:** Dropdown lists, product selection, active products only

---

### 3. Customer and Suppliers by City

**Purpose:** Combines customers and suppliers, grouped by city, showing their relationships.

**Columns:**
- `City` - City name
- `CompanyName` - Company name
- `ContactName` - Contact person name
- `Relationship` - 'Customers' or 'Suppliers'

**Use Case:** Location-based business analysis, finding nearby suppliers and customers

---

### 4. Invoices

**Purpose:** Comprehensive invoice view combining orders, order details, products, customers, employees, and shippers.

**Columns:**
- `OrderID` - Order identifier
- `CustomerID` - Customer reference
- `CompanyName` - Customer company name
- `Address`, `City`, `Region`, `PostalCode`, `Country` - Shipping address fields
- `FirstName`, `LastName` - Employee (salesman) name
- `OrderDate`, `RequiredDate`, `ShippedDate` - Date information
- `ShipperName` - Shipping company
- `ProductID` - Product reference
- `ProductName` - Product name
- `UnitPrice` - Unit price at time of order
- `Quantity` - Quantity ordered
- `Discount` - Discount applied
- `ExtendedPrice` - Calculated total (UnitPrice * Quantity * (1 - Discount))

**Use Case:** Invoice generation, detailed order analysis, billing

---

### 5. Orders Qry

**Purpose:** Provides a detailed view of orders with related customer and employee information.

**Columns:**
- `OrderID` - Order identifier
- `CustomerID` - Customer reference
- `EmployeeID` - Employee reference
- `OrderDate`, `RequiredDate`, `ShippedDate` - Order timing
- `ShipVia`, `Freight` - Shipping details
- `ShipName`, `ShipAddress`, `ShipCity`, `ShipRegion`, `ShipPostalCode`, `ShipCountry` - Destination address
- `CompanyName`, `Address`, `City`, `Region`, `PostalCode`, `Country` - Customer location
- `Salesman` - Employee full name

**Use Case:** Order management, sales tracking, shipping analysis

---

### 6. Order Subtotals

**Purpose:** Calculates the subtotal for each order before shipping costs.

**Columns:**
- `OrderID` - Order identifier
- `Subtotal` - Sum of (UnitPrice × Quantity × (1 - Discount)) for all items

**Use Case:** Order value analysis, revenue calculation, financial reporting

---

### 7. Product Sales for 1997

**Purpose:** Shows product sales aggregated by category for the year 1997.

**Columns:**
- `CategoryName` - Product category
- `ProductName` - Product name
- `ProductSales` - Total sales amount for the product in 1997

**Use Case:** Historical sales analysis, product performance comparison, year-specific reporting

---

### 8. Products Above Average Price

**Purpose:** Lists products priced above the average product price.

**Columns:**
- `ProductName` - Product name
- `UnitPrice` - Product price

**Use Case:** Premium product analysis, pricing analysis, high-value inventory

---

### 9. Products by Category

**Purpose:** Displays all active products organized by category.

**Columns:**
- `CategoryName` - Category name
- `ProductName` - Product name
- `QuantityPerUnit` - Package quantity
- `UnitsInStock` - Current inventory level
- `Discontinued` - Discontinuation status

**Use Case:** Product catalog, inventory browsing, category-based reporting

---

### 10. Quarterly Orders

**Purpose:** Identifies distinct customers who placed orders in the last quarter.

**Columns:**
- `CustomerID` - Customer identifier
- `CompanyName` - Company name
- `City` - City location
- `Country` - Country location

**Use Case:** Recent customer activity, quarterly sales tracking, customer engagement

---

### 11. Sales Totals by Amount

**Purpose:** Shows orders with subtotals exceeding 2500, sorted by amount.

**Columns:**
- `OrderID` - Order identifier
- `CustomerID` - Customer reference
- `Subtotal` - Order subtotal (items only, excluding shipping)

**Use Case:** High-value order analysis, large order tracking, VIP customer identification

---

### 12. Summary of Sales by Quarter

**Purpose:** Aggregates sales data by year and quarter.

**Columns:**
- `Year` - Calendar year
- `Quarter` - Quarter (1-4)
- `SaleAmount` - Total sales for the period

**Use Case:** Quarterly business performance, trend analysis, period-based reporting

---

### 13. Summary of Sales by Year

**Purpose:** Shows total sales aggregated by calendar year.

**Columns:**
- `Year` - Calendar year
- `SaleAmount` - Total sales for the year

**Use Case:** Annual performance review, year-over-year comparison, long-term trend analysis

---

### 14. Category Sales for 1997

**Purpose:** Shows category-level sales totals for 1997.

**Columns:**
- `CategoryName` - Product category name
- `CategorySales` - Total sales by category in 1997

**Use Case:** Category performance in 1997, historical category comparison, product line analysis

---

### 15. Order Details Extended

**Purpose:** Extends order details with product names and calculated extended prices.

**Columns:**
- `OrderID` - Order identifier
- `ProductID` - Product identifier
- `ProductName` - Product name
- `UnitPrice` - Price per unit
- `Quantity` - Quantity ordered
- `Discount` - Discount rate
- `ExtendedPrice` - Calculated total (UnitPrice × Quantity × (1 - Discount))

**Use Case:** Order line-item details, invoice line items, order analysis

---

### 16. Sales by Category

**Purpose:** Aggregates sales by product category, showing individual product performance.

**Columns:**
- `CategoryID` - Category identifier
- `CategoryName` - Category name
- `ProductName` - Product name
- `ProductSales` - Total sales for the product across all orders

**Use Case:** Product performance by category, sales mix analysis, category trending

---

## Implementation Notes

### Date Handling
SQLite date functions are used for year extraction and date comparisons:
- `STRFTIME('%Y', date)` - Extract year
- `STRFTIME('%m', date)` - Extract month
- Quarter calculation: `((MONTH + 2) / 3)`

### Calculated Fields
Several views include calculated fields:
- **ExtendedPrice**: `UnitPrice × Quantity × (1 - Discount)`
- **SaleAmount**: Sum of extended prices
- **Quarter**: Derived from month: `((month + 2) / 3)`

### View Dependencies
Some views depend on others:
- `Sales Totals by Amount` depends on `Order Subtotals`
- `Invoices` and `Order Details Extended` both calculate ExtendedPrice
- Sales summary views all calculate from `Orders` and `Order Details`

### Performance Considerations
- Aggregate views (7, 12, 13, 14, 16) may be slower with large datasets
- Consider creating indexes on:
  - `Orders.OrderDate`
  - `Products.Discontinued`
  - `[Order Details].OrderID`
  - `[Order Details].ProductID`

---

## Creating the Views

To create all views in your SQLite database, execute:

```bash
sqlite3 src/NorthwindAspire.Backend/northwind.db < src/NorthwindAspire.Backend/create_views.sql
```

Or from within SQLite:

```sql
.read src/NorthwindAspire.Backend/create_views.sql
```

---

## Testing the Views

After creating the views, test them with sample queries:

```sql
-- Test view creation
SELECT name FROM sqlite_master WHERE type='view' ORDER BY name;

-- Test a simple view
SELECT * FROM [Current Product List] LIMIT 5;

-- Test an aggregate view
SELECT * FROM [Summary of Sales by Year];

-- Test a complex view
SELECT * FROM Invoices WHERE OrderID = 10248;
```

---

## Related Files

- **View SQL Script**: `src/NorthwindAspire.Backend/create_views.sql`
- **View Implementation Guide**: `VIEWS_VIEWMODELS_AND_MAPPERS.md`
- **Database Schema**: `src/NorthwindAspire.Backend/Northwind.md`
- **Database Context**: `src/NorthwindAspire.Backend/Data/NorthwindContext.cs`
