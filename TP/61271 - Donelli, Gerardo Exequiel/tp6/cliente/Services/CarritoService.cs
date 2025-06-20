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
/// Evento que se dispara cuando se debe actualizar el stock
/// </summary>
public Action<int, int>? OnStockActualizar; // productoId, cambio (positivo o negativo)

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

    var item = _items.FirstOrDefault(x => x.ProductoId == producto.Id);// buscamos si ya existe un item con ese producto
    // si no existe, creamos uno nuevo

    if (item == null)// si no existe el item en el carrito
    // verificamos que la cantidad no supere el stock del producto
    // si supera el stock, agregamos la cantidad máxima posible
    {
        int cantidadFinal = Math.Min(cantidad, producto.Stock);
        item = new ItemCarrito
        {
            ProductoId = producto.Id,
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Cantidad = cantidadFinal
        };
        _items.Add(item);// agregamos el nuevo item al carrito
        // Disparamos el evento de actualización de stock
        OnStockActualizar?.Invoke(producto.Id, -cantidadFinal); // restamos stock
    }
    else
    {
        int nuevaCantidad = Math.Min(item.Cantidad + cantidad, producto.Stock);
        int diferencia = nuevaCantidad - item.Cantidad;
        item.Cantidad = nuevaCantidad;
        OnStockActualizar?.Invoke(producto.Id, -diferencia); // restamos lo agregado
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
    var item = _items.FirstOrDefault(x => x.ProductoId == productoId);// buscamos el item por su ID
    // si no existe, no hacemos nada
    if (item != null)// si existe el item en el carrito
    // verificamos que la cantidad no supere el stock del producto
    {
        if (cantidad <= 0)
        {
            OnStockActualizar?.Invoke(productoId, item.Cantidad); // devolver stock
            _items.Remove(item);
        }
        else
        {
            int diferencia = cantidad - item.Cantidad;
            item.Cantidad = cantidad;
            OnStockActualizar?.Invoke(productoId, -diferencia); // positivo o negativo
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
    var item = _items.FirstOrDefault(x => x.ProductoId == productoId);// buscamos el item por su ID
    // si no existe, no hacemos nada
    if (item != null)// si existe el item en el carrito
    // devolvemos el stock del producto
    {
        OnStockActualizar?.Invoke(productoId, item.Cantidad); // devolver stock
        _items.Remove(item);
        NotificarCambios();
    }
}


    /// <summary>
    /// Vacía el carrito de compras
    /// </summary>
    public void LimpiarCarrito()
{
    foreach (var item in _items)// recorremos todos los items del carrito
    // y devolvemos el stock de cada producto
    {
        OnStockActualizar?.Invoke(item.ProductoId, item.Cantidad);// devolver stock
        // Devolvemos el stock del producto al inventario
    }
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
