using Microsoft.AspNetCore.Mvc;
using MinhaApiComSQLite.Controllers;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Models;
using MinhaApiComSQLite.Repositories;
using MinhaApiComSQLite.Services;
using MinhaApiComSQLite.Validators;
using Moq;

namespace MinhaApiComSQLite.Tests;

public class CategoriesControllerTest
{
    private readonly CategoriesController _controller;
    private readonly List<Category> _dummyCategories;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;

    public CategoriesControllerTest()
    {
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        var createCategoryValidator = new CreateCategoryValidator();
        var updateCategoryValidator = new UpdateCategoryValidator();
        var service = new CategoryService(_mockCategoryRepo.Object);
        _controller = new CategoriesController(
            service,
            createCategoryValidator,
            updateCategoryValidator
        );
        _dummyCategories =
        [
            new Category { Id = 1, Name = "Teste" },
            new Category { Id = 2, Name = "My product" },
        ];
    }

    [Fact]
    public void GetCategories_ReturnsAllCategories()
    {
        _mockCategoryRepo.Setup(r => r.GetAll()).Returns(_dummyCategories);

        var categories = _controller.GetCategories();

        var categoryList = categories.ToList();
        Assert.Equal(2, categoryList.Count);
        Assert.Equal(1, categoryList.First().Id);
    }

    [Fact]
    public async Task GetCategory_ReturnsOkRequestWithExistingId()
    {
        var fakeCat = _dummyCategories.First();
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(fakeCat.Id)).ReturnsAsync(fakeCat);

        var actionResult = await _controller.GetCategory(fakeCat.Id);

        var okActionResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okActionResult.Value);
        var category = Assert.IsType<CategoryDto>(okActionResult.Value);
        Assert.NotNull(category);
        Assert.Equal(fakeCat.Id, category.Id);
    }

    [Fact]
    public async Task GetCategory_ReturnsNotFound_WhenCategoryDoesntExist()
    {
        var fakeCat = _dummyCategories.First();
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(fakeCat.Id)).ReturnsAsync(null as Category);

        var actionResult = await _controller.GetCategory(fakeCat.Id);

        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateCategory_ReturnsBadRequest_WhenNameIsInvalid(string name)
    {
        // Arrange
        var dto = new CreateCategoryDto { Name = name };

        // Act
        var actionResult = await _controller.CreateCategory(dto);

        // Assert
        var badRequestAction = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, badRequestAction.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_ReturnsBadRequest_WhenCategoryExists()
    {
        var fakeCat = _dummyCategories.First();
        var dto = new CreateCategoryDto { Name = fakeCat.Name };
        _mockCategoryRepo.Setup(r => r.NameExists(fakeCat.Name)).ReturnsAsync(true);

        var actionResult = await _controller.CreateCategory(dto);

        var badRequestAction = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.NotNull(badRequestAction.StatusCode);
        Assert.Equal(400, badRequestAction.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsNotFound_WhenIdDoesntExist()
    {
        const int invalidId = 5;
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(invalidId)).ReturnsAsync(null as Category);
        var validDto = new UpdateCategoryDto { Name = "A new name" };

        var actionResult = await _controller.UpdateCategory(invalidId, validDto);

        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        var fakeCat = _dummyCategories.First();
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(fakeCat.Id)).ReturnsAsync(fakeCat);

        var validDto = new UpdateCategoryDto { Name = "A new name" };
        var actionResult = await _controller.UpdateCategory(fakeCat.Id, validDto);

        _mockCategoryRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<Category>()));
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task UpdateCategory_ReturnsBadRequest_WithInvalidName(string name)
    {
        var fakeCat = _dummyCategories.First();
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(fakeCat.Id)).ReturnsAsync(fakeCat);

        var validDto = new UpdateCategoryDto { Name = name };
        var actionResult = await _controller.UpdateCategory(fakeCat.Id, validDto);

        Assert.IsType<BadRequestObjectResult>(actionResult);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesntExist()
    {
        const int invalidId = 5;
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(invalidId)).ReturnsAsync(null as Category);

        var actionResult = await _controller.DeleteCategory(invalidId);
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNoContent_WhenDeleteIsSuccessful()
    {
        var fakeCat = _dummyCategories.First();
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(fakeCat.Id)).ReturnsAsync(fakeCat);

        var actionResult = await _controller.DeleteCategory(fakeCat.Id);
        Assert.IsType<NoContentResult>(actionResult);
        _mockCategoryRepo.Verify(r => r.DeleteAsync(fakeCat));
    }
}
