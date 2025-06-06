using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Servidor.Models;

namespace MYContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompras { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(ProductoSeeder.ObtenerProductos());
    }
}

public static class ProductoSeeder
{
    public static List<Producto> ObtenerProductos()
    {
        return new List<Producto>
        {
            new Producto {
                Id_producto = 1,
                Nombre = "Café Molido",
                Descripcion = "Café molido premium 250g",
                Precio = 5.99m,
                Stock = 50,
                ImagenUrl = "https://www.cursosbaristacafe.com.mx/storage/2024/01/Molido-de-Cafe-Medio.webp"
              },
            new Producto
            {
                Id_producto = 2,
                Nombre = "Té Verde",
                Descripcion = "Té verde en hojas 100g",
                Precio = 3.49m,
                Stock = 100,
                ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_623500-MLA46099507682_052021-O.webp"
            },
            new Producto
            {
                Id_producto = 3,
                Nombre = "Azúcar Orgánica",
                Descripcion = "Azúcar de caña orgánica 1kg",
                Precio = 2.75m,
                Stock = 75,
                ImagenUrl = "https://enindigo.com.ar/wp-content/uploads/2022/04/producto_02628_01-600x600.jpeg.webp"
            },
            new Producto
            {
                Id_producto = 4,
                Nombre = "Chocolate Amargo",
                Descripcion = "Tableta de chocolate 70% cacao",
                Precio = 4.25m,
                Stock = 40,
                ImagenUrl = "https://http2.mlstatic.com/D_Q_NP_704370-MLU71074143177_082023-O.webp"
            },
            new Producto
            {
                Id_producto = 5,
                Nombre = "Galletas Integrales",
                Descripcion = "Galletas de avena sin azúcar",
                Precio = 3.15m,
                Stock = 60,
                ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_976948-MLA78705567537_082024-O.webp"
            },
            new Producto
            {
                Id_producto = 6,
                Nombre = "Aceite de Oliva",
                Descripcion = "Aceite de oliva extra virgen 500ml",
                Precio = 6.99m,
                Stock = 30,
                ImagenUrl = "https://sakwinetravel.com/wp-content/uploads/2019/10/BA01_portada.webp"
            },
            new Producto
            {
                Id_producto = 7,
                Nombre = "Pasta Integral",
                Descripcion = "Fideos integrales 500g",
                Precio = 2.95m,
                Stock = 80,
                ImagenUrl = "https://hudamar.com.ar/wp-content/uploads/2023/10/FUSILLI-GRUN.webp"
            },
            new Producto
            {
                Id_producto = 8,
                Nombre = "Sal Rosada",
                Descripcion = "Sal rosada del Himalaya 250g",
                Precio = 1.99m,
                Stock = 90,
                ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_846436-MLA47804695685_102021-O.webp"
            },
            new Producto
            {
                Id_producto = 9,
                Nombre = "Mermelada de Frutilla",
                Descripcion = "Mermelada artesanal sin azúcar",
                Precio = 3.99m,
                Stock = 45,
                ImagenUrl = "https://arcorencasa.com/wp-content/uploads/2024/10/20241008-13218.webp"
            },
            new Producto
            {
                Id_producto = 10,
                Nombre = "Leche de Almendras",
                Descripcion = "Bebida vegetal sin lactosa 1L",
                Precio = 4.50m,
                Stock = 35,
                ImagenUrl = "https://aratiendas.com/wp-content/uploads/2024/04/BEBIDA-DE-ALMENDRAS-ALMOND-VALLEY-X-1000-ML.webp"
            }
        };
    }
}