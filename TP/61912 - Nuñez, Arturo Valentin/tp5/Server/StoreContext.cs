using Microsoft.EntityFrameworkCore;

public class StoreContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public StoreContext(DbContextOptions<StoreContext> options) : base(options)
    {
    }
} 