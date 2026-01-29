using Microsoft.EntityFrameworkCore;
using MyApi.Dtos;
using MyApi.Entities;
using MyApi.Interfaces;
using MyApi.Data;
namespace MyApi.Services;
public class UserServiceEf : IUserService
{
    private readonly AppDbContext _db;
    public UserServiceEf(AppDbContext db) => _db = db;
    public async Task<UserEntity?> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return null;
        var user = new UserEntity
        {
            Name = dto.Name,
            Age = dto.Age,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.HasShopCertificate ? "seller" : "user"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }
    public async Task<UserEntity?> ValidateUserAsync(string email, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }
    public async Task<bool> SetRoleAsync(int userId, string role)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        user.Role = role;
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        return await _db.Users
            .Select(u => new UserResponseDto(u.Id, u.Name, u.Age, u.Email, u.Role))
            .ToListAsync();
    }
    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var u = await _db.Users.FindAsync(id);
        return u == null ? null : new UserResponseDto(u.Id, u.Name, u.Age, u.Email, u.Role);
    }
    public async Task<UserResponseDto?> CreateAsync(UserCreateDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email)) return null;

        var user = new UserEntity
        {
            Name = dto.Name,
            Age = dto.Age,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.HasShopCertificate ? "seller" : "user"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserResponseDto(user.Id, user.Name, user.Age, user.Email, user.Role);
    }
    public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return null;

        user.Name = dto.Name;
        user.Age = dto.Age;

        await _db.SaveChangesAsync();
        return new UserResponseDto(user.Id, user.Name, user.Age, user.Email, user.Role);
    }
    public async Task<UserResponseDto?> DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return null;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return new UserResponseDto(user.Id, user.Name, user.Age, user.Email, user.Role);
    }
}
