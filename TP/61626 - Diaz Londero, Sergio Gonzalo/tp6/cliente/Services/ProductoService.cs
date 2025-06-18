// Servicio singleton para compartir la lista de productos entre Home y Carrito
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class ProductoService
    {
        public List<Producto> Productos { get; private set; } = new();
        public void SetProductos(List<Producto> productos) => Productos = productos;
        public Producto? BuscarPorId(int id) => Productos.Find(p => p.Id == id);
        public void ActualizarProductos(List<Producto> nuevos)
        {
            foreach (var nuevo in nuevos)
            {
                // Buscar por ID en la lista global
                var existente = Productos.FirstOrDefault(p => p.Id == nuevo.Id);
                if (existente != null)
                {
                    // Solo actualizar nombre, precio, descripción, imagen. JAMÁS el stock.
                    existente.Nombre = nuevo.Nombre;
                    existente.Precio = nuevo.Precio;
                    existente.Descripcion = nuevo.Descripcion;
                    existente.ImagenUrl = nuevo.ImagenUrl;
                }
                // Si no existe, lo agrego solo si no hay ninguno igual
                else if (!Productos.Any(p => p.Id == nuevo.Id))
                {
                    Productos.Add(new Producto {
                        Id = nuevo.Id,
                        Nombre = nuevo.Nombre,
                        Precio = nuevo.Precio,
                        Descripcion = nuevo.Descripcion,
                        ImagenUrl = nuevo.ImagenUrl,
                        Stock = nuevo.Stock
                    });
                }
            }
        }
    }
}
