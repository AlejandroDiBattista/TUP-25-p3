using cliente.Models;
using System.Collections.Generic;
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
        public void QuitarDelCarrito(Producto producto)
        {
            var itemExistente = _carrito.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (itemExistente != null)
            {
                _carrito.Remove(itemExistente);
            }
        }

        public List<CarritoItem> ObtenerCarrito()
        {
            return _carrito;
        }

        public void VaciarCarrito()
        {
            _carrito.Clear();
        }
        public decimal CalcularTotal()
        {
            return _carrito.Sum(item => item.Producto.Precio * item.Cantidad);
        }
    }
}