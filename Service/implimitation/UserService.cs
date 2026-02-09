using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;
using MyApi.Data;
using MyApi.Dtos;
using MyApi.Entity;
using MyApi.Service.interfaces;

namespace MyApi.Service.implimitation;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    
    public UserService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProductDto>> GetProducts(Page_SortDto sort)
    {
        var quary = _db.Products.AsQueryable();

        quary = sort.SortBy switch
        {
            SortBy.Name => quary.OrderBy(p => p.Name),
            SortBy.Price => quary.OrderBy(p => p.Price),
            _ => quary.OrderBy(p => p.Id)
        };
        
        return await quary
            .Skip((sort.Page-1)*15)
            .Take(15)
            .Select( p=> new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    UserId =  p.UserId
                })
            .ToListAsync();
        
        
    }

    public async Task<IEnumerable<UserDto>> GetUsers(Page_SortDto sort)
    {
        var quary = _db.Users.AsQueryable();

        quary = sort.SortBy switch
        {
            SortBy.Name => quary.OrderBy(p => p.Nick),
            SortBy.Email => quary.OrderBy(p => p.Email),
            _ => quary.OrderBy(p => p.Id)
        };
        
        return await quary
            .Skip((sort.Page-1)*15)
            .Take(15)
            .Select( p=> new UserDto
            {
                Id = p.Id,
                Nick =  p.Nick,
                Email =  p.Email,
                Role =  p.Role,
                HasSertificate =  p.HasSertificate
            })
            .ToListAsync();
        
        
    }
    
}