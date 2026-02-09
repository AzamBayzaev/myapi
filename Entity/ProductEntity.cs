namespace MyApi.Entity;

public class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }
}