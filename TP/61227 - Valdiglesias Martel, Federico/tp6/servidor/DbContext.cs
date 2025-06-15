using Microsoft.EntityFrameworkCore;


public record DatosClienteDto(string Nombre, string Apellido, string Email);

public class TiendaDb : DbContext
{
    public TiendaDb(DbContextOptions<TiendaDb> options) : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<ItemCompra> ItemsCompra { get; set; } // O DetalleCompra si así se llama tu clase
}