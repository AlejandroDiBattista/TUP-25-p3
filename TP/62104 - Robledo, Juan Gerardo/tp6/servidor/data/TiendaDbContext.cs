using Microsoft.EntityFrameworkCore;
using servidor.models;

namespace servidor.data
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Compra> Compras => Set<Compra>();
        public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Datos de ejemplo
        modelBuilder.Entity<Producto>().HasData(
             new Producto { Id = 1, Nombre = "Celular A1", Descripcion = "Smartphone básico", Precio = 120000, Stock = 10, ImagenUrl = "https://i.blogs.es/2b336d/samsung_galaxy_a01/1366_2000.jpg" },
            new Producto { Id = 2, Nombre = "Celular B2", Descripcion = "Gama media", Precio = 180000, Stock = 5, ImagenUrl = "https://celularesindustriales.com.ar/wp-content/uploads/81IMNoEKmDL._AC_UF8941000_QL80_.jpg" },
            new Producto { Id = 3, Nombre = "Auriculares", Descripcion = "Bluetooth", Precio = 25000, Stock = 20, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/593/734/products/ng-a100bt-pl-angulo1-8d30f7d17ecc9b8c8e16518679401776-1024-1024.jpg" },
            new Producto { Id = 4, Nombre = "Cargador rápido", Descripcion = "USB-C", Precio = 15000, Stock = 15, ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_990917-MLA81168771561_122024-O.webp" },
            new Producto { Id = 5, Nombre = "Notebook X1", Descripcion = "8GB RAM", Precio = 450000, Stock = 3, ImagenUrl = "https://http2.mlstatic.com/D_605931-MLA80537530656_112024-O.jpg" },
            new Producto { Id = 6, Nombre = "Tablet", Descripcion = "10 pulgadas", Precio = 220000, Stock = 7, ImagenUrl = "https://philco.com.ar/media/catalog/product/cache/c8f6a96bef9e9f64cd4973587df2520f/t/p/tp10a332_icfbso0002_1_.jpg" },
            new Producto { Id = 7, Nombre = "Gaseosa Cola", Descripcion = "1.5L", Precio = 1200, Stock = 30, ImagenUrl = "https://www.liderlogo.es/wp-content/uploads/2023/01/coca-cola-1-1.jpg" },
            new Producto { Id = 8, Nombre = "Mouse gamer", Descripcion = "RGB", Precio = 10000, Stock = 12, ImagenUrl = "https://spacegamer.com.ar/img/Public/1058-producto-impact-8051.jpg" },
            new Producto { Id = 9, Nombre = "Teclado mecánico", Descripcion = "RGB", Precio = 18000, Stock = 10, ImagenUrl = "https://spacegamer.com.ar/img/Public/1058/93879-producto-1.jpg" },
            new Producto { Id = 10, Nombre = "Cámara Web", Descripcion = "Full HD", Precio = 25000, Stock = 6, ImagenUrl = "https://sampietroweb.com.ar/Image/0/750_750-0088110744_1.jpg" }
        );
    }  
    }
}