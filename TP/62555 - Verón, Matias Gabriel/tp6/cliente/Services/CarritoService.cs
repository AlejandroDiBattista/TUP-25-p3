using cliente.Models;
using System.Collections.Generic;
using System.Linq;

namespace cliente.Services
{
    public class CarritoService
    {
        private List<CarritoItem> _items = new List<CarritoItem>();
        
        // Evento para notificar cambios a los componentes
        public event Action OnChange;
        
        // Propiedad pública para acceder a los items
        public IReadOnlyList<CarritoItem> Items => _items.AsReadOnly();

        private int _carritoId = 0;
        public int CarritoId
        {
            get => _carritoId;
            set => _carritoId = value;
        }

        public void AgregarAlCarrito(Producto producto, int cantidad)
        {
            // Validación básica
            if (producto == null || cantidad <= 0) return;
            
            var itemExistente = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                _items.Add(new CarritoItem 
                { 
                    Producto = producto, 
                    Cantidad = cantidad 
                });
            }
            
            NotificarCambios();
        }

        public void ModificarCantidad(Producto producto, int nuevaCantidad)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            
            if (item != null)
            {
                if (nuevaCantidad <= 0)
                {
                    QuitarDelCarrito(producto);
                }
                else if (nuevaCantidad <= producto.Stock) // Validar stock
                {
                    item.Cantidad = nuevaCantidad;
                    NotificarCambios();
                }
            }
        }

        public void QuitarDelCarrito(Producto producto)
        {
            var item = _items.FirstOrDefault(i => i.Producto.Id == producto.Id);
            if (item != null)
            {
                _items.Remove(item);
                NotificarCambios();
            }
        }

        public void VaciarCarrito()
        {
            _items.Clear();
            NotificarCambios();
        }

        public decimal CalcularTotal()
        {
            return _items.Sum(item => item.Producto.Precio * item.Cantidad);
        }

        public int ObtenerCantidadTotal()
        {
            return _items.Sum(item => item.Cantidad);
        }

        private void NotificarCambios()
        {
            OnChange?.Invoke();
        }
    }
}