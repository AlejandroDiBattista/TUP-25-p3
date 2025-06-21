// Models/ArticuloInventario.cs
namespace servidor.Models
{
    public class ArticuloInventario
    {
        public int Id { get; set; }
        public string Denominacion { get; set; } = string.Empty; // Antes Nombre
        public string Caracteristicas { get; set; } = string.Empty; // Antes Descripcion
        public double ValorUnitario { get; set; } // Antes Precio
        public int CantidadDisponible { get; set; } // Antes Stock
        public string UrlImagenArticulo { get; set; } = string.Empty; // Antes ImagenUrl
    }
}