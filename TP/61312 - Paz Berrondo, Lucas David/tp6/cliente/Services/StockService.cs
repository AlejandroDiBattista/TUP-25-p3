using cliente.Models;

namespace cliente.Services;

/// <summary>
/// Servicio para manejar el estado del stock de productos en tiempo real.
/// Permite sincronizar el stock entre diferentes páginas cuando se agregan/eliminan productos del carrito.
/// </summary>
public class StockService
{
    private readonly ApiService _apiService;
    private Dictionary<int, int> _stockCache = new();

    // Evento para notificar cambios de stock
    public event Action<int, int>? StockCambiado; // productoId, nuevoStock

    public StockService(ApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Actualiza el stock de un producto en la cache y notifica a los suscriptores.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <param name="nuevoStock">Nuevo stock del producto</param>
    public void ActualizarStock(int productoId, int nuevoStock)
    {
        _stockCache[productoId] = nuevoStock;
        StockCambiado?.Invoke(productoId, nuevoStock);
    }

    /// <summary>
    /// Disminuye el stock de un producto cuando se agrega al carrito.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <param name="cantidad">Cantidad agregada</param>
    public void DisminuirStock(int productoId, int cantidad)
    {
        if (_stockCache.ContainsKey(productoId))
        {
            var nuevoStock = Math.Max(0, _stockCache[productoId] - cantidad);
            ActualizarStock(productoId, nuevoStock);
        }
    }

    /// <summary>
    /// Aumenta el stock de un producto cuando se elimina del carrito.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <param name="cantidad">Cantidad eliminada</param>
    public void AumentarStock(int productoId, int cantidad)
    {
        if (_stockCache.ContainsKey(productoId))
        {
            var nuevoStock = _stockCache[productoId] + cantidad;
            ActualizarStock(productoId, nuevoStock);
        }
    }

    /// <summary>
    /// Obtiene el stock actual de un producto desde la cache.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <returns>Stock actual o -1 si no está en cache</returns>
    public int ObtenerStockCache(int productoId)
    {
        return _stockCache.ContainsKey(productoId) ? _stockCache[productoId] : -1;
    }

    /// <summary>
    /// Inicializa la cache de stock con una lista de productos.
    /// </summary>
    /// <param name="productos">Lista de productos</param>
    public void InicializarCache(List<ProductoDto> productos)
    {
        _stockCache.Clear();
        foreach (var producto in productos)
        {
            _stockCache[producto.Id] = producto.Stock;
        }
    }    /// <summary>
    /// Sincroniza el stock de todos los productos con el servidor.
    /// </summary>
    /// <param name="carritoId">ID del carrito para calcular stock disponible</param>
    /// <param name="terminoBusqueda">Término de búsqueda opcional</param>
    /// <returns>Lista de productos con stock actualizado</returns>
    public async Task<List<ProductoDto>> SincronizarStockAsync(string? carritoId = null, string? terminoBusqueda = null)
    {
        try
        {
            // Obtener productos con o sin búsqueda
            var productos = string.IsNullOrWhiteSpace(terminoBusqueda) 
                ? await _apiService.ObtenerProductosAsync()
                : await _apiService.BuscarProductosAsync(terminoBusqueda);
            
            // Si tenemos carrito, obtener el stock disponible real
            if (!string.IsNullOrEmpty(carritoId))
            {
                foreach (var producto in productos)
                {
                    var (stockTotal, cantidadEnCarrito, stockDisponible, _) = 
                        await _apiService.ObtenerStockDisponibleAsync(producto.Id, carritoId);
                    
                    // Actualizar el stock visual con el stock disponible
                    producto.Stock = stockDisponible;
                }
            }

            // Actualizar cache con los valores reales
            InicializarCache(productos);
            
            return productos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al sincronizar stock: {ex.Message}");
            return new List<ProductoDto>();
        }
    }

    /// <summary>
    /// Limpia la cache de stock.
    /// </summary>
    public void LimpiarCache()
    {
        _stockCache.Clear();
    }
}
