using MyApi.Dtos;

namespace MyApi.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(ProductCreateDto dto, int sellerId);
    Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto, int sellerId);
    Task<bool> DeleteAsync(int id, int sellerId);
}