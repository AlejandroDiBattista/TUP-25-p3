using System.Net.Http.Json;
using cliente.Models;
#nullable enable

namespace cliente.Services;

public partial class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
   public async Task<List<CompraResumen>> ObtenerHistorialCompras()
    {
        return await _httpClient.GetFromJsonAsync<List<CompraResumen>>("/api/compras");
    }
    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos" };
        }
        catch (Exception ex)
        {
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}" };
        }
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Producto>>("/api/productos");
            return response ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo productos: {ex.Message}");
            return new List<Producto>();
        }
    }

    public async Task<Carrito> CrearCarrito()
    {
        try
        {
            var response = await _httpClient.PostAsync("/api/carritos", null);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Carrito>() ?? new Carrito();
            }
            
            Console.WriteLine($"Error creando carrito: {response.StatusCode}");
            return new Carrito();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción al crear carrito: {ex.Message}");
            return new Carrito();
        }
    }

    public async Task<Carrito> ObtenerCarrito(int carritoId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Carrito>($"/api/carritos/{carritoId}") ?? new Carrito();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo carrito: {ex.Message}");
            return new Carrito();
        }
    }

    public async Task<bool> AgregarProductoAlCarrito(int carritoId, int productoId, int cantidad)
    {
        try
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"/api/carritos/{carritoId}/{productoId}",
            cantidad // Envía solo el número, no un objeto
        );
        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error agregando producto: {ex.Message}");
        return false;
    }
    }

    public async Task<(bool Success, string Message)> QuitarProductoDelCarrito(int carritoId, int productoId)
{
    try
    {
        var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }
        return (true, "Producto eliminado");
    }
    catch (Exception ex)
    {
        return (false, ex.Message);
    }
}

    public async Task<(bool Success, string Message)> VaciarCarrito(int carritoId)
{
    try
    {
        var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }
        return (true, "Operación exitosa");
    }
    catch (Exception ex)
    {
        return (false, ex.Message);
    }
}

public async Task<CompraResumen?> ConfirmarCompra(int carritoId, CompraDTO compra)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", compra);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CompraResumen>();
        }
        return null;
    }
}