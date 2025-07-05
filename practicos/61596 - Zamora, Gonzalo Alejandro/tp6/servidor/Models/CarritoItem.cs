using tp6.Data;
using tp6.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
// Para trabajar con colecciones como List
namespace tp6.Models
{
    public class CarritoItem
    {
        // Claves primarias compuestas: CarritoId + ProductoId
        public Guid CarritoId { get; set; }
        public Carrito Carrito { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        // 🔹 Agregamos esta propiedad que usa el frontend:
        public decimal PrecioUnitario { get; set; }

        // La clave primaria compuesta
        public override bool Equals(object obj)
        {
            return obj is CarritoItem item &&
                   CarritoId.Equals(item.CarritoId) &&
                   ProductoId == item.ProductoId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CarritoId, ProductoId);
        }
    }
}