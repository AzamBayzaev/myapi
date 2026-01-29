namespace MyApi.Dtos;

public record RegisterDto(
    string Name,
    int Age,
    string Email,
    string Password,
    bool HasShopCertificate = false
);