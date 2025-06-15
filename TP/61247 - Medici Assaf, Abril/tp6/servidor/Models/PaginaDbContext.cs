using Microsoft.EntityFrameworkCore;

namespace Servidor.Models;

public class PaginaContext : DbContext
{
    public PaginaContext(DbContextOptions<PaginaContext> opciones) : base(opciones) {}

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Operacion> Operaciones => Set<Operacion>();
    public DbSet<ItemOperacion> ItemsOperaciones => Set<ItemOperacion>();


//AGREGA LOS PRODUCTOS PUTA
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
        new Producto { ProductoId = 1, Titulo = "Baggie Crema", Detalle = "Pantalon de algodon con estampado.", Valor = 35200, CantidadDisponible = 12, ImagenUrl = "img.catalogo/baggie_blanco.jpg" },
        new Producto { ProductoId = 2, Titulo = "Baggie Negro", Detalle = "Pantalon de algodon con estampado.", Valor = 35200, CantidadDisponible = 8, ImagenUrl = "img.catalogo/baggie_negro.jpeg" },
        new Producto { ProductoId = 3, Titulo = "Bermuda", Detalle = "Bermuda de jean", Valor = 25000, CantidadDisponible = 15, ImagenUrl = "img.catalogo/bermuda.jpeg" },
        new Producto { ProductoId = 4, Titulo = "Buzo marron", Detalle = "Buzo de algodon bordado.", Valor = 40000, CantidadDisponible = 15, ImagenUrl = "img.catalogo/buzo_marron.jpg" },
        new Producto { ProductoId = 5, Titulo = "Buzo negro", Detalle = "Buzo de algodon bordado", Valor = 40000, CantidadDisponible = 15, ImagenUrl = "img.catalogo/buzo_negro.jpeg" },
        new Producto { ProductoId = 6, Titulo = "Camisa negra", Detalle = "Camisa mangas cortas", Valor = 37000, CantidadDisponible = 15, ImagenUrl = "img.catalogo/camisa.jpg" },
        new Producto { ProductoId = 8, Titulo = "Gorra", Detalle = "Gorra gris gastado", Valor = 15000, CantidadDisponible = 15, ImagenUrl = "img.catalogo/gorra.jpeg" },
        new Producto { ProductoId = 9, Titulo = "Jean Ahumado", Detalle = "Pantalon de jean con bordado", Valor = 40000, CantidadDisponible = 15, ImagenUrl = "img.catalogo/jean_logo.jpeg" },
        new Producto { ProductoId = 10, Titulo = "Pantalon cargo", Detalle = "Pantalon cargo camuflado gris", Valor = 41500, CantidadDisponible = 15, ImagenUrl = "img.catalogo/pantalon_camuflado.jpeg" }
        );
    }
}
