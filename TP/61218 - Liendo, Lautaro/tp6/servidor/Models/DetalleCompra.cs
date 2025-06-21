// Models/DetalleCompra.cs
namespace servidor.Models
{
    public class DetalleCompra
    {
        public int Id { get; set; }
        public int ArticuloInventarioId { get; set; } // Antes ProductoId
        public int RegistroCompraId { get; set; } // Antes CompraId
        public int CantidadAdquirida { get; set; } // Antes Cantidad
        public double PrecioUnitarioAlMomento { get; set; } // Antes PrecioUnitario
    }
}