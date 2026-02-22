using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Tests.Fixtures;

public static class ODataTestData
{
    public static void SeedTestCategories(NorthwindContext context)
    {
        var categories = new List<Category>
        {
            new() { CategoryId = 1, CategoryName = "Beverages", Description = "Soft drinks, coffees, teas" },
            new() { CategoryId = 2, CategoryName = "Condiments", Description = "Sweet and savory sauces" },
            new() { CategoryId = 3, CategoryName = "Confections", Description = "Desserts and candy" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();
    }

    public static void SeedTestCustomers(NorthwindContext context)
    {
        var customers = new List<Customer>
        {
            new() 
            { 
                CustomerId = "ALFKI", 
                CompanyName = "Alfreds Futterkiste", 
                ContactName = "Maria Anders",
                City = "Berlin"
            },
            new() 
            { 
                CustomerId = "BLONP", 
                CompanyName = "Blondesddsl", 
                ContactName = "Frédérique Citeaux",
                City = "Strasbourg"
            },
            new() 
            { 
                CustomerId = "BOLID", 
                CompanyName = "Bólido Comidas preparadas", 
                ContactName = "Martín Sommer",
                City = "Madrid"
            }
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();
    }

    public static void SeedTestProducts(NorthwindContext context)
    {
        var products = new List<Product>
        {
            new() 
            { 
                ProductId = 1, 
                ProductName = "Chai", 
                UnitPrice = 18.00m,
                UnitsInStock = 39
            },
            new() 
            { 
                ProductId = 2, 
                ProductName = "Chang", 
                UnitPrice = 19.00m,
                UnitsInStock = 17
            },
            new() 
            { 
                ProductId = 3, 
                ProductName = "Aniseed Syrup", 
                UnitPrice = 10.00m,
                UnitsInStock = 13
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }

    public static void SeedTestData(NorthwindContext context)
    {
        SeedTestCategories(context);
        SeedTestCustomers(context);
        SeedTestProducts(context);
    }
}
