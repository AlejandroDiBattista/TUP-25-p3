using System.ComponentModel.DataAnnotations;

namespace cliente.Services;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = "";
}

public class CarritoIdDto
{
    public Guid carritoId { get; set; }
}

public class CarritoItemDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
    public string ImagenUrl { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal Subtotal { get; set; }
    public int Stock { get; set; } // Agregado para permitir el control de stock
}

public class ConfirmarCompraDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = "";
    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = "";
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string Email { get; set; } = "";
}

public class DatosRespuesta
{
    public string Mensaje { get; set; } = "";
    public DateTime Fecha { get; set; }
}
