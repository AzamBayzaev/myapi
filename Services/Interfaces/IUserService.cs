using MyApi.Dtos;
using MyApi.Entities;
namespace MyApi.Interfaces;
public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(int id);
    Task<UserResponseDto?> CreateAsync(UserCreateDto user);
    Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto user);
    Task<UserResponseDto?> DeleteAsync(int id);
    Task<UserEntity?> RegisterAsync(RegisterDto user);
    Task<UserEntity?> ValidateUserAsync(string email, string password);
}
