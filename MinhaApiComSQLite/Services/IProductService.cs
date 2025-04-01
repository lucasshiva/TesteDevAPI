using ErrorOr;
using MinhaApiComSQLite.Models;

namespace MinhaApiComSQLite.Services;

public interface IProductService
{
    public IEnumerable<Product> GetAll();
    public Task<Product?> GetByIdAsync(int id);
    public Task<ErrorOr<Product>> CreateAsync(Product product);
    public Task<bool> DeleteAsync(int id);
}
