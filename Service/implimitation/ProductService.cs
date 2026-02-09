using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;
using MyApi.Data;
using MyApi.Dtos;
using MyApi.Entity;

public class ProductService :  IProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db) => _db = db;

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
        var quary = _db.Products.AsQueryable();
        quary = p.SortBy switch
        {
            SortBy.Name => quary.OrderBy(p => p.Name),
            SortBy.Price => quary.OrderBy(p => p.Price),
            _ => quary.OrderBy(p => p.Id)
        };
        
        var res = await  quary
            .Skip((p.Page-1)*15)
            .Take(15)
            .Select(x=> new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                UserId = x.UserId
            }).ToListAsync();
        
        return res;
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
