using Ardalis.GuardClauses;

namespace MinhaApiComSQLite.Models;

public class Product
{
    private Product() { } // For EF Core

    public Product(string name, decimal price, int categoryId)
    {
        Name = Guard.Against.NullOrEmpty(name);
        Price = Guard.Against.NegativeOrZero(price);
        CategoryId = Guard.Against.Zero(categoryId);
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public void UpdatePrice(decimal newPrice)
    {
        var price = Guard.Against.NegativeOrZero(newPrice);
        Price = price;
    }
}
