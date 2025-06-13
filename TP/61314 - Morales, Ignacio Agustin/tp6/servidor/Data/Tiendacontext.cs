using Microsoft.EntityFrameworkCore;

public class TiendaDb : DbContext
{
    public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCarrito> ItemsCarrito => Set<ItemCarrito>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Productos predefinidos
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Iphone 15", Descripcion = "Celular con la mejor autonomía", Stock = 10, Precio = 780000, ImagenUrl = "imagenes/iphone15.webp" },
            new Producto { Id = 2, Nombre = "Iphone 15 Plus", Descripcion = "Celular con batería resistente", Stock = 5, Precio = 820000, ImagenUrl = "imagenes/iphone15plus.webp" },
            new Producto { Id = 3, Nombre = "Iphone 16", Descripcion = "Celular de última generación", Stock = 13, Precio = 980000, ImagenUrl = "imagenes/iphone16.webp" },
            new Producto { Id = 4, Nombre = "Iphone 16 Pro MAX", Descripcion = "Celular más demandado del mercado", Stock = 6, Precio = 120000000, ImagenUrl = "imagenes/iphone16promax.webp" },
            new Producto { Id = 5, Nombre = "AirPods Pro", Descripcion = "Auriculares bluetooth", Stock = 20, Precio = 200000, ImagenUrl = "imagenes/airpodspro.webp" },
            new Producto { Id = 6, Nombre = "MacBook Air", Descripcion = "Notebook eficaz", Stock = 12, Precio = 220000000, ImagenUrl = "imagenes/macbookair.webp" },
            new Producto { Id = 7, Nombre = "MacBook Pro", Descripcion = "Notebook más potente", Stock = 8, Precio = 390000000, ImagenUrl = "imagenes/macbookpro.webp" },
            new Producto { Id = 8, Nombre = "Apple Watch", Descripcion = "Reloj inteligente", Stock = 15, Precio = 340000, ImagenUrl = "imagenes/applewatch.webp" },
            new Producto { Id = 9, Nombre = "Ipad Air", Descripcion = "Pantalla inteligente", Stock = 11, Precio = 720000, ImagenUrl = "imagenes/ipadair.webp" },
            new Producto { Id = 10, Nombre = "Ipad Pro", Descripcion = "Ipad más potente y avanzado", Stock = 9, Precio = 130000000, ImagenUrl = "imagenes/ipadpro.webp" }
        );

        modelBuilder.Entity<ItemCarrito>()
            .HasOne(i => i.Producto)
            .WithMany(p => p.ItemsCarrito)
            .HasForeignKey(i => i.ProductoId);

        modelBuilder.Entity<ItemCarrito>()
            .HasOne(i => i.Carrito)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CarritoId);

        modelBuilder.Entity<ItemCompra>()
            .HasOne(i => i.Compra)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CompraId);

        modelBuilder.Entity<ItemCompra>()
            .HasOne<Producto>()
            .WithMany()
            .HasForeignKey(i => i.ProductoId);
    }
      }
