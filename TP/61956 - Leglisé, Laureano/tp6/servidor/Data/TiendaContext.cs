using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) {}

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Iphone 14 Pro Max", Descripcion = "Smartphone gama alta", Precio = 150000, Stock = 10, ImagenUrl = "" },
            new Producto { Id = 2, Nombre = "Airpods Pro 2", Descripcion = "Bluetooth", Precio = 25000, Stock = 15, ImagenUrl = "" },
            new Producto { Id = 3, Nombre = "Macbook Pro 16", Descripcion = "Laptop gama alta", Precio = 300000, Stock = 5, ImagenUrl = "" }
        );
    }
}
