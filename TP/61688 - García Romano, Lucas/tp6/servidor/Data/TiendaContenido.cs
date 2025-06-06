using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data;
//Base de Datos de la Tienda
public class TiendaContext : DbContext
{
    //Constructor el cuales recibe las opciciones de configuracion
    public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }
//Para las entidades de la Base de Datos
    public DbSet<Producto> Productos => Set<Producto>();
    //Compras y sus detalles
    public DbSet<Compra> Compras => Set<Compra>();
    //Detalles de la compra 
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();
//Configuracion de la Base de Datos
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
//Agrega los Productos y sus detalles
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Dell UltraSharp U2723QE", Descripcion = "Monitor 27' 4K UHD con colores precisos y USB-C. Ideal para trabajo profesional.", Precio = 10000, Stock = 15, ImagenUrl = "." },
            new Producto { Id = 2, Nombre = "LG 27GN950-B", Descripcion = "Monitor gaming 27' 4K a 144Hz con respuesta rápida para una experiencia fluida.", Precio = 9000, Stock = 12, ImagenUrl = "." },
            new Producto { Id = 3, Nombre = "ASUS ProArt PA278CV", Descripcion = "Pantalla 27' QHD para edición de fotos y videos con colores reales.", Precio = 8700, Stock = 10, ImagenUrl = "." },
            new Producto { Id = 4, Nombre = "Samsung Odyssey G7", Descripcion = "Monitor curvo 27' o 32' QHD a 240Hz, perfecto para gamers competitivos.", Precio = 15000, Stock = 8, ImagenUrl = "." },
            new Producto { Id = 5, Nombre = "BenQ EX3501R", Descripcion = "Pantalla curva 35' ultrapanorámica para productividad y entretenimiento.", Precio = 25000, Stock = 6, ImagenUrl = "." },
            new Producto { Id = 6, Nombre = "Acer Predator XB273K", Descripcion = "Monitor gaming 27' 4K a 144Hz con tecnología G-Sync para gráficos sin fallos.", Precio = 25000, Stock = 5, ImagenUrl = "." },
            new Producto { Id = 7, Nombre = "Eizo ColorEdge CG319X", Descripcion = "Pantalla profesional 31' 4K+ con calibración para edición de color precisa.", Precio = 12000, Stock = 2, ImagenUrl = "." },
            new Producto { Id = 8, Nombre = "ViewSonic VX2458-MHD", Descripcion = "Monitor 24' Full HD 75Hz, ideal para uso diario y gaming casual.", Precio = 15000, Stock = 14, ImagenUrl = "." },
            new Producto { Id = 9, Nombre = "HP Z27n G2", Descripcion = "Monitor 27' QHD con excelente reproducción de color para oficina y diseño.", Precio = 11000, Stock = 9, ImagenUrl = "." },
            new Producto { Id = 10, Nombre = "Dell S3422DWG", Descripcion = "Pantalla curva 34' WQHD a 144Hz para gaming y multitarea eficiente.", Precio = 7800, Stock = 4, ImagenUrl = "." }
        );
    }
}