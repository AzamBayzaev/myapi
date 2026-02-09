namespace MyApi.Dtos;

public class UserDto
{
    public int Id { get; set; }             
    public string Nick { get; set; }
    public string Email { get; set; }
    public bool HasSertificate { get; set; }
    public string Role { get; set; }
}