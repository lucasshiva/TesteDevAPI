namespace MinhaApiComSQLite.Models;

public class Product
{
    public int Id { get; set; }

    public required string Name { get; set; } = string.Empty;

    public required decimal Price { get; set; }

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = null!;
}
