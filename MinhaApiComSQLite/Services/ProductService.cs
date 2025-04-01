using ErrorOr;
using MinhaApiComSQLite.Extensions;
using MinhaApiComSQLite.Models;
using MinhaApiComSQLite.Repositories;

namespace MinhaApiComSQLite.Services;

public class ProductService : IProductService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository
    )
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public IEnumerable<Product> GetAll()
    {
        return _productRepository.GetAll();
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        return _productRepository.GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return false;
        await _productRepository.DeleteAsync(product);
        return true;
    }

    public async Task<ErrorOr<Product>> CreateAsync(Product product)
    {
        List<Error> errors = [];

        product.Name = product.Name.CapitalizeFirstLetter();

        if (await _productRepository.NameExists(product.Name))
            errors.Add(Error.Validation(description: "Product name must be unique"));

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        if (category == null)
            errors.Add(Error.Validation(description: "Category doesn't exist"));

        if (errors.Count > 0)
            return errors;

        try
        {
            return await _productRepository.CreateAsync(product);
        }
        catch (Exception e)
        {
            errors.Add(Error.Unexpected(description: e.Message));
            return errors;
        }
    }
}
