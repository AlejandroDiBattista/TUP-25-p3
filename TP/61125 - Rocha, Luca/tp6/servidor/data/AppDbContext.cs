using Microsoft.EntityFrameworkCore;
using servidor.Modelos;
using System;

namespace servidor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Corrección en las rutas de imágenes (solo el nombre del archivo)
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Vaso medidor", ImagenUrl = "vaso1.jpg" },
            new Producto { Id = 2, Nombre = "Manta calefactora", ImagenUrl = "manta.jpg" },
            new Producto { Id = 3, Nombre = "Pipeta graduada", ImagenUrl = "pipeta.jpg" },
            new Producto { Id = 4, Nombre = "Matraz Erlenmeyer", ImagenUrl = "matraz.jpg" },
            new Producto { Id = 5, Nombre = "Balanza digital", ImagenUrl = "balanza.jpg" },
            new Producto { Id = 6, Nombre = "Agitador magnético", ImagenUrl = "agitador.jpg" },
            new Producto { Id = 7, Nombre = "Tubos de ensayo", ImagenUrl = "tubos.jpg" },
            new Producto { Id = 8, Nombre = "Probeta", ImagenUrl = "probeta.jpg" },
            new Producto { Id = 9, Nombre = "Cronómetro", ImagenUrl = "cronometro.jpg" },
            new Producto { Id = 10, Nombre = "Termómetro digital", ImagenUrl = "termometro.jpg" }
        );
    }
}