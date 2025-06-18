using cliente.Models;

namespace cliente.Services;

public class StockService
{
    private readonly ApiService _apiService;
    private Dictionary<int, int> _stockCache = new();

    public event Action<int, int>? StockCambiado; // productoId, nuevoStock

    public StockService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public void ActualizarStock(int productoId, int nuevoStock)
    {
        _stockCache[productoId] = nuevoStock;
        StockCambiado?.Invoke(productoId, nuevoStock);
    }

    public void DisminuirStock(int productoId, int cantidad)
    {
        if (_stockCache.ContainsKey(productoId))
        {
            var nuevoStock = Math.Max(0, _stockCache[productoId] - cantidad);
            ActualizarStock(productoId, nuevoStock);
        }
    }

    public void AumentarStock(int productoId, int cantidad)
    {
        if (_stockCache.ContainsKey(productoId))
        {
            var nuevoStock = _stockCache[productoId] + cantidad;
            ActualizarStock(productoId, nuevoStock);
        }
    }

    public int ObtenerStockCache(int productoId)
    {
        return _stockCache.ContainsKey(productoId) ? _stockCache[productoId] : -1;
    }

    public void InicializarCache(List<ProductoDto> productos)
    {
        _stockCache.Clear();
        foreach (var producto in productos)
        {
            _stockCache[producto.Id] = producto.Stock;
        }
    }/// <summary>
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
            }            InicializarCache(productos);
            return productos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al sincronizar stock: {ex.Message}");
            return new List<ProductoDto>();
        }
    }

    public void LimpiarCache()
    {
        _stockCache.Clear();
    }
}
