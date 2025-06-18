using System.ComponentModel.DataAnnotations;

namespace Cliente.Models;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
}

public class ItemCarritoDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Importe => Cantidad * PrecioUnitario;
}

public class CarritoDto
{
    public string Id { get; set; } = string.Empty;
    public List<ItemCarritoDto> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Importe);
    public int TotalItems => Items.Sum(i => i.Cantidad);
}

public class ConfirmarCompraDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    [SoloLetras(ErrorMessage = "El nombre solo puede contener letras y espacios")]
    public string NombreCliente { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
    [SoloLetras(ErrorMessage = "El apellido solo puede contener letras y espacios")]
    public string ApellidoCliente { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingresa un email válido")]
    [StringLength(200, ErrorMessage = "El email no puede exceder los 200 caracteres")]
    public string EmailCliente { get; set; } = string.Empty;
}

public class ResumenCompraDto
{
    public int TotalItems { get; set; }
    public decimal Total { get; set; }
}
