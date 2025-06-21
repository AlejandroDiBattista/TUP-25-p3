// --------------------------------------------------------------------------------------
// DTOs (Data Transfer Objects) utilizados para la transferencia de datos de compras
// entre el cliente Blazor y la API. Cada clase está comentada línea por línea y por bloque.
// --------------------------------------------------------------------------------------

namespace cliente.Dtos
{
    // Representa un ítem dentro de una compra (producto + cantidad + precio)
    public class ItemCompraDto
    {
        // Identificador único del ítem de compra
        public int Id { get; set; }
        // Cantidad de unidades compradas de este producto
        public int Cantidad { get; set; }
        // Precio unitario del producto al momento de la compra
        public decimal PrecioUnitario { get; set; }
        // Información básica del producto comprado
        public ProductoDto Producto { get; set; }
    }

    // Representa la información básica de un producto (solo para mostrar en historial)
    public class ProductoDto
    {
        // Identificador único del producto
        public int Id { get; set; }
        // Nombre del producto
        public string Nombre { get; set; }
    }
}
