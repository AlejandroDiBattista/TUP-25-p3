using System.ComponentModel.DataAnnotations;

namespace Cliente.Models
{
    public class CarritoItem
    {
        [Required]
        public Guid CarritoId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal PrecioUnitario { get; set; }

        public decimal Importe => PrecioUnitario * Cantidad;

        // 🔹 Constructor para facilitar la inicialización
        public CarritoItem() { }

        public CarritoItem(Guid carritoId, int productoId, string nombre, int cantidad, decimal precioUnitario)
        {
            CarritoId = carritoId;
            ProductoId = productoId;
            Nombre = nombre;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
        }
    }
}
