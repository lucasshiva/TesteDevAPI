using ErrorOr;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Models;

namespace MinhaApiComSQLite.Services;

public interface IProductService
{
    public IEnumerable<Product> GetAll();
    public Task<Product?> GetByIdAsync(int id);
    public Task<ErrorOr<Product>> CreateAsync(CreateProductDto dto);
    public Task<bool> DeleteAsync(int id);
}
