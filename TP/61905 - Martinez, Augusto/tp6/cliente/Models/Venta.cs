using Cliente.Models;

namespace Cliente.Models;

public class Venta
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string NombreCliente { get; set; }
    public string ApellidoCliente { get; set; }
    public string EmailCliente { get; set; }
    public List<VentaItem> VentaItems { get; set; } = new();
}

