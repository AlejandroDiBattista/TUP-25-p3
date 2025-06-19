using Microsoft.EntityFrameworkCore;
using Servidor.Modelos;

namespace Servidor.Datos;

public class TiendaDbContext : DbContext
{
    public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Tarta de Frutilla", Descripcion = "Base crocante con crema pastelera y frutillas frescas", Precio = 45000.00m, Stock = 10, ImagenUrl = "https://www.recetasnestlecam.com/sites/default/files/styles/recipe_detail_mobile/public/srh_recipes/07892f02f7c57b83d5703b4ee924221e.jpg?itok=Vrlk1qve" },
            new Producto { Id = 2, Nombre = "Cheesecake", Descripcion = "Tarta de queso con base de galleta y coulis de frutos rojos", Precio = 48000.00m, Stock = 8, ImagenUrl = "https://laopinionaustral.com.ar/media/uploads/2024/05/receta-lemon-pie.jpg" },
            new Producto { Id = 3, Nombre = "Brownie", Descripcion = "Brownie húmedo con nueces y baño de chocolate", Precio = 32000.00m, Stock = 15, ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSWk6QSUvVAwMUCKo4rWpU5yAuBsmfw0JwftnblS3skQIm_7cG85vIKik-wOE4OlNaS4K0&usqp=CAU" },
            new Producto { Id = 4, Nombre = "Lemon Pie", Descripcion = "Tarta de limón con merengue italiano", Precio = 44000.00m, Stock = 7, ImagenUrl = "https://laopinionaustral.com.ar/media/uploads/2024/05/receta-lemon-pie.jpg" },
            new Producto { Id = 5, Nombre = "Torta Selva Negra", Descripcion = "Bizcochuelo de chocolate, crema y cerezas", Precio = 60000.00m, Stock = 5, ImagenUrl = "https://tofuu.getjusto.com/orioneat-local/resized2/zqaT5XLQ7RAqLXsKd-2400-x.webp" },
            new Producto { Id = 6, Nombre = "Torta Oreo", Descripcion = "Torta con crema de oreo y base de galletas", Precio = 55000.00m, Stock = 6, ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS8QoDdXTxTbBYqfQlOBARSbi2NMifsivPdUA&s" },
            new Producto { Id = 7, Nombre = "Cupcake de Vainilla", Descripcion = "Cupcake con buttercream de colores", Precio = 9000.00m, Stock = 25, ImagenUrl = "https://storage.googleapis.com/fitia_public_images/recipes%2FGR-R-V-00001923_kh2jkptyj90wdbzs6jrjm2dn_large.jpeg" },
            new Producto { Id = 8, Nombre = "Macarons", Descripcion = "Caja con 6 macarons surtidos", Precio = 27000.00m, Stock = 10, ImagenUrl = "https://assets.tmecosys.com/image/upload/t_web_rdp_recipe_584x480/img/recipe/ras/Assets/B9FF58C7-D19C-4699-ADA6-E49760836EBB/Derivates/e1b80516-57ce-471c-bdee-eed02a742326.jpg" },
            new Producto { Id = 9, Nombre = "Alfajor de Maicena", Descripcion = "Relleno de dulce de leche y coco rallado", Precio = 7000.00m, Stock = 30, ImagenUrl = "https://assets.unileversolutions.com/recipes-v3/160118-default.png" },
            new Producto { Id = 10, Nombre = "Torta de Zanahoria", Descripcion = "Bizcochuelo de zanahoria con frosting de queso", Precio = 50000.00m, Stock = 14, ImagenUrl = "https://coosol.es/wp-content/uploads/2023/05/rebanada-pastel-zanahoria-dulce-mesa-madera-1.jpg" }
        );
    }
}
