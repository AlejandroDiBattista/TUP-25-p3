// Models/RegistroCompra.cs
namespace servidor.Models
{
    public class RegistroCompra
    {
        public int Id { get; set; }
        public DateTime FechaRealizacion { get; set; } // Antes Fecha
        public double MontoTotal { get; set; } // Antes Total
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public List<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>(); // Antes Items
    }
}