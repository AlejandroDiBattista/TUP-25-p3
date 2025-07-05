using tp6.Models;               // Para acceder a las clases dentro de tp6.Models
using Microsoft.EntityFrameworkCore;  // Para trabajar con Entity Framework Core (EF Core)
using System;                   // Para tipos básicos como Guid, DateTime
using System.Collections.Generic;  // Para trabajar con colecciones como List

namespace tp6.Data
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        // Define las propiedades DbSet para las entidades
        public DbSet<Producto> Productos { get; set; }      // Define Productos
        public DbSet<Carrito> Carritos { get; set; }        // Define Carritos
        public DbSet<CarritoItem> CarritoItems { get; set; } // Define CarritoItems
        public DbSet<Compra> Compras { get; set; }          // Define Compras

        // Configuración del modelo
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configurar la relación entre Compra y ItemCompra (1:n)
    modelBuilder.Entity<ItemCompra>()
        .HasOne(ic => ic.Compra)              // Cada ItemCompra está relacionado con una Compra
        .WithMany(c => c.Items)               // Una Compra tiene muchos ItemCompra
        .HasForeignKey(ic => ic.CompraId);    // La clave foránea en ItemCompra que se refiere a Compra

    // Configuración de la relación entre Producto e ItemCompra
    modelBuilder.Entity<ItemCompra>()
        .HasOne(ic => ic.Producto)            // Cada ItemCompra está relacionado con un Producto
        .WithMany()                           // No necesitas especificar el lado "con muchos" aquí
        .HasForeignKey(ic => ic.ProductoId);  // La clave foránea en ItemCompra que se refiere a Producto

    // Configuración de la clave primaria compuesta en CarritoItem
    modelBuilder.Entity<CarritoItem>()
        .HasKey(ci => new { ci.CarritoId, ci.ProductoId });  // Clave primaria compuesta

    // Configurar las relaciones para CarritoItem
    modelBuilder.Entity<CarritoItem>()
        .HasOne(ci => ci.Carrito)
        .WithMany(c => c.CarritoItems)
        .HasForeignKey(ci => ci.CarritoId);

    // Cargar datos iniciales para los productos
   
modelBuilder.Entity<Producto>().HasData(
    new Producto { ProductoId = 1, Nombre = "Smartphone Galaxy Z", Descripcion = "Pantalla plegable", Precio = 1200.00m, Stock = 50, ImagenUrl = "img/smartphone.jpeg" },
    new Producto { ProductoId = 2, Nombre = "Laptop Dell XPS 13", Descripcion = "Laptop ultra delgada", Precio = 999.99m, Stock = 30, ImagenUrl = "img/laptop.jpeg" },
    new Producto { ProductoId = 3, Nombre = "Auriculares Sony WH-1000XM4", Descripcion = "Cancelación de ruido", Precio = 350.00m, Stock = 100, ImagenUrl = "img/auriculares.jpeg" },
    new Producto { ProductoId = 4, Nombre = "Apple Watch Series 7", Descripcion = "Reloj inteligente", Precio = 399.99m, Stock = 80, ImagenUrl = "img/applewatch.jpeg" },
    new Producto { ProductoId = 5, Nombre = "Cámara Canon EOS 80D", Descripcion = "Fotografía profesional", Precio = 850.00m, Stock = 40, ImagenUrl = "img/canon.jpg" },
    new Producto { ProductoId = 6, Nombre = "Teclado Razer Huntsman", Descripcion = "Teclado mecánico gamer", Precio = 129.99m, Stock = 60, ImagenUrl = "img/razer.jpg" },
    new Producto { ProductoId = 7, Nombre = "Monitor LG UltraWide", Descripcion = "Monitor curvo 34''", Precio = 400.00m, Stock = 25, ImagenUrl = "img/monitor.jpeg" },
     new Producto { ProductoId = 8, Nombre = "PlayStation 5", Descripcion = "Consola de última generación", Precio = 499.99m, Stock = 15, ImagenUrl = "img/ps5.jpeg" },
    new Producto { ProductoId = 9, Nombre = "Nintendo Switch OLED", Descripcion = "Consola híbrida", Precio = 349.00m, Stock = 50, ImagenUrl = "img/nintendo.jpeg" },
    new Producto { ProductoId = 10, Nombre = "Silla DXRacer", Descripcion = "Silla ergonómica gamer", Precio = 299.00m, Stock = 10, ImagenUrl = "img/silla.jpeg" }
  );

  }

    }
}