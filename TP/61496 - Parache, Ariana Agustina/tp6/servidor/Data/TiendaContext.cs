using Microsoft.EntityFrameworkCore;
using servidor.Models;
using Compartido.Models;
using Compartido.Dtos;


namespace servidor.Data 
{
    public class TiendaContext : DbContext
    {

        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }


        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }
            public DbSet<Carrito> Carritos { get; set; }
        public DbSet<ItemCarrito> ItemsCarrito { get; set; }
    }
}
