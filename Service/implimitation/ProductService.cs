using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MyApi.Data;
using MyApi.Dtos;
using MyApi.Entity;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    private readonly IDistributedCache _cache;

    public ProductService(IDistributedCache _c, AppDbContext db)
    {
        _db = db;
        _cache = _c;
    }

    public async Task<ProductDto?> CreateProductAsync(ProductCreateDto dto)
    {
        var product = new ProductEntity
        {
            Name = dto.Name,
            Price = dto.Price,
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            UserId = product.UserId
        };
    }

    public async Task<IEnumerable<ProductDto>> GetProductAsync(Page_SortDto p)
    {
        string key = $"products:page:{p.Page}:sort:{p.SortBy}";
        
        var cachedJson = await _cache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cachedJson))
        {
            return JsonSerializer.Deserialize<IEnumerable<ProductDto>>(cachedJson)!;
        }
        
        var query = _db.Products.AsQueryable();
        query = p.SortBy switch
        {
            SortBy.Name => query.OrderBy(x => x.Name),
            SortBy.Price => query.OrderBy(x => x.Price),
            _ => query.OrderBy(x => x.Id)
        };

        var result = await query
            .Skip((p.Page - 1) * 15)
            .Take(15)
            .Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                UserId = x.UserId
            }).ToListAsync();
        
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return result;
    }

    public async Task<ProductDto?> UpdateAsync(int Userid, int id,ProductUpdateDto dto)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == Userid);
        if (product == null) return null;
        
        product.Name = dto.Name;
        product.Price = dto.Price;
        
        await _db.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            UserId = product.UserId
        };
        
    }

    public async Task<(ProductDto?,string Dis)> DeleteAsync(int Userid, int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(n=>n.UserId==Userid && n.Id == id);
        if (product==null) return (null,"\"Product not found or does not belong to user\"");
        
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        
        return (new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            UserId = product.UserId
        },"user successfully deleted");
    }

}
