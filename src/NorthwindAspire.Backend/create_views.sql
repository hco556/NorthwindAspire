-- ============================================================================
-- Northwind Database Views - SQLite3 Version
-- ============================================================================
-- This script creates all 16 views for the Northwind database
-- Compatible with SQLite3
-- ============================================================================

-- ============================================================================
-- 1. Alphabetical list of products
-- ============================================================================
DROP VIEW IF EXISTS [Alphabetical list of products];
CREATE VIEW [Alphabetical list of products] AS
SELECT 
    Products.ProductID,
    Products.ProductName,
    Products.SupplierID,
    Products.CategoryID,
    Products.QuantityPerUnit,
    Products.UnitPrice,
    Products.UnitsInStock,
    Products.UnitsOnOrder,
    Products.ReorderLevel,
    Products.Discontinued
FROM Products
ORDER BY Products.ProductName;

-- ============================================================================
-- 2. Current Product List
-- ============================================================================
DROP VIEW IF EXISTS [Current Product List];
CREATE VIEW [Current Product List] AS
SELECT 
    ProductID,
    ProductName
FROM Products
WHERE Discontinued = 0;

-- ============================================================================
-- 3. Customer and Suppliers by City
-- ============================================================================
DROP VIEW IF EXISTS [Customer and Suppliers by City];
CREATE VIEW [Customer and Suppliers by City] AS
SELECT 
    City,
    CompanyName,
    ContactName,
    'Customers' AS Relationship
FROM Customers
UNION ALL
SELECT 
    City,
    CompanyName,
    ContactName,
    'Suppliers'
FROM Suppliers
ORDER BY City, CompanyName;

-- ============================================================================
-- 4. Invoices
-- ============================================================================
DROP VIEW IF EXISTS [Invoices];
CREATE VIEW [Invoices] AS
SELECT 
    Orders.OrderID,
    Customers.CustomerID,
    Customers.CompanyName,
    Customers.Address,
    Customers.City,
    Customers.Region,
    Customers.PostalCode,
    Customers.Country,
    Employees.FirstName,
    Employees.LastName,
    Orders.OrderDate,
    Orders.RequiredDate,
    Orders.ShippedDate,
    Shippers.CompanyName AS ShipperName,
    [Order Details].ProductID,
    Products.ProductName,
    [Order Details].UnitPrice,
    [Order Details].Quantity,
    [Order Details].Discount,
    ([Order Details].UnitPrice * [Order Details].Quantity * (1 - [Order Details].Discount)) AS ExtendedPrice
FROM Shippers 
INNER JOIN Orders ON Shippers.ShipperID = Orders.ShipVia 
INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID 
INNER JOIN Employees ON Orders.EmployeeID = Employees.EmployeeID 
INNER JOIN [Order Details] ON Orders.OrderID = [Order Details].OrderID 
INNER JOIN Products ON [Order Details].ProductID = Products.ProductID;

-- ============================================================================
-- 5. Orders Qry
-- ============================================================================
DROP VIEW IF EXISTS [Orders Qry];
CREATE VIEW [Orders Qry] AS
SELECT 
    Orders.OrderID,
    Orders.CustomerID,
    Orders.EmployeeID,
    Orders.OrderDate,
    Orders.RequiredDate,
    Orders.ShippedDate,
    Orders.ShipVia,
    Orders.Freight,
    Orders.ShipName,
    Orders.ShipAddress,
    Orders.ShipCity,
    Orders.ShipRegion,
    Orders.ShipPostalCode,
    Orders.ShipCountry,
    Customers.CompanyName,
    Customers.Address,
    Customers.City,
    Customers.Region,
    Customers.PostalCode,
    Customers.Country,
    (Employees.FirstName || ' ' || Employees.LastName) AS Salesman
FROM Employees 
INNER JOIN Orders ON Employees.EmployeeID = Orders.EmployeeID 
INNER JOIN Customers ON Orders.CustomerID = Customers.CustomerID;

-- ============================================================================
-- 6. Order Subtotals
-- ============================================================================
DROP VIEW IF EXISTS [Order Subtotals];
CREATE VIEW [Order Subtotals] AS
SELECT 
    [Order Details].OrderID,
    SUM(([Order Details].UnitPrice * [Order Details].Quantity * (1 - [Order Details].Discount))) AS Subtotal
FROM [Order Details]
GROUP BY [Order Details].OrderID;

-- ============================================================================
-- 7. Product Sales for 1997
-- ============================================================================
DROP VIEW IF EXISTS [Product Sales for 1997];
CREATE VIEW [Product Sales for 1997] AS
SELECT 
    Categories.CategoryName,
    Products.ProductName,
    SUM(CAST([Order Details].Quantity AS NUMERIC) * [Order Details].UnitPrice) AS ProductSales
FROM Categories 
INNER JOIN Products ON Categories.CategoryID = Products.CategoryID 
INNER JOIN [Order Details] ON Products.ProductID = [Order Details].ProductID 
INNER JOIN Orders ON [Order Details].OrderID = Orders.OrderID
WHERE STRFTIME('%Y', Orders.OrderDate) = '1997'
GROUP BY Categories.CategoryName, Products.ProductName;

-- ============================================================================
-- 8. Products Above Average Price
-- ============================================================================
DROP VIEW IF EXISTS [Products Above Average Price];
CREATE VIEW [Products Above Average Price] AS
SELECT 
    Products.ProductName,
    Products.UnitPrice
FROM Products
WHERE Products.UnitPrice > (
    SELECT AVG(UnitPrice)
    FROM Products
)
ORDER BY Products.UnitPrice DESC;

-- ============================================================================
-- 9. Products by Category
-- ============================================================================
DROP VIEW IF EXISTS [Products by Category];
CREATE VIEW [Products by Category] AS
SELECT 
    Categories.CategoryName,
    Products.ProductName,
    Products.QuantityPerUnit,
    Products.UnitsInStock,
    Products.Discontinued
FROM Categories 
INNER JOIN Products ON Categories.CategoryID = Products.CategoryID
WHERE Products.Discontinued = 0
ORDER BY Categories.CategoryName, Products.ProductName;

-- ============================================================================
-- 10. Quarterly Orders
-- ============================================================================
DROP VIEW IF EXISTS [Quarterly Orders];
CREATE VIEW [Quarterly Orders] AS
SELECT DISTINCT
    Customers.CustomerID,
    Customers.CompanyName,
    Customers.City,
    Customers.Country
FROM Customers 
INNER JOIN Orders ON Customers.CustomerID = Orders.CustomerID
WHERE Orders.OrderDate >= DATETIME('now', 'start of year', '+9 months');

-- ============================================================================
-- 11. Sales Totals by Amount
-- ============================================================================
DROP VIEW IF EXISTS [Sales Totals by Amount];
CREATE VIEW [Sales Totals by Amount] AS
SELECT 
    [Order Subtotals].OrderID,
    Orders.CustomerID,
    [Order Subtotals].Subtotal
FROM Orders 
INNER JOIN [Order Subtotals] ON Orders.OrderID = [Order Subtotals].OrderID
WHERE [Order Subtotals].Subtotal > 2500
ORDER BY [Order Subtotals].Subtotal DESC;

-- ============================================================================
-- 12. Summary of Sales by Quarter
-- ============================================================================
DROP VIEW IF EXISTS [Summary of Sales by Quarter];
CREATE VIEW [Summary of Sales by Quarter] AS
SELECT 
    CAST(STRFTIME('%Y', Orders.OrderDate) AS INTEGER) AS Year,
    CAST(((CAST(STRFTIME('%m', Orders.OrderDate) AS INTEGER) + 2) / 3) AS INTEGER) AS Quarter,
    SUM(([Order Details].UnitPrice * [Order Details].Quantity * (1 - [Order Details].Discount))) AS SaleAmount
FROM Orders 
INNER JOIN [Order Details] ON Orders.OrderID = [Order Details].OrderID
GROUP BY CAST(STRFTIME('%Y', Orders.OrderDate) AS INTEGER), CAST(((CAST(STRFTIME('%m', Orders.OrderDate) AS INTEGER) + 2) / 3) AS INTEGER)
ORDER BY Year, Quarter;

-- ============================================================================
-- 13. Summary of Sales by Year
-- ============================================================================
DROP VIEW IF EXISTS [Summary of Sales by Year];
CREATE VIEW [Summary of Sales by Year] AS
SELECT 
    CAST(STRFTIME('%Y', Orders.OrderDate) AS INTEGER) AS Year,
    SUM(([Order Details].UnitPrice * [Order Details].Quantity * (1 - [Order Details].Discount))) AS SaleAmount
FROM Orders 
INNER JOIN [Order Details] ON Orders.OrderID = [Order Details].OrderID
GROUP BY CAST(STRFTIME('%Y', Orders.OrderDate) AS INTEGER)
ORDER BY Year;

-- ============================================================================
-- 14. Category Sales for 1997
-- ============================================================================
DROP VIEW IF EXISTS [Category Sales for 1997];
CREATE VIEW [Category Sales for 1997] AS
SELECT 
    Categories.CategoryName,
    SUM(([Order Details].UnitPrice * [Order Details].Quantity * (1 - [Order Details].Discount))) AS CategorySales
FROM Categories 
INNER JOIN Products ON Categories.CategoryID = Products.CategoryID 
INNER JOIN [Order Details] ON Products.ProductID = [Order Details].ProductID 
INNER JOIN Orders ON [Order Details].OrderID = Orders.OrderID
WHERE STRFTIME('%Y', Orders.OrderDate) = '1997'
GROUP BY Categories.CategoryName;

-- ============================================================================
-- 15. Order Details Extended
-- ============================================================================
DROP VIEW IF EXISTS [Order Details Extended];
CREATE VIEW [Order Details Extended] AS
SELECT 
    [Order Details].OrderID,
    [Order Details].ProductID,
    Products.ProductName,
    [Order Details].UnitPrice,
    [Order Details].Quantity,
    [Order Details].Discount,
    ([Order Details].UnitPrice * [Order Details].Quantity * (1 - [Order Details].Discount)) AS ExtendedPrice
FROM Products 
INNER JOIN [Order Details] ON Products.ProductID = [Order Details].ProductID;

-- ============================================================================
-- 16. Sales by Category
-- ============================================================================
DROP VIEW IF EXISTS [Sales by Category];
CREATE VIEW [Sales by Category] AS
SELECT 
    Categories.CategoryID,
    Categories.CategoryName,
    Products.ProductName,
    SUM(CAST([Order Details].Quantity AS NUMERIC) * [Order Details].UnitPrice) AS ProductSales
FROM Categories 
INNER JOIN Products ON Categories.CategoryID = Products.CategoryID 
INNER JOIN [Order Details] ON Products.ProductID = [Order Details].ProductID 
INNER JOIN Orders ON [Order Details].OrderID = Orders.OrderID
GROUP BY Categories.CategoryID, Categories.CategoryName, Products.ProductName
ORDER BY Categories.CategoryName, Products.ProductName;

-- ============================================================================
-- End of Views Script
-- ============================================================================
