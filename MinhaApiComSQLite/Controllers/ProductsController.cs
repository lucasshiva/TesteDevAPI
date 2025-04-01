using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Extensions;
using MinhaApiComSQLite.Models;
using MinhaApiComSQLite.Services;

namespace MinhaApiComSQLite.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductDto> _validator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductDto> validator
    )
    {
        _productService = productService;
        _validator = validator;
    }

    [HttpGet]
    public IEnumerable<ProductDto> GetProducts()
    {
        var products = _productService
            .GetAll()
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = new CategoryDto { Id = p.Category.Id, Name = p.Category.Name },
            });
        return products;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = new CategoryDto { Id = product.Category.Id, Name = product.Category.Name },
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
    {
        var validationResult = await _validator.ValidateAsync(createProductDto);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        var product = new Product(
            createProductDto.Name,
            createProductDto.Price,
            createProductDto.CategoryId
        );

        var result = await _productService.CreateAsync(product);
        if (result.IsError)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);

            return BadRequest(ModelState);
        }

        var createdProduct = result.Value;

        var productDto = new ProductDto
        {
            Id = createdProduct.Id,
            Name = createdProduct.Name,
            Price = createdProduct.Price,
            Category = new CategoryDto
            {
                Id = createdProduct.Category.Id,
                Name = createdProduct.Category.Name,
            },
        };
        return CreatedAtAction(nameof(GetProduct), new { id = productDto.Id }, productDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
