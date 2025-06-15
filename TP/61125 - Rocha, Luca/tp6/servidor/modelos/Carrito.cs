using System.Collections.Generic;
using System.Linq;

namespace servidor.Modelos
{
    public class Carrito
    {
        public int Id { get; set; } // ✅ Agregamos la clave primaria requerida
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public decimal Total => Productos.Sum(p => p.Precio);
    }
}