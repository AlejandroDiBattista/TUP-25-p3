using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

namespace servidor.Modelos
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ItemCarrito> ItemsCarrito { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Carrito>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Carrito)
                .HasForeignKey(i => i.CarritoId);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Toyota Corolla", Descripcion = "Sedán compacto, motor 1.8L", Precio = 12000000, Stock = 5, ImagUrl = "toyotacorolla.jpg" },
                new Producto { Id = 2, Nombre = "Ford Fiesta", Descripcion = "Hatchback, motor 1.6L", Precio = 9500000, Stock = 7, ImagUrl = "forfiesta.jpg" },
                new Producto { Id = 3, Nombre = "Volkswagen Golf", Descripcion = "Hatchback, motor 1.4L", Precio = 11000000, Stock = 4, ImagUrl = "VolkswagenGolf.jpg" },
                new Producto { Id = 4, Nombre = "Renault Sandero", Descripcion = "Hatchback, motor 1.6L", Precio = 8000000, Stock = 6, ImagUrl = "RenaultSandero.jpg" },
                new Producto { Id = 5, Nombre = "Chevrolet Onix", Descripcion = "Hatchback, motor 1.0L", Precio = 8500000, Stock = 8, ImagUrl = "ChevroletOnix.jpg" },
                new Producto { Id = 6, Nombre = "Peugeot 208", Descripcion = "Hatchback, motor 1.2L", Precio = 9000000, Stock = 5, ImagUrl = "Peugeot 208.jpg" },
                new Producto { Id = 7, Nombre = "Fiat Cronos", Descripcion = "Sedán, motor 1.3L", Precio = 9500000, Stock = 7, ImagUrl = "FiatCronos.jpg" },
                new Producto { Id = 8, Nombre = "Honda Fit", Descripcion = "Hatchback, motor 1.5L", Precio = 10500000, Stock = 3, ImagUrl = "HondaCivic.jpg" },
                new Producto { Id = 9, Nombre = "Nissan Versa", Descripcion = "Sedán, motor 1.6L", Precio = 11500000, Stock = 4, ImagUrl = "NissanVersa.jpg" },
                new Producto { Id = 10, Nombre = "Kia Rio", Descripcion = "Hatchback, motor 1.4L", Precio = 9800000, Stock = 6, ImagUrl = "ToyotaHilux.jpg" }
            );
        }

        public async Task<int> GuardarCambiosAsync()
        {
            await SaveChangesAsync();
            return 0;
        }
    }
}