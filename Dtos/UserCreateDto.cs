namespace MyApi.Dtos;
public record UserCreateDto(
    string Name,
    int Age,
    string Email,
    string Password,
    bool HasShopCertificate
);