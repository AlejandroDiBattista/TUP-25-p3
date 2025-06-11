using cliente.Models;
using System.Colletions.Generic;
using System.Linq;

namespace cliente.Services
{
    public class CarritoService
    {
        private List<CarritoItem> _carrito;

        public CarritoService()
        {
            _carrito = new List<CarritoItem>();
        }

        public void AgregarAlCarrito(Producto producto, int cantidad)
        {
            var itemExistente = _carrito.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                _carrito.Add(new CarritoItem { Producto = producto, Cantidad = cantidad });
            }
        }

    }
}