using Microsoft.EntityFrameworkCore;
using MinhaApiComSQLite.Data;
using MinhaApiComSQLite.Models;

namespace MinhaApiComSQLite.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetAll()
    {
        return _context.Products.Include(p => p.Category).ToList();
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        return _context.Products.SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        var result = _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<bool> NameExists(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name);
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
