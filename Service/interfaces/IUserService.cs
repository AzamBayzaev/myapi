using MyApi.Dtos;

namespace MyApi.Service.interfaces;

public interface IUserService
{
    Task<IEnumerable<ProductDto>> GetProducts(Page_SortDto sort);
    Task<IEnumerable<UserDto>> GetUsers(Page_SortDto sort);
}