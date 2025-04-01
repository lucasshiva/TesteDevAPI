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

    public ProductsControllerTest()
    {
        var validator = new CreateProductDtoValidator();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        IProductService productService = new ProductService(
            _mockProductRepository.Object,
            _mockCategoryRepository.Object
        );
        _controller = new ProductsController(productService, validator);
    }

    [Fact]
    public void GetAllProducts_ReturnsAllProducts()
    {
        // Arrange
        List<Product> fakeProducts =
        [
            new("Primeiro produto", 10M, 2)
            {
                Id = 1,
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
                new Product("A valid name", 10M, fakeCategory.Id)
                {
                    Id = productId,
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

        var fakeProduct = new Product(dto.Name, dto.Price, fakeCategory.Id)
        {
            Id = 1,
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
        var dto = new CreateProductDto
        {
            Name = name,
            Price = 10m,
            CategoryId = 1,
        };

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

    [Fact]
    public async Task CreateProduct_CapitalizesFirstLetterInProductName_WhenSavingProductInDatabase()
    {
        // Arrange
        var cat = new Category { Id = 1, Name = "Teste" };
        var dto = new CreateProductDto
        {
            Name = "lower case name",
            Price = 10M,
            CategoryId = cat.Id,
        };
        // This is the product the service passed to the repository.
        var productToSave = new Product("Lower case name", dto.Price, dto.CategoryId);
        var createdProduct = new Product("Lower case name", dto.Price, dto.CategoryId)
        {
            Id = 2,
            Category = cat,
        };
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(cat.Id)).ReturnsAsync(cat);
        _mockProductRepository.Setup(r => r.NameExists(It.IsAny<string>())).ReturnsAsync(false);

        // Since Moq uses reference, we need to match the value manually.
        _mockProductRepository
            .Setup(r => r.CreateAsync(It.Is<Product>(p => p.Name == productToSave.Name)))
            .ReturnsAsync(createdProduct);

        // Act
        var actionResult = await _controller.CreateProduct(dto);

        // Assert
        _mockProductRepository.Verify(r =>
            r.CreateAsync(It.Is<Product>(p => p.Name == productToSave.Name))
        );
        Assert.NotNull(actionResult);
        var objectResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.NotNull(objectResult.Value);
        var product = Assert.IsType<ProductDto>(objectResult.Value);
        Assert.NotNull(product.Category);
    }
}
