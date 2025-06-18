// Models/DetalleCarritoMemoria.cs
namespace cliente.Models
{
    // Clase que representa un detalle de ítem en el carrito, como lo maneja el backend en memoria.
    public class DetalleCarritoMemoria
    {
        public int ArticuloId { get; set; } // ID del artículo
        public int Unidades { get; set; } // Cantidad de unidades de este artículo
        public double ValorPorUnidad { get; set; } // Precio por unidad del artículo en el momento de añadirlo
    }
}
