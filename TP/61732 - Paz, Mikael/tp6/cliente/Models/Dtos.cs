namespace cliente.Models;

public class ProductoDto {
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class CarritoDto {
    public Guid Id { get; set; }
    public List<ItemCarritoDto> Items { get; set; } = new();
}

public class ItemCarritoDto {
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
