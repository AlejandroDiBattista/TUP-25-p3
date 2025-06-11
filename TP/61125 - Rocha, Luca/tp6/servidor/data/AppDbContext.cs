namespace servidor.Data; // Espacio de nombres corregido

using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
}