using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Extensions;
using MinhaApiComSQLite.Services;

namespace MinhaApiComSQLite.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IValidator<CreateCategoryDto> _createCategoryValidator;
    private readonly IValidator<UpdateCategoryDto> _updateCategoryValidator;

    public CategoriesController(
        ICategoryService categoryService,
        IValidator<CreateCategoryDto> createCategoryValidator,
        IValidator<UpdateCategoryDto> updateCategoryValidator
    )
    {
        _categoryService = categoryService;
        _createCategoryValidator = createCategoryValidator;
        _updateCategoryValidator = updateCategoryValidator;
    }

    [HttpGet]
    public IEnumerable<CategoryDto> GetCategories()
    {
        return _categoryService.GetAll().Select(c => new CategoryDto { Id = c.Id, Name = c.Name });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        var dto = new CategoryDto { Id = category.Id, Name = category.Name };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
    {
        var validationResult = await _createCategoryValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var result = await _categoryService.CreateAsync(dto);
        if (result.IsError)
        {
            result.Errors.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var category = result.Value;
        var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name };

        return CreatedAtAction(nameof(GetCategory), new { id = categoryDto.Id }, categoryDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        var validationResult = await _updateCategoryValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var updated = await _categoryService.UpdateAsync(id, dto);
        if (!updated)
            return NotFound(new { error = "Category not found" });
        return NoContent();
    }
}
