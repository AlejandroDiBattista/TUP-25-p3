namespace servidor.Data; // Corrección del espacio de nombres

using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Matraz Erlenmeyer", Descripcion = "Matraz de vidrio de 500ml con boca ancha.", Precio = 2500, Stock = 30, ImagenUrl = "matraz_erlenmeyer.jpg" },
            new Producto { Id = 2, Nombre = "Balón Aforado", Descripcion = "Balón de vidrio con capacidad de 250ml.", Precio = 1800, Stock = 25, ImagenUrl = "balon_aforado.jpg" },
            new Producto { Id = 3, Nombre = "Probeta Graduada", Descripcion = "Probeta de 100ml con graduación precisa.", Precio = 2000, Stock = 40, ImagenUrl = "probeta_graduada.jpg" },
            new Producto { Id = 4, Nombre = "Pipeta Volumétrica", Descripcion = "Pipeta de vidrio de 10ml.", Precio = 1500, Stock = 50, ImagenUrl = "pipeta_volumetrica.jpg" },
            new Producto { Id = 5, Nombre = "Bureta de Vidrio", Descripcion = "Bureta de 25ml con llave de teflón.", Precio = 3500, Stock = 20, ImagenUrl = "bureta_vidrio.jpg" },
            new Producto { Id = 6, Nombre = "Agitador Magnético", Descripcion = "Agitador con barra magnética para laboratorio.", Precio = 7500, Stock = 10, ImagenUrl = "agitador_magnetico.jpg" },
            new Producto { Id = 7, Nombre = "Mechero Bunsen", Descripcion = "Mechero con regulador de oxígeno para quemado eficiente.", Precio = 4200, Stock = 15, ImagenUrl = "mechero_bunsen.jpg" },
            new Producto { Id = 8, Nombre = "Caja de Portaobjetos", Descripcion = "Caja con 50 portaobjetos de vidrio.", Precio = 1200, Stock = 35, ImagenUrl = "portaobjetos.jpg" },
            new Producto { Id = 9, Nombre = "Manta de Calentamiento", Descripcion = "Manta eléctrica para calentar matraces hasta 500ml.", Precio = 8500, Stock = 12, ImagenUrl = "manta_calentamiento.jpg" },
            new Producto { Id = 10, Nombre = "Termómetro Digital", Descripcion = "Termómetro digital de precisión para laboratorio.", Precio = 5300, Stock = 18, ImagenUrl = "termometro_digital.jpg" }
        );
    }
}