using Microsoft.EntityFrameworkCore;

namespace servidor.Models;

public class TiendaContext : DbContext
{
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<CarritoItem> CarritoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Iphone 15 Pro Max", Descripcion = "Celular Ultima Generacion", Precio = 2000000, Stock = 10, ImagenUrl = "https://i5.walmartimages.com/seo/Restored-Apple-iPhone-15-Pro-Max-256GB-Unlocked-Blue-Titanium-MU693LL-A-Excellent-Condition_dd2d42c6-cc25-4bee-81ef-7847120498d5.663475b807d168a41e9082d258d9c7ce.jpeg" },
            new Producto { Id = 2, Nombre = "Notebook Gamer ASUS Rog Strix G17", Descripcion = "Notebook Profesional y Gaming", Precio = 3465000, Stock = 5, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/156/703/products/notebook-gamer-asus-rog-strix-g17-g713pv-ws94-17-ryzen-9-7845hx-1tb-ssd-16gb-rtx-4060-copia-4a3f8882e47fbcca8317314309110054-1024-1024.png" },
            new Producto { Id = 3, Nombre = "Audio Technica ATH-M50X", Descripcion = "Auriculares para Audio Profesional", Precio = 300000, Stock = 15, ImagenUrl = "https://m.media-amazon.com/images/I/71BR7ivLOAL.jpg" },
            new Producto { Id = 4, Nombre = "Logitech G203", Descripcion = "Mouse con luces RGB", Precio = 15000, Stock = 20, ImagenUrl = "https://spacegamer.com.ar/img/Public/1058-producto-203bb-8934.jpg" },
            new Producto { Id = 5, Nombre = "HyperX Alloy Origins", Descripcion = "Teclado con switches mecánicos", Precio = 135000, Stock = 8, ImagenUrl = "https://www.newmaster.com.ar/wp-content/uploads/2021/08/1-1.jpg" },
            new Producto { Id = 6, Nombre = "Apple Watch SE", Descripcion = "Reloj inteligente", Precio = 620000, Stock = 12, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/097/819/products/apple-watch-serie-se-44mm1-ef69a751ed99e1572d16758829200917-1024-1024.png" },
            new Producto { Id = 7, Nombre = "Samgung Galaxy Tab A9", Descripcion = "Pantalla HD", Precio = 235000, Stock = 6, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_927502-MLU75081110091_032024-O.webp" },
            new Producto { Id = 8, Nombre = "Parlante JBL Go", Descripcion = "Sonido potente", Precio = 60000, Stock = 18, ImagenUrl = "https://www.oscarbarbieri.com/pub/media/catalog/product/cache/7baadf0dec41407c7702efdbff940ecb/4/4/44a9f81c092a58e6af72a2b902f8c330.jpg" },
            new Producto { Id = 9, Nombre = "Disco SSD WD Green 480 GB", Descripcion = "Disco super rápido", Precio = 29000, Stock = 25, ImagenUrl = "https://compucordoba.com.ar/img/Public/1078-producto-d-nq-np-699067-mla31583397158-072019-o1-614.jpg" },
            new Producto { Id = 10, Nombre = "Pendrive Kingston 64GB", Descripcion = "USB 3.1", Precio = 9000, Stock = 30, ImagenUrl = "https://www.torca.com.ar/images/00000000000622444173364.jpg" }
        );
    }
}
