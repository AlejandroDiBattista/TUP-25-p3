// Models/ItemCarritoDisplay.cs
namespace cliente.Models
{
    // Clase auxiliar para mostrar los ítems del carrito en el frontend.
    // Combina la información del artículo con la cantidad y el precio del carrito.
    public class ItemCarritoDisplay
    {
        public ArticuloInventario? Articulo { get; set; } // Detalles completos del artículo
        public int Cantidad { get; set; } // Cantidad en el carrito
        public double PrecioUnitario { get; set; } // Precio unitario en el carrito
        public double Subtotal => Cantidad * PrecioUnitario; // Subtotal calculado
    }
}
