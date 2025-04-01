using Microsoft.AspNetCore.Mvc;
using MinhaApiComSQLite.Controllers;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Models;
using MinhaApiComSQLite.Repositories;
using MinhaApiComSQLite.Services;
using MinhaApiComSQLite.Validators;
using Moq;

namespace MinhaApiComSQLite.Tests;

public class ProductsControllerTest
{
    private readonly ProductsController _controller;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly IProductService _productService;

    public ProductsControllerTest()
    {
        var validator = new CreateProductDtoValidator();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _productService = new ProductService(
            _mockProductRepository.Object,
            _mockCategoryRepository.Object
        );
        _controller = new ProductsController(_productService, validator);
    }

    [Fact]
    public void GetAllProducts_ReturnsAllProducts()
    {
        // Arrange
        List<Product> fakeProducts =
        [
            new()
            {
                Id = 1,
                Name = "Primeiro produto",
                Price = 10M,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Technology" },
            },
        ];
        _mockProductRepository.Setup(r => r.GetAll()).Returns(fakeProducts);

        // Act
        var products = _controller.GetProducts().ToList();

        // Assert
        _mockProductRepository.Verify(r => r.GetAll());
        Assert.Single(products);
        Assert.Equal(1, products.First().Id);
        Assert.Equal(2, products.First().Category.Id);
        Assert.Equal("Technology", products.First().Category.Name);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFoundAction_IfProductDoesntExist()
    {
        // Arrange
        const int productId = 1;
        _mockProductRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(null as Product);

        // Act
        var actionResult = await _controller.GetProduct(productId);

        // Assert
        _mockProductRepository.Verify(r => r.GetByIdAsync(productId));
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public async Task GetProduct_ReturnsCorrectProduct_WhenGivenAnExistingId()
    {
        // Arrange
        const int productId = 1;
        var fakeCategory = new Category { Id = 3, Name = "Random name" };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(fakeCategory.Id))
            .ReturnsAsync(fakeCategory);
        _mockProductRepository
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(
                new Product
                {
                    Id = productId,
                    Name = "A valid name",
                    Price = 10m,
                    CategoryId = fakeCategory.Id,
                    Category = fakeCategory,
                }
            );

        // Act
        var actionResult = await _controller.GetProduct(productId);

        // Assert
        var okActionResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okActionResult);
        Assert.NotNull(okActionResult.Value);

        var product = Assert.IsType<ProductDto>(okActionResult.Value);
        Assert.NotNull(product);
        Assert.Equal(productId, product.Id);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedAtAction_WithValidDto()
    {
        // Arrange
        var fakeCategory = new Category { Id = 2, Name = "Technology" };
        var dto = new CreateProductDto
        {
            Name = "Product 1",
            Price = 10m,
            CategoryId = fakeCategory.Id,
        };

        var fakeProduct = new Product
        {
            Id = 1,
            Name = dto.Name,
            Price = dto.Price,
            CategoryId = fakeCategory.Id,
            Category = fakeCategory,
        };

        _mockCategoryRepository
            .Setup(r => r.GetByIdAsync(fakeCategory.Id))
            .ReturnsAsync(fakeCategory);

        _mockProductRepository
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(fakeProduct);

        // Act
        var actionResult = await _controller.CreateProduct(dto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        var createdProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);
        Assert.NotNull(createdProduct.Category);

        Assert.Equal(fakeProduct.Id, createdProduct.Id);
        Assert.Equal(fakeProduct.Price, createdProduct.Price);
        Assert.Equal(dto.Name, createdProduct.Name);
        Assert.Equal(fakeCategory.Id, createdProduct.Category.Id);
        Assert.Equal(fakeCategory.Name, createdProduct.Category.Name);
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WhenCategoryIsMissing()
    {
        var dto = new CreateProductDto { Name = "A valid name", Price = 10M };

        var actionResult = await _controller.CreateProduct(dto);

        var badRequestAction = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, badRequestAction.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateProduct_ReturnsBadRequest_WhenNameIsInvalid(string name)
    {
        // Arrange
        var dto = new CreateProductDto { Name = name, Price = 10m };

        // Act
        var actionResult = await _controller.CreateProduct(dto);

        // Assert
        var badRequestAction = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, badRequestAction.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WhenNameStartsWithLowercase()
    {
        // Arrange
        var dto = new CreateProductDto { Name = "invalid name", Price = 10m };
        var product = new Product
        {
            Id = 1,
            Name = dto.Name,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
        };

        // List<Error> errors = [Error.Validation()];
        // _mockProductRepository.Setup(s => s.CreateAsync(product)).ReturnsAsync(errors);

        // Act
        var actionResult = await _controller.CreateProduct(dto);

        // Assert
        var badRequestAction = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, badRequestAction.StatusCode);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-5.0)]
    public async Task CreateProduct_ReturnsBadRequest_WhenPriceIsZeroOrLess(decimal invalidPrice)
    {
        // Arrange
        var dto = new CreateProductDto { Name = "A valid product name", Price = invalidPrice };
        // List<Error> errors = [Error.Validation()];
        // _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(errors);

        // Act
        var actionResult = await _controller.CreateProduct(dto);

        // Assert
        var badRequestAction = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, badRequestAction.StatusCode);
    }
}
