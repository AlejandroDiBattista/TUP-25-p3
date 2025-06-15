namespace Servidor.Models;

public class ItemOperacion
{
    public int ItemOperacionId { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    public int OperacionId { get; set; }
    public Operacion? Operacion { get; set; }

    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
