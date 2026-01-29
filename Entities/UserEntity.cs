namespace MyApi.Entities;

public class UserEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public int Age { get; set; }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "user";
}