
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyApi.Data;
using MyApi.Dtos;
using MyApi.Entities;
using MyApi.Interfaces;

namespace MyApi.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IMemoryCache _cache;
        private static int _productsCacheVersion = 1;

        public ProductService(AppDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        private string MakeCacheKey(ProductQueryDto q)
        {
            var search = string.IsNullOrWhiteSpace(q.Search) ? "" : q.Search.Trim().ToLowerInvariant();
            var sortBy = (q.SortBy ?? "name").ToLowerInvariant();
            var sortDir = (q.SortDir ?? "asc").ToLowerInvariant();
            var page = Math.Max(1, q.Page);
            var pageSize = Math.Clamp(q.PageSize, 1, 100);
            var version = Volatile.Read(ref _productsCacheVersion);
            return $"products:v{version}:search={search}:sort={sortBy}:{sortDir}:page={page}:size={pageSize}";
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(ProductQueryDto query)
        {
            var key = MakeCacheKey(query);

            var items = await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                entry.SlidingExpiration = TimeSpan.FromMinutes(2);

                IQueryable<ProductEntity> q = _db.Products.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var term = query.Search.Trim();
  
                    q = q.Where(p => EF.Functions.Like(p.Name, $"%{term}%") ||
                                     EF.Functions.Like(p.Description, $"%{term}%"));
                }

                var sortBy = (query.SortBy ?? "name").ToLowerInvariant();
                var sortDir = (query.SortDir ?? "asc").ToLowerInvariant();

                q = (sortBy, sortDir) switch
                {
                    ("price", "desc") => q.OrderByDescending(p => p.Price),
                    ("price", _) => q.OrderBy(p => p.Price),
                    _ => sortDir == "desc" ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name)
                };

                var page = Math.Max(1, query.Page);
                var pageSize = Math.Clamp(query.PageSize, 1, 100);

                var itemsFromDb = await q
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDto(p.Id, p.Name, p.Price))
                    .ToListAsync();

                return itemsFromDb;
            });

            return items;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return p == null ? null : new ProductDto(p.Id, p.Name, p.Price);
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto, int sellerId)
        {
            var product = new ProductEntity
            {
                Name = dto.Name,
                Description = "", 
                Price = dto.Price,
                OwnerId = sellerId
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            
            Interlocked.Increment(ref _productsCacheVersion);

            return new ProductDto(product.Id, product.Name, product.Price);
        }

        public async Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto, int sellerId)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return null;
            if (product.OwnerId != sellerId) return null;

            product.Name = dto.Name;
            product.Price = dto.Price;
            await _db.SaveChangesAsync();

            Interlocked.Increment(ref _productsCacheVersion);

            return new ProductDto(product.Id, product.Name, product.Price);
        }

        public async Task<bool> DeleteAsync(int id, int sellerId)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return false;
            if (product.OwnerId != sellerId) return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            Interlocked.Increment(ref _productsCacheVersion);

            return true;
        }
    }
}
