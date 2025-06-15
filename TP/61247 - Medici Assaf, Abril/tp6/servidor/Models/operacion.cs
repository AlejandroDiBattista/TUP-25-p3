namespace Servidor.Models;

public class Operacion
{
    public int OperacionId { get; set; }
    public DateTime FechaHora { get; set; }
    public decimal Total { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;

    public List<ItemOperacion> Items { get; set; } = new();
}
