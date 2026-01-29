using MyApi.Dtos;
using MyApi.Interfaces;
namespace MyApi.Services;
public class ProductService : IProductService
{
    public Task<IEnumerable<ProductDto>> GetAllAsync()
        => Task.FromResult(Enumerable.Empty<ProductDto>());

    public Task<ProductDto?> GetByIdAsync(int id)
        => Task.FromResult<ProductDto?>(null);

    public Task<ProductDto> CreateAsync(ProductCreateDto dto, int sellerId)
        => Task.FromResult(new ProductDto(0, dto.Name, dto.Price));

    public Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto, int sellerId)
        => Task.FromResult<ProductDto?>(null);

    public Task<bool> DeleteAsync(int id, int sellerId)
        => Task.FromResult(false);
}