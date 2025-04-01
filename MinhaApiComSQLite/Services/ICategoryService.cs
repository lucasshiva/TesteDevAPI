using ErrorOr;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Models;

namespace MinhaApiComSQLite.Services;

public interface ICategoryService
{
    public IEnumerable<Category> GetAll();
    public Task<ErrorOr<Category>> CreateAsync(CreateCategoryDto dto);
    public Task<Category?> GetByIdAsync(int id);
    public Task<bool> UpdateAsync(int id, UpdateCategoryDto dto);
    public Task<bool> DeleteAsync(int id);
}
