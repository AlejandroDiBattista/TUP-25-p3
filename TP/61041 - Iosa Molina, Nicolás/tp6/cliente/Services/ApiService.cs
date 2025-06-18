using System.Net.Http.Json;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;
    private Dictionary<int, int> stockLocalModifications = new Dictionary<int, int>();
    
    public event Func<Task> OnCarritoChanged;
    public event Func<Task> OnStockChanged;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }    public async Task<List<ProductoDto>> ObtenerProductosAsync(string busqueda = "") {
        List<ProductoDto> productos;
        
        if (string.IsNullOrWhiteSpace(busqueda))
            productos = await _httpClient.GetFromJsonAsync<List<ProductoDto>>("/productos");
        else
            productos = await _httpClient.GetFromJsonAsync<List<ProductoDto>>($"/productos/buscar/{busqueda}");
        
        // Aplicar modificaciones locales de stock
        if (productos != null)
        {
            foreach (var producto in productos)
            {
                var modificacion = GetStockModification(producto.Id);
                if (modificacion > 0)
                {
                    producto.Stock -= modificacion;
                    if (producto.Stock < 0) producto.Stock = 0; // Prevenir stock negativo
                }
            }
        }
        
        return productos;
    }

    public async Task<List<ItemCarritoDto>> GetCarrito() {
        return await ObtenerCarritoAsync();
    }

    public async Task<List<ItemCarritoDto>> ObtenerCarritoAsync() {
        var carritoId = await ObtenerCarritoIdAsync();
        return await _httpClient.GetFromJsonAsync<List<ItemCarritoDto>>($"/carritos/{carritoId}");
    }    public async Task AgregarAlCarritoAsync(int productoId, int cantidad, int cantidadAnterior = 0) {
        var carritoId = await ObtenerCarritoIdAsync();
        var url = $"/carritos/{carritoId}/{productoId}?cantidad={cantidad}";
        await _httpClient.PutAsync(url, null);
        
        // Calcular la diferencia entre la cantidad nueva y la anterior
        int diferencia = cantidad - cantidadAnterior;
        
        if (diferencia > 0) {
            // Si estamos añadiendo más productos, aumentar el stock local usado
            ActualizarStockLocal(productoId, diferencia);
            await NotificarCambioStock();
        }
        // No necesitamos hacer nada aquí si diferencia < 0, porque ya lo manejamos en ActualizarCantidadAsync
        
        await NotificarCambioCarrito();
    }public async Task ActualizarCantidadAsync(int productoId, int nuevaCantidad) {
        // Obtener la cantidad actual para comparar
        var cantidadActual = 0;
        var carritoActual = await ObtenerCarritoAsync();
        var itemEnCarrito = carritoActual.FirstOrDefault(i => i.ProductoId == productoId);
        if (itemEnCarrito != null) {
            cantidadActual = itemEnCarrito.Cantidad;
        }
        
        // Si la nueva cantidad es menor que la actual, actualizar el stock local
        if (nuevaCantidad < cantidadActual && stockLocalModifications.ContainsKey(productoId)) {
            int diferencia = cantidadActual - nuevaCantidad;
            stockLocalModifications[productoId] = Math.Max(0, stockLocalModifications[productoId] - diferencia);
            if (stockLocalModifications[productoId] == 0) {
                stockLocalModifications.Remove(productoId);
            }
            await NotificarCambioStock();
        }
        
        // Usar el método existente para agregar al carrito (que manejará aumentos)
        await AgregarAlCarritoAsync(productoId, nuevaCantidad, cantidadActual);
    }public async Task EliminarDelCarritoAsync(int productoId) {
        // Obtener la cantidad actual en el carrito para actualizar el stock
        var cantidadActual = 0;
        var carritoActual = await ObtenerCarritoAsync();
        var itemEnCarrito = carritoActual.FirstOrDefault(i => i.ProductoId == productoId);
        if (itemEnCarrito != null) {
            cantidadActual = itemEnCarrito.Cantidad;
        }
        
        var carritoId = await ObtenerCarritoIdAsync();
        var url = $"/carritos/{carritoId}/{productoId}";
        await _httpClient.DeleteAsync(url);
        
        // Quitar del stock local esta cantidad
        if (cantidadActual > 0 && stockLocalModifications.ContainsKey(productoId)) {
            stockLocalModifications[productoId] = Math.Max(0, stockLocalModifications[productoId] - cantidadActual);
            if (stockLocalModifications[productoId] == 0) {
                stockLocalModifications.Remove(productoId);
            }
            await NotificarCambioStock();
        }
        
        await NotificarCambioCarrito();
    }public async Task VaciarCarritoAsync() {
        var carritoId = await ObtenerCarritoIdAsync();
        await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        
        // Resetear todos los cambios locales de stock
        stockLocalModifications.Clear();
        
        await NotificarCambioCarrito();
    }    public async Task<CompraRespuestaDto> ConfirmarCompraAsync(ClienteDto cliente) {
        try {
            var carritoId = await ObtenerCarritoIdAsync();
            
            var json = $"{{\"Nombre\":\"{cliente.Nombre}\",\"Apellido\":\"{cliente.Apellido}\",\"Email\":\"{cliente.Email}\"}}";
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/carritos/{carritoId}/confirmar", content);
            
            response.EnsureSuccessStatusCode();
            var resultado = await response.Content.ReadFromJsonAsync<CompraRespuestaDto>();
            
            // Resetear todos los cambios locales de stock después de confirmar la compra
            stockLocalModifications.Clear();
            
            await NotificarCambioCarrito();
            return resultado;
        }
        catch (Exception) {
            throw;
        }
    }

    private string _carritoId;
    private async Task<string> ObtenerCarritoIdAsync() {
        if (!string.IsNullOrEmpty(_carritoId)) return _carritoId;
        var resp = await _httpClient.PostAsync("/carritos", null);
        var data = await resp.Content.ReadFromJsonAsync<CarritoRespuestaDto>();
        _carritoId = data.Carrito;
        return _carritoId;
    }
    
    private async Task NotificarCambioCarrito()
    {
        if (OnCarritoChanged != null)
        {
            await OnCarritoChanged.Invoke();
        }
    }
      // Método para actualizar el stock local
    public void ActualizarStockLocal(int productoId, int cantidadRestada)
    {
        if (stockLocalModifications.ContainsKey(productoId))
        {
            stockLocalModifications[productoId] += cantidadRestada;
        }
        else
        {
            stockLocalModifications[productoId] = cantidadRestada;
        }
    }
    
    // Método para obtener la modificación de stock local
    public int GetStockModification(int productoId)
    {
        return stockLocalModifications.ContainsKey(productoId) ? stockLocalModifications[productoId] : 0;
    }

    private async Task NotificarCambioStock()
    {
        if (OnStockChanged != null)
        {
            await OnStockChanged.Invoke();
        }
    }
}

public class ProductoDto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public double Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }
}

public class ItemCarritoDto {
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; }
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
}

public class ClienteDto {
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
}

public class CompraRespuestaDto {
    public int Id { get; set; }
    public double Total { get; set; }
}

public class CarritoRespuestaDto {
    public string Carrito { get; set; }
}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
