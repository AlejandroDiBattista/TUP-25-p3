using System.Net.Http.Json;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public event Func<Task> OnCarritoChanged;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

    public async Task<List<ProductoDto>> ObtenerProductosAsync(string busqueda = "") {
        if (string.IsNullOrWhiteSpace(busqueda))
            return await _httpClient.GetFromJsonAsync<List<ProductoDto>>("/productos");
        return await _httpClient.GetFromJsonAsync<List<ProductoDto>>($"/productos/buscar/{busqueda}");
    }

    public async Task<List<ItemCarritoDto>> GetCarrito() {
        return await ObtenerCarritoAsync();
    }

    public async Task<List<ItemCarritoDto>> ObtenerCarritoAsync() {
        var carritoId = await ObtenerCarritoIdAsync();
        return await _httpClient.GetFromJsonAsync<List<ItemCarritoDto>>($"/carritos/{carritoId}");
    }

    public async Task AgregarAlCarritoAsync(int productoId, int cantidad) {
        var carritoId = await ObtenerCarritoIdAsync();
        var url = $"/carritos/{carritoId}/{productoId}?cantidad={cantidad}";
        await _httpClient.PutAsync(url, null);
        await NotificarCambioCarrito();
    }

    public async Task ActualizarCantidadAsync(int productoId, int cantidad) {
        await AgregarAlCarritoAsync(productoId, cantidad);
    }

    public async Task EliminarDelCarritoAsync(int productoId) {
        var carritoId = await ObtenerCarritoIdAsync();
        var url = $"/carritos/{carritoId}/{productoId}";
        await _httpClient.DeleteAsync(url);
        await NotificarCambioCarrito();
    }

    public async Task VaciarCarritoAsync() {
        var carritoId = await ObtenerCarritoIdAsync();
        await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        await NotificarCambioCarrito();
    }    public async Task<CompraRespuestaDto> ConfirmarCompraAsync(ClienteDto cliente) {
        try {
            var carritoId = await ObtenerCarritoIdAsync();
            
            var json = $"{{\"Nombre\":\"{cliente.Nombre}\",\"Apellido\":\"{cliente.Apellido}\",\"Email\":\"{cliente.Email}\"}}";
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/carritos/{carritoId}/confirmar", content);
            
            response.EnsureSuccessStatusCode();
            var resultado = await response.Content.ReadFromJsonAsync<CompraRespuestaDto>();
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
