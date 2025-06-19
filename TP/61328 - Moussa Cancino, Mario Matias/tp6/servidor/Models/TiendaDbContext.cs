using Microsoft.EntityFrameworkCore;
namespace servidor.Modelos;

public class TiendaDbContext : DbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    // Quitar esta línea que está causando problemas:
    // public DbSet<ItemCompra> ItemsDeCompra { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar que ItemCompra sea una entidad owned por Compra
        modelBuilder.Entity<Compra>()
            .OwnsMany(c => c.Items, item =>
            {
                item.WithOwner().HasForeignKey("CompraId");
                item.Property<int>("Id");
                item.HasKey("Id");
            });

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Galaxy S23", Descripcion = "El flagship de Samsung con IA.", Precio = 950.00m, Stock = 15, ImagenUrl = "images/S24-3.jpg" },
            new Producto { Id = 2, Nombre = "iPhone 15 Pro", Descripcion = "Titanio. Potencia. Pro.", Precio = 1200.00m, Stock = 10, ImagenUrl = "images/15pro-jpeg.jpg" },
            new Producto { Id = 3, Nombre = "iPhone 16 Pro", Descripcion = "La magia de Google en tu mano.", Precio = 800.00m, Stock = 20, ImagenUrl = "images/16pro.jpg" },
            new Producto { Id = 4, Nombre = "Xiaomi 13T", Descripcion = "Cámara Leica, rendimiento superior.", Precio = 700.00m, Stock = 25, ImagenUrl = "images/xiaomi13t.jpg" },
            new Producto { Id = 5, Nombre = "Cargador Rápido 65W", Descripcion = "Carga universal para todos tus dispositivos.", Precio = 45.50m, Stock = 50, ImagenUrl = "images/cargador65.jpg" },
            new Producto { Id = 6, Nombre = "Auriculares Inalámbricos Pro", Descripcion = "Cancelación de ruido activa y sonido Hi-Fi.", Precio = 150.00m, Stock = 30, ImagenUrl = "images/airpods.jpg" },
            new Producto { Id = 7, Nombre = "Aplle Watch", Descripcion = "Monitorea tu salud y actividad física.", Precio = 199.99m, Stock = 40, ImagenUrl = "images/smartwatch.jpg" },
            new Producto { Id = 8, Nombre = "Apple Pencil", Descripcion = "simplifica tus tareas.", Precio = 25.00m, Stock = 100, ImagenUrl = "images/pencilapple.jpg" },
            new Producto { Id = 9, Nombre = "Cover Silicona", Descripcion = "Máxima resistencia contra golpes y rayones.", Precio = 15.00m, Stock = 200, ImagenUrl = "images/cover.jpg" },
            new Producto { Id = 10, Nombre = "Power Bank 20000mAh", Descripcion = "Batería externa para múltiples cargas.", Precio = 60.00m, Stock = 35, ImagenUrl = "images/powerbank.jpg" }
        );
    }
}