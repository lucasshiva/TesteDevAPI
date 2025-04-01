using ErrorOr;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Models;
using MinhaApiComSQLite.Repositories;

namespace MinhaApiComSQLite.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public IEnumerable<Category> GetAll()
    {
        return _categoryRepository.GetAll();
    }

    public async Task<ErrorOr<Category>> CreateAsync(CreateCategoryDto dto)
    {
        List<Error> errors = [];
        var exists = await _categoryRepository.NameExists(dto.Name);
        if (exists)
            errors.Add(Error.Conflict(description: "Category already exists"));

        if (errors.Count > 0)
            return errors;

        return await _categoryRepository.CreateAsync(new Category { Name = dto.Name });
    }

    public Task<Category?> GetByIdAsync(int id)
    {
        return _categoryRepository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var existing = await _categoryRepository.GetByIdAsync(id);
        if (existing == null)
            return false;
        var updated = new Category { Name = dto.Name };
        await _categoryRepository.UpdateAsync(existing, updated);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return false;
        await _categoryRepository.DeleteAsync(category);
        return true;
    }
}
