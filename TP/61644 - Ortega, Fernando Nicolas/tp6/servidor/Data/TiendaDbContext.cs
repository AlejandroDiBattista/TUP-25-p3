using Microsoft.EntityFrameworkCore;
using servidor.Models;
namespace servidor.Data
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ArticuloCompra> ArticulosCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Camiseta Oficial MotoGP", Descripcion = "Camiseta de algodón con logo MotoGP", Precio = 25000, Stock = 15, ImagenUrl = "img/CamisetaOficialMotoGP.png" },
                new Producto { Id = 2, Nombre = "Gorra Yamaha Racing", Descripcion = "Gorra oficial Yamaha MotoGP", Precio = 18000, Stock = 10, ImagenUrl = "img/GorraoficialYamahaMotoGP.png" },
                new Producto { Id = 3, Nombre = "Guantes Alpinestars GP", Descripcion = "Guantes de competición Alpinestars", Precio = 40000, Stock = 8, ImagenUrl = "img/GuantesdecompeticiónAlpinestars.png" },
                new Producto { Id = 4, Nombre = "Casco Replica Valentino Rossi", Descripcion = "Casco edición especial VR46", Precio = 150000, Stock = 5, ImagenUrl = "img/CascoediciónespecialVR46.png" },
                new Producto { Id = 5, Nombre = "Chaqueta Repsol Honda", Descripcion = "Chaqueta oficial Repsol Honda Team", Precio = 60000, Stock = 7, ImagenUrl = "img/ChaquetaoficialRepsolHondaTeam.png" },
                new Producto { Id = 6, Nombre = "Miniatura MotoGP Ducati", Descripcion = "Miniatura de moto Ducati Desmosedici", Precio = 12000, Stock = 20, ImagenUrl = "img/MiniaturademotoDucatiDesmosedici.png" },
                new Producto { Id = 7, Nombre = "Llavero MotoGP", Descripcion = "Llavero metálico con logo MotoGP", Precio = 3000, Stock = 50, ImagenUrl = "img/LlaverometálicoconlogoMotoGP.png" },
                new Producto { Id = 8, Nombre = "Botella Yamaha Monster", Descripcion = "Botella deportiva Yamaha Monster Energy", Precio = 7000, Stock = 25, ImagenUrl = "img/BotelladeportivaYamahaMonsterEnergy.png" },
                new Producto { Id = 9, Nombre = "Póster Marc Márquez", Descripcion = "Póster oficial Marc Márquez 93", Precio = 4000, Stock = 30, ImagenUrl = "img/PósteroficialMarcMárquez93.png" },
                new Producto { Id = 10, Nombre = "Bandera MotoGP", Descripcion = "Bandera oficial MotoGP para fans", Precio = 9000, Stock = 18, ImagenUrl = "img/BanderaoficialMotoGPparafans.png" }
            );
        }
    }
}