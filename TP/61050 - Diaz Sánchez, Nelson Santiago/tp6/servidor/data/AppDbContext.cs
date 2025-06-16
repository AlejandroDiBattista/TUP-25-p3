using System.Collections.Generic;  
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore;
using servidor.Entidades;
namespace servidor.Data {

public class AppDbContext : DbContext
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Productos.db");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
    new Producto { Id = 1, Nombre = "Heladera", Descripcion = "Heladera con freezer", Imagen = "https://assets.hotsale.com.ar/uploads/offers/465371/680fe0b634761.jpg?w=500&h=375", Precio = 150000.0, Cantidad = 10 },
    new Producto { Id = 2, Nombre = "Lavarropas", Descripcion = "Lavarropas automático carga frontal", Imagen = "https://authogar.vtexassets.com/arquivos/ids/198954-500-auto?v=638767894197430000&width=500&height=auto&aspect=true", Precio = 120000.0, Cantidad = 8 },
    new Producto { Id = 3, Nombre = "Microondas", Descripcion = "Microondas digital 20L", Imagen = "https://thumbs.dreamstime.com/b/abra-el-horno-de-microondas-84772112.jpg", Precio = 45000.0, Cantidad = 15 },
    new Producto { Id = 4, Nombre = "Aire Acondicionado", Descripcion = "Aire acondicionado split frío/calor", Imagen = "https://static.vecteezy.com/system/resources/thumbnails/026/484/369/small/white-air-conditioner-and-remote-isolated-on-white-wall-background-cooling-product-for-in-summer-clipping-path-free-photo.jpg", Precio = 180000.0, Cantidad = 6 },
    new Producto { Id = 5, Nombre = "Horno eléctrico", Descripcion = "Horno eléctrico 45 litros", Imagen = "https://http2.mlstatic.com/D_Q_NP_2X_697835-MLU74245658329_012024-E.webp", Precio = 60000.0, Cantidad = 12 },
    new Producto { Id = 6, Nombre = "Batidora", Descripcion = "Batidora de mano 5 velocidades", Imagen = "https://http2.mlstatic.com/D_Q_NP_2X_978238-MLU72674744248_112023-V.webp", Precio = 20000.0, Cantidad = 20 },
    new Producto { Id = 7, Nombre = "Licuadora", Descripcion = "Licuadora de vaso 1.5L", Imagen = "https://img.freepik.com/fotos-premium/licuadora-electrica_909293-2672.jpg?semt=ais_hybrid&w=740", Precio = 25000.0, Cantidad = 18 },
    new Producto { Id = 8, Nombre = "Cafetera", Descripcion = "Cafetera eléctrica 12 tazas", Imagen = "https://http2.mlstatic.com/D_Q_NP_2X_956659-MLA83391699886_042025-V.webp", Precio = 30000.0, Cantidad = 14 },
    new Producto { Id = 9, Nombre = "Plancha", Descripcion = "Plancha a vapor con suela antiadherente", Imagen = "https://http2.mlstatic.com/D_Q_NP_2X_889265-MLA84235372773_042025-V.webp", Precio = 18000.0, Cantidad = 25 },
    new Producto { Id = 10, Nombre = "Tostadora", Descripcion = "Tostadora 2 rebanadas con regulador", Imagen = "https://http2.mlstatic.com/D_Q_NP_2X_957292-MLA52160650303_102022-V.webp", Precio = 15000.0, Cantidad = 22 }
);

    }
}
}