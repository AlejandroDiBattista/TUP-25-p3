using Microsoft.EntityFrameworkCore;
using servidor.Models;

namespace servidor.Data
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Celular Samsung A50", Descripcion = "Celular gama media", Precio = 120000, Stock = 10, ImagenUrl = "https://rongahogar.com.ar/storage/2024/07/00000653_1.jpg" },
                new Producto { Id = 2, Nombre = "Celular Samsung S25 Ultra", Descripcion = "Celular gama alta", Precio = 250000, Stock = 10, ImagenUrl = "https://bairesit.com.ar/Image/0/750_750-Proyecto%20nuevo%20(11)2.jpg" },
                new Producto { Id = 3, Nombre = "Auriculares", Descripcion = "Auriculares inalámbricos", Precio = 15000, Stock = 20, ImagenUrl = "https://images.bidcom.com.ar/resize?src=https://static.bidcom.com.ar/publicacionesML/productos/ABLUE120/1000x1000-ABLUE120.jpg&h=400&q=100" },
                new Producto { Id = 4, Nombre = "Cargador", Descripcion = "Cargador de carga rápida", Precio = 5000, Stock = 15, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/001/463/508/products/cargador-samsung1-55d817b8d4276f779d16908330131611-640-0.png" },
                new Producto { Id = 5, Nombre = "Notebook N4020 ", Descripcion = "14.1 Pulgadas 4GB 128GB SSD Philco", Precio = 350000, Stock = 5, ImagenUrl = "https://philco.com.ar/media/catalog/product/cache/c8f6a96bef9e9f64cd4973587df2520f/n/1/n14p4020_7.jpg" },
                new Producto { Id = 6, Nombre = "Mouse", Descripcion = "Mouse inalámbrico Gamer Logitech Lightspeed G305 / 6013 Azul", Precio = 3000, Stock = 25, ImagenUrl = "https://www.rodo.com.ar/media/catalog/product/cache/855090a5c67e45b26c9e0d345e7592dc/3/5/352810_logitech_mouse_inal_mbrico_3.jpg" },
                new Producto { Id = 7, Nombre = "Teclado", Descripcion = "Teclado mecánico Redragon Kumara Black rgb Switch Red", Precio = 8000, Stock = 12, ImagenUrl = "https://spacegamer.com.ar/img/Public/1058-producto-teclado-mecanico-redragon-kumara-black-rgb-switch-blue-6217.jpg" },
                new Producto { Id = 8, Nombre = "Monitor", Descripcion = "Monitor PHILIPS 241V8L/77 24 Pulgadas Full HD", Precio = 60000, Stock = 7, ImagenUrl = "https://bangho.vtexassets.com/arquivos/ids/161197-800-450?v=638004141157100000&width=800&height=450&aspect=true" },
                new Producto { Id = 9, Nombre = "Coca-Cola", Descripcion = "Botella 2L", Precio = 1200, Stock = 30, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/798/865/products/128527835-99c3b15680bb5015fb16639288497218-1024-1024.jpg" },
                new Producto { Id = 10, Nombre = "Cable USB", Descripcion = "Cable USB tipo C", Precio = 2000, Stock = 18, ImagenUrl = "https://www.heavenimagenes.com/heavencommerce/502e7a9a-55bf-47af-9c95-18d3e0fd480f/images/v2/USB/4186_xlarge.jpg" }
            );
        }
    }
}
