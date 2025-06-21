using System.Collections.Generic;
using System.Linq;
using cliente.Models;

namespace cliente.Services {
    public class CarritoService
    {
        private List<Producto> productosEnCarrito = new List<Producto>();

        public int CantidadTotal => productosEnCarrito.Sum(p => p.Cantidad);

        public List<Producto> ProductosConStock { get; set; } = new();
        public List<Producto> ObtenerCarrito()
        {
            return productosEnCarrito;
        }

        public void AgregarProducto(Producto producto)
        {
            var productoExistente = productosEnCarrito.FirstOrDefault(p => p.Id == producto.Id);
            if (productoExistente != null)
            {
                productoExistente.Cantidad += 1;
            }
            else
            {
                productosEnCarrito.Add(new Producto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Imagen = producto.Imagen,
                    Precio = producto.Precio,
                    Cantidad = 1
                });
            }
        }

        public event Action OnChange;
        public void NotificarCambio()
        {
            OnChange?.Invoke();
        }
    }
}
