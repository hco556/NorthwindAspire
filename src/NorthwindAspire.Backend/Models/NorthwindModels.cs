namespace NorthwindAspire.Backend.Models;

// Categories
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

// Customers
public class Customer
{
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

// Employees
public class Employee
{
    public int EmployeeId { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TitleOfCourtesy { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string HomePhone { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public string PhotoPath { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public int? ReportsTo { get; set; }
    public Employee? Manager { get; set; }
    public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<EmployeeTerritory> Territories { get; set; } = new List<EmployeeTerritory>();
}

// Order Details
public class OrderDetail
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

// Orders
public class Order
{
    public int OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public decimal Freight { get; set; }
    public string ShipName { get; set; } = string.Empty;
    public string ShipAddress { get; set; } = string.Empty;
    public string ShipCity { get; set; } = string.Empty;
    public string ShipRegion { get; set; } = string.Empty;
    public string ShipPostalCode { get; set; } = string.Empty;
    public string ShipCountry { get; set; } = string.Empty;
    public Customer Customer { get; set; } = null!;
    public Employee? Employee { get; set; }
    public Shipper? Shipper { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

// Products
public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public int QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public int UnitsOnOrder { get; set; }
    public int ReorderLevel { get; set; }
    public string Discontinued { get; set; } = string.Empty;
    public Supplier? Supplier { get; set; }
    public Category? Category { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

// Shippers
public class Shipper
{
    public int ShipperId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

// Suppliers
public class Supplier
{
    public int SupplierId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string HomePage { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

// Territories
public class Territory
{
    public string TerritoryId { get; set; } = string.Empty;
    public string TerritoryDescription { get; set; } = string.Empty;
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    public ICollection<EmployeeTerritory> Employees { get; set; } = new List<EmployeeTerritory>();
}

// Regions
public class Region
{
    public int RegionId { get; set; }
    public string RegionDescription { get; set; } = string.Empty;
    public ICollection<Territory> Territories { get; set; } = new List<Territory>();
}

// Employee Territories (Junction Table)
public class EmployeeTerritory
{
    public int EmployeeId { get; set; }
    public string TerritoryId { get; set; } = string.Empty;
    public Employee Employee { get; set; } = null!;
    public Territory Territory { get; set; } = null!;
}
