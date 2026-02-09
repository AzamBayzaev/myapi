using Microsoft.EntityFrameworkCore;
using MyApi.Entity;

namespace MyApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
}