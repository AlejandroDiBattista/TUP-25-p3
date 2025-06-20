namespace Cliente.Models2
{
    /// <summary>
    /// Representa un producto disponible en el catálogo de la tienda.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción breve del producto.
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        public decimal Precio { get; set; }

        /// <summary>
        /// Cantidad disponible en stock.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Ruta o URL de la imagen del producto.
        /// </summary>
        public string ImagenUrl { get; set; } = string.Empty;
    }
}
