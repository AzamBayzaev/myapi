using MyApi.Entities;

namespace MyApi.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserEntity user);
}