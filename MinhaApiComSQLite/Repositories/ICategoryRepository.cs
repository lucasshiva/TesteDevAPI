using MinhaApiComSQLite.Models;

namespace MinhaApiComSQLite.Repositories;

public interface ICategoryRepository
{
    public IEnumerable<Category> GetAll();
    public Task<Category?> GetByIdAsync(int id);
    public Task<Category> CreateAsync(Category category);
    public Task<bool> NameExists(string name);
    public Task UpdateAsync(Category existing, Category updated);
    public Task DeleteAsync(Category category);
}
