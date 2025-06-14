using Microsoft.EntityFrameworkCore;
using TiendaOnline.Models;
using System;

namespace TiendaOnline.Data
{
    public class AppDbContext : DbContext
    {

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Celular XYZ", Descripcion = "Celular moderno y rápido", Precio = 15000m, Stock = 5, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 2, Nombre = "Auriculares ABC", Descripcion = "Auriculares inalámbricos", Precio = 3000m, Stock = 10, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 3, Nombre = "Smartwatch 123", Descripcion = "Reloj inteligente", Precio = 7000m, Stock = 8, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 4, Nombre = "Cargador rápido", Descripcion = "Cargador USB tipo C", Precio = 1200m, Stock = 15, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 5, Nombre = "Powerbank 10000mAh", Descripcion = "Batería portátil", Precio = 2500m, Stock = 20, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 6, Nombre = "Mouse inalámbrico", Descripcion = "Mouse ergonómico", Precio = 1800m, Stock = 12, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 7, Nombre = "Teclado mecánico", Descripcion = "Teclado con retroiluminación", Precio = 5000m, Stock = 7, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 8, Nombre = "Monitor 24\"", Descripcion = "Monitor Full HD", Precio = 12000m, Stock = 4, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 9, Nombre = "Memoria USB 64GB", Descripcion = "Pendrive rápido", Precio = 1000m, Stock = 25, ImagenUrl = "https://via.placeholder.com/150" },
                new Producto { Id = 10, Nombre = "Cable HDMI", Descripcion = "Cable para video", Precio = 800m, Stock = 30, ImagenUrl = "https://via.placeholder.com/150" }
            );
        }
    }
}

