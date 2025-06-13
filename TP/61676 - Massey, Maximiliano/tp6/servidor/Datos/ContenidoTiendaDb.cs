using Microsoft.EntityFrameworkCore;
public class ContenidoTiendaDb : DbContext
{
    public ContenidoTiendaDb(DbContextOptions<ContenidoTiendaDb> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<ItemCompra> ItemsCompra => Set<ItemCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>().HasData(
            new Producto {Id= 1, Nombre = "half-life", Descripcion = "Acción en primera persona. Escapa de Black Mesa y lucha contra alienígenas y soldados.", Precio = 5.79, Stock = 10, Imagen = "img/half-life.png"},
            new Producto {Id= 2, Nombre = "Blasphemous", Descripcion = "Plataformas y acción en un mundo oscuro. Mejora habilidades y vence enemigos.", Precio = 12.49, Stock = 15, Imagen = "img/blasphemous.png"},
            new Producto {Id= 3, Nombre = "Hollow Knight", Descripcion = "Explora Hallownest, combate criaturas y descubre secretos en 2D.", Precio = 4.99, Stock = 20, Imagen = "img/hollow knight.png"},
            new Producto {Id= 4, Nombre = "Celeste", Descripcion = "Ayuda a Madeline a escalar la montaña Celeste en este desafiante juego de plataformas.", Precio = 9.99, Stock = 25, Imagen = "img/celeste.png"},
            new Producto {Id= 5, Nombre = "Resident Evli 4", Descripcion = "Acción y terror. Rescata a Ashley y enfrenta enemigos infectados en España.", Precio = 2.99, Stock = 30, Imagen = "img/Resident Evli 4.png"},
            new Producto {Id= 6, Nombre = "dark souls", Descripcion = "RPG desafiante. Explora un mundo oscuro y enfrenta jefes épicos.", Precio = 31.99, Stock = 5, Imagen = "img/dark souls.png"},
            new Producto {Id= 7, Nombre = "grim fandango", Descripcion = "Aventura gráfica. Ayuda a Manny Calavera a guiar almas en el inframundo con humor y misterio.", Precio = 7.99, Stock = 18, Imagen = "img/grim fandango.png"},
            new Producto {Id= 8, Nombre = "terraria", Descripcion = "Aventura sandbox en 2D. Explora, construye, combate y sobrevive en mundos generados.", Precio = 9.99, Stock = 12, Imagen = "img/terraria.png"},
            new Producto {Id= 9, Nombre = "the forest", Descripcion = "Supervivencia en primera persona. Construye, explora y enfréntate a caníbales en una isla misteriosa.", Precio = 14.99, Stock = 22, Imagen = "img/the forest.png"},
            new Producto {Id= 10, Nombre = "skyrim", Descripcion = "RPG de mundo abierto. Explora Skyrim, completa misiones y desarrolla tu personaje como quieras.", Precio = 19.99, Stock = 14, Imagen = "img/skyrim.png"}
            );
    }
}