using System.Net.Http.Json;
using cliente.Models;

namespace cliente.Services;

/// <summary>
/// Servicio para gestionar el carrito de compras
/// </summary>
public class CarritoService
{
    private readonly HttpClient _httpClient;
    private readonly List<ItemCarrito> _items = new();
    
    /// <summary>
    /// Evento que se dispara cuando el contenido del carrito cambia
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de carrito
    /// </summary>
    /// <param name="httpClient">Cliente HTTP para realizar peticiones al servidor</param>
    public CarritoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Obtiene la lista de items en el carrito
    /// </summary>
    public IReadOnlyList<ItemCarrito> ObtenerItems() => _items.AsReadOnly();
    //IReadOnlyList<ItemCarrito> es una interfaz que permite acceder a la lista sin modificarla directamente.

    /// <summary>
    /// Obtiene la cantidad total de items en el carrito
    /// </summary>
    public int ObtenerCantidadTotal() => _items.Sum(x => x.Cantidad);

    /// <summary>
    /// Obtiene el monto total del carrito
    /// </summary>
    public decimal ObtenerTotal() => _items.Sum(x => x.Subtotal);

    /// <summary>
    /// Agrega un producto al carrito
    /// </summary>
    /// <param name="producto">Información del producto a agregar</param>
    /// <param name="cantidad">Cantidad a agregar (por defecto 1)</param>
    public void AgregarItem(ProductoDTO producto, int cantidad = 1)
    {
        if (cantidad <= 0) return;

        var item = _items.FirstOrDefault(x => x.ProductoId == producto.Id);
        
        if (item == null)
        {
            item = new ItemCarrito
            {
                ProductoId = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Cantidad = Math.Min(cantidad, producto.Stock)
            };
            _items.Add(item);
        }
        else
        {
            ActualizarCantidad(producto.Id, Math.Min(item.Cantidad + cantidad, producto.Stock));
        }

        NotificarCambios();
    }

    /// <summary>
    /// Actualiza la cantidad de un item en el carrito
    /// </summary>
    /// <param name="productoId">ID del producto a actualizar</param>
    /// <param name="cantidad">Nueva cantidad</param>
    public void ActualizarCantidad(int productoId, int cantidad)
    {
        var item = _items.FirstOrDefault(x => x.ProductoId == productoId);
        if (item != null)
        {
            if (cantidad <= 0)
            {
                _items.Remove(item);
            }
            else
            {
                item.Cantidad = cantidad;
            }
            NotificarCambios();
        }
    }

    /// <summary>
    /// Elimina un item del carrito
    /// </summary>
    /// <param name="productoId">ID del producto a eliminar</param>
    public void EliminarItem(int productoId)
    {
        var item = _items.FirstOrDefault(x => x.ProductoId == productoId);
        if (item != null)
        {
            _items.Remove(item);
            NotificarCambios();
        }
    }

    /// <summary>
    /// Vacía el carrito de compras
    /// </summary>
    public void LimpiarCarrito()
    {
        _items.Clear();
        NotificarCambios();
    }

    /// <summary>
    /// Procesa la compra de los items en el carrito
    /// </summary>
    /// <param name="nombre">Nombre del cliente</param>
    /// <param name="apellido">Apellido del cliente</param>
    /// <param name="email">Email del cliente</param>
    /// <returns>True si la compra fue exitosa, False en caso contrario</returns>
    public async Task<bool> ProcesarCompraAsync(string nombre, string apellido, string email)
    {
        if (!_items.Any()) return false;

        try
        {
            var orden = new OrdenCompraDTO
            {
                Items = _items.Select(x => new ItemOrdenDTO 
                { 
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("/api/compras", new
            {
                Items = orden.Items,
                NombreCliente = nombre,
                ApellidoCliente = apellido,
                EmailCliente = email
            });

            if (response.IsSuccessStatusCode)
            {
                LimpiarCarrito();
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al procesar la compra: {ex.Message}");
            return false;
        }
    }

    private void NotificarCambios() => OnChange?.Invoke();
}
