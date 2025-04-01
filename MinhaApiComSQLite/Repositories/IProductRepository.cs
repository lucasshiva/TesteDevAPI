using MinhaApiComSQLite.Models;

namespace MinhaApiComSQLite.Repositories;

public interface IProductRepository
{
    public IEnumerable<Product> GetAll();
    public Task<Product?> GetByIdAsync(int id);
    public Task<Product> CreateAsync(Product product);
    public Task<bool> NameExists(string name);
    public Task DeleteAsync(Product product);
}
