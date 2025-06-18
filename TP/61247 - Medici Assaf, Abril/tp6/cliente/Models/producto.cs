namespace Cliente.Models;

public class Producto
{
    public int ProductoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Detalle { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int CantidadDisponible { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}
