using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyApi.Service.interfaces;
using MyApi.Entity;
using MyApi.Dtos;
using MyApi.Data;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }
    
    public async Task<(bool Success, string Error)> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(n => n.Email == dto.Email)) return (false, "user is already exist"); 
        
        var sertic = dto.HasSertificate;

        var role = "User";
        if(sertic) role = "Seller";

        var res = new UserEntity()
        {
            Nick = dto.Nick,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            HasSertificate = sertic,
            Role = role
        };
        
        _db.Users.Add(res);
        await _db.SaveChangesAsync();
        
        return (true, "register successful");

    }

    public async Task<(bool Success, string Token, string Error)> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(n=>n.Email == dto.Email);
        
        if(user == null) return (false,null,"Email is not exist");
        
        if(!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return (false,null,"Wrong password");
        
        var token = GenerateJwtToken(user);
        
        return (true,token,null);
        
    }

    private string GenerateJwtToken(UserEntity user)
    {
        var jwt = _config.GetSection("Jwt");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expire = DateTime.UtcNow.AddMinutes(Double.Parse(jwt["ExpiresMinutes"] ?? "60"));
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken
        (
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expire,
            signingCredentials: cred
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}