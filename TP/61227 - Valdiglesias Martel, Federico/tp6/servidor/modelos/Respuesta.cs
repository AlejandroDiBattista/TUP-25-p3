using System.Collections.Generic;

namespace servidor.modelos;

public record DatosClienteDto(string Nombre, string Apellido, string Email);

public class DetalleRespuestaDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string ImagenUrl { get; set; } = "";
}

public class CompraRespuestaDto
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public List<DetalleRespuestaDto> Items { get; set; } = new();
}