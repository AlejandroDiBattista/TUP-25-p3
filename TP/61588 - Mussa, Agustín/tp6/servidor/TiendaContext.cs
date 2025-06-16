using Microsoft.EntityFrameworkCore;
using servidor.Modelos;
using System.Collections.Generic;
using System;

namespace servidor
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<Compra> Compras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemCarrito>()
                .HasKey(ic => ic.Id);

            modelBuilder.Entity<ItemCarrito>()
                .HasOne<Carrito>()
                .WithMany(c => c.Items)
                .HasForeignKey(ic => ic.CarritoId);

            modelBuilder.Entity<ItemCompra>()
                .HasKey(ic => ic.Id);

            modelBuilder.Entity<ItemCompra>()
                .HasOne<Compra>()
                .WithMany()
                .HasForeignKey(ic => ic.CompraId);
        }

        public static void SeedDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
                db.Database.Migrate();

                if (!db.Productos.Any())
                {
                    db.Productos.AddRange(new List<Producto>
                    {
                        new Producto { Nombre = "Producto 1", Descripcion = "Silla Gamer Vertagear SL5800 HygennX Carbon Black Ergonomic", Precio = 497.596M, Stock = 10, ImagenUrl = "/img/Captura1.png" },
                        new Producto { Nombre = "Producto 2", Descripcion = "Mother Asrock B660M-HDV S1700 DDR4", Precio = 900.000M, Stock = 5, ImagenUrl = "/img/Captura2.png" },
                        new Producto { Nombre = "Producto 3", Descripcion = "Monitor Lenovo ThinkVision S22i-30 21.5 FHD IPS 75Hz Anti Glare VESA", Precio = 149.999M, Stock = 5, ImagenUrl = "/img/Captura3.png" },
                        new Producto { Nombre = "Producto 4", Descripcion = "Procesador Intel Core i9 14900K 6.0GHz Turbo Socket 1700 Raptor Lake", Precio = 673.665M, Stock = 0, ImagenUrl = "/img/Captura4.png" },
                        new Producto { Nombre = "Producto 5", Descripcion = "Fuente Be Quiet 1000W 80 Plus Platinum STRAIGHT POWER 12 Full Modular ATX 3.1 PCIe 5.1", Precio = 306.450M, Stock = 20, ImagenUrl = "/img/Captura5.png" },
                        new Producto { Nombre = "Producto 6", Descripcion = "Memoria Corsair DDR5 64GB (2x32GB) 6000MHz Dominator Titanium RGB CL30 Intel XMP3.0/AMD EXPO ICUE", Precio = 433.799M, Stock = 15, ImagenUrl = "/img/Captura6.png" },
                        new Producto { Nombre = "Producto 7", Descripcion = "Teclado Hipermagnético Steel Series Apex PRO TKL 3 Wireless 2.4Ghz Bluetooth Black RGB Omnipoint 3.0 Hall Effect", Precio = 332.100M, Stock = 8, ImagenUrl = "/img/Captura7.png" },
                        new Producto { Nombre = "Producto 8", Descripcion = "Mouse Logitech G Pro X Superlight Wireless", Precio = 120.000M, Stock = 12, ImagenUrl = "/img/Captura8.png" },
                        new Producto { Nombre = "Producto 9", Descripcion = "Auriculares HyperX Cloud II", Precio = 80.000M, Stock = 7, ImagenUrl = "/img/Captura9.png" },
                        new Producto { Nombre = "Producto 10", Descripcion = "Pad Mouse Redragon Flick XL", Precio = 15.000M, Stock = 25, ImagenUrl = "/img/Captura10.png" }
                    });
                    db.SaveChanges();
                }
            }
        }
    }
}