using Microsoft.EntityFrameworkCore;
using TiendaOnline.Api.Models;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Api.Data;
using TiendaOnline.Api.Models;



namespace TiendaOnline.Api.Models;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}
namespace TiendaOnline.Api.Models;

public class Compra
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string ApellidoCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;

    public List<ItemCompra> Items { get; set; } = new();
}

namespace TiendaOnline.Api.Models;

public class ItemCompra
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
 
namespace TiendaOnline.Api.Data;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "iPhone 12", Descripcion = "6.1\" Super Retina XDR, A14 Bionic", Precio = 250000, Stock = 10, ImagenUrl = "img/iphone12.jpg" },
            new Producto { Id = 2, Nombre = "iPhone 13", Descripcion = "6.1\" Super Retina XDR, A15 Bionic", Precio = 300000, Stock = 8, ImagenUrl = "img/iphone13.jpg" },
            new Producto { Id = 3, Nombre = "iPhone 13 Pro", Descripcion = "Pantalla ProMotion 120Hz", Precio = 400000, Stock = 5, ImagenUrl = "img/iphone13pro.jpg" },
            new Producto { Id = 4, Nombre = "iPhone 14", Descripcion = "Cámara mejorada, chip A15", Precio = 350000, Stock = 7, ImagenUrl = "img/iphone14.jpg" },
            new Producto { Id = 5, Nombre = "iPhone 14 Pro", Descripcion = "Dynamic Island, A16 Bionic", Precio = 450000, Stock = 6, ImagenUrl = "img/iphone14pro.jpg" },
            new Producto { Id = 6, Nombre = "iPhone SE (2022)", Descripcion = "Compacto, chip A15", Precio = 220000, Stock = 10, ImagenUrl = "img/iphonese.jpg" },
            new Producto { Id = 7, Nombre = "iPhone 15", Descripcion = "USB-C, cámara 48MP", Precio = 500000, Stock = 9, ImagenUrl = "img/iphone15.jpg" },
            new Producto { Id = 8, Nombre = "iPhone 15 Plus", Descripcion = "Pantalla 6.7\"", Precio = 550000, Stock = 5, ImagenUrl = "img/iphone15plus.jpg" },
            new Producto { Id = 9, Nombre = "iPhone 15 Pro", Descripcion = "Titanio, A17 Pro", Precio = 600000, Stock = 4, ImagenUrl = "img/iphone15pro.jpg" },
            new Producto { Id = 10, Nombre = "iPhone 15 Pro Max", Descripcion = "Zoom óptico 5x", Precio = 650000, Stock = 3, ImagenUrl = "img/iphone15promax.jpg" }
        );
    }
}