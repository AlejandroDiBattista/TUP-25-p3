// --------------------------------------------------------------------------------------
// CarritoService.cs - Servicio para gestionar el carrito de compras en el cliente Blazor
// Archivo exhaustivamente comentado línea por línea y por bloque.
// --------------------------------------------------------------------------------------

using System.Linq; // Para operaciones LINQ sobre listas
using System.Collections.Generic; // Para List<T>
using System; // Para Action

namespace cliente.Services
{
    // Servicio que mantiene y gestiona el estado del carrito de compras en memoria
    public class CarritoService
    {
        private readonly ProductoService _productoService;
        public event Action? OnChange;
        // Lista de ítems actualmente en el carrito
        public List<ItemCarrito> Items { get; set; } = new();

        public CarritoService(ProductoService productoService)
        {
            _productoService = productoService;
        }

        // Agrega un producto al carrito, validando stock disponible
        public bool AgregarProducto(Producto producto)
        {
            // Siempre usar la referencia global
            var prodGlobal = _productoService.BuscarPorId(producto.Id);
            if (prodGlobal == null) return false;
            if (prodGlobal.Stock <= 0)
                return false; // No se puede agregar más si no hay stock

            var existente = Items.FirstOrDefault(p => p.Producto.Id == prodGlobal.Id);
            if (existente != null)
            {
                // Permitir agregar hasta agotar el stock
                existente.Cantidad++;
                prodGlobal.Stock--;
            }
            else
            {
                Items.Add(new ItemCarrito { Producto = prodGlobal, Cantidad = 1 });
                prodGlobal.Stock--;
            }
            NotifyStateChanged();
            return true;
        }

        // Elimina un producto del carrito por su ID y devuelve el stock al producto
        public void EliminarProducto(int productoId)
        {
            var item = Items.FirstOrDefault(p => p.Producto.Id == productoId);
            if (item != null)
            {
                // Siempre devolver el stock a la instancia global
                var prodGlobal = _productoService.BuscarPorId(productoId);
                if (prodGlobal != null)
                {
                    prodGlobal.Stock += item.Cantidad;
                }
                Items.Remove(item);
                NotifyStateChanged();
            }
        }

        // Vacía completamente el carrito y devuelve el stock de todos los productos
        public void VaciarCarrito()
        {
            foreach (var item in Items)
            {
                // Buscar la referencia global y regenerar el stock ahí
                var prodGlobal = _productoService.BuscarPorId(item.Producto.Id);
                if (prodGlobal != null)
                {
                    prodGlobal.Stock += item.Cantidad;
                }
            }
            Items.Clear();
            NotifyStateChanged();
        }

        // Cambia la cantidad de un producto en el carrito
        public void CambiarCantidad(int productoId, int delta)
        {
            var prodGlobal = _productoService.BuscarPorId(productoId);
            var item = Items.FirstOrDefault(i => i.Producto.Id == productoId);
            if (item != null && prodGlobal != null)
            {
                if (delta == 1 && prodGlobal.Stock > 0)
                {
                    item.Cantidad++;
                    prodGlobal.Stock--;
                }
                else if (delta == -1 && item.Cantidad > 1)
                {
                    item.Cantidad--;
                    prodGlobal.Stock++;
                }
                else if (delta == -1 && item.Cantidad == 1)
                {
                    EliminarProducto(productoId);
                    return;
                }
                NotifyStateChanged();
            }
        }

        // Calcula el total a pagar por todos los ítems del carrito
        public decimal CalcularTotal()
        {
            return Items.Sum(i => i.Cantidad * i.Producto.Precio);
        }

        // Devuelve la cantidad de un producto específico en el carrito
        private int CantidadEnCarrito(int productoId)
        {
            var item = Items.FirstOrDefault(p => p.Producto.Id == productoId);
            return item?.Cantidad ?? 0;
        }

        // Propiedad que devuelve la cantidad total de productos en el carrito
        public int CantidadTotal => Items.Sum(i => i.Cantidad);

        private void NotifyStateChanged() => OnChange?.Invoke();
    }

    // Representa un ítem del carrito (producto + cantidad)
    public class ItemCarrito
    {
        public Producto Producto { get; set; } // Producto agregado
        public int Cantidad { get; set; } // Cantidad de ese producto
    }
}
