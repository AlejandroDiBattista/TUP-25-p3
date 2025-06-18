#nullable enable
// Models/ArticuloInventario.cs
namespace cliente.Models
{
    // Clase que representa un artículo en el inventario, reflejando el modelo del backend.
    public class ArticuloInventario
    {
        public int Id { get; set; }
        public string Denominacion { get; set; } = string.Empty; // Nombre del artículo
        public string Caracteristicas { get; set; } = string.Empty; // Descripción del artículo
        public double ValorUnitario { get; set; } // Precio unitario
        public int CantidadDisponible { get; set; } // Stock disponible
        public string UrlImagenArticulo { get; set; } = string.Empty; // URL de la imagen del artículo
    }
}
