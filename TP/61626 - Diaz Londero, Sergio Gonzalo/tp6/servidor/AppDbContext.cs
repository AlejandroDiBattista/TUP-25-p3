using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

// Este contexto define las tablas de la base de datos y cómo se relacionan
public class AppDbContext : DbContext
{
    // Constructor que recibe las opciones de configuración
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tabla de productos
    public DbSet<Producto> Productos { get; set; }
    // Tabla de compras
    public DbSet<Compra> Compras { get; set; }
    // Tabla de ítems de compra
    public DbSet<ItemCompra> ItemsCompra { get; set; }

    // Puedes agregar más configuración si lo necesitas
    // protected override void OnModelCreating(ModelBuilder modelBuilder) { ... }
}
