using MyApi.Dtos;

public interface IProductService
{
    Task<ProductDto?> CreateProductAsync(ProductCreateDto dto);
    Task<IEnumerable<ProductDto>> GetProductAsync(Page_SortDto p);
    Task<ProductDto?> UpdateAsync(int Userid, int id, ProductUpdateDto dto);
    Task<(ProductDto?,string Dis)>DeleteAsync(int Userid,int id);
}