using Microsoft.EntityFrameworkCore;
using MinhaApiComSQLite.Data;
using MinhaApiComSQLite.Models;

namespace MinhaApiComSQLite.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetAll()
    {
        return _context.Categories.ToList();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        var result = await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public Task<bool> NameExists(string name)
    {
        return _context.Categories.AnyAsync(c => c.Name == name);
    }

    public async Task UpdateAsync(Category existing, Category updated)
    {
        existing.Name = updated.Name;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
