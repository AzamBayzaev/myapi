namespace MyApi.Service.interfaces;
using MyApi.Dtos;
public interface IAuthService
{
    Task<(bool Success, string Error)> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string Token, string Error)> LoginAsync(LoginDto dto);
}