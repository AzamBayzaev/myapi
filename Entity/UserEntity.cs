namespace MyApi.Entity;

public class UserEntity
{
    public int Id { get; set; }             
    public string Nick { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } 
    public bool HasSertificate { get; set; }
    public string Role { get; set; }

    public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
}