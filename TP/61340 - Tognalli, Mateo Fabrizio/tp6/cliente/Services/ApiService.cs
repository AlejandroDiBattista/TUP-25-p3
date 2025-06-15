using System.Net.Http.Json;
using Cliente.Models;

namespace Cliente.Services;

public class ApiService 
{
    private readonly HttpClient _httpClient;    public ApiService(HttpClient httpClient) 
    {
        _httpClient = httpClient;
    }

    // === PRODUCTOS ===
    public async Task<List<ProductoDto>> ObtenerProductosAsync(string busqueda = null)
    {
        try 
        {
            var url = "/productos";
            if (!string.IsNullOrEmpty(busqueda))
                url += $"?busqueda={Uri.EscapeDataString(busqueda)}";
                
            var productos = await _httpClient.GetFromJsonAsync<List<ProductoDto>>(url);
            return productos ?? new List<ProductoDto>();
        } 
        catch (Exception ex) 
        {            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<ProductoDto>();
        }
    }

    // === CARRITO ===
    public async Task<string> CrearCarritoAsync()
    {
        try 
        {
            var response = await _httpClient.PostAsync("/carritos", null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return result?["carritoId"];
            }
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"Error al crear carrito: {ex.Message}");        }
        return null;
    }

    public async Task<CarritoDto> ObtenerCarritoAsync(string carritoId)
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<CarritoDto>($"/carritos/{carritoId}");
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"Error al obtener carrito: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AgregarProductoCarritoAsync(string carritoId, int productoId, int cantidad = 1)
    {
        try 
        {
            var response = await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"Error al agregar producto al carrito: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EliminarProductoCarritoAsync(string carritoId, int productoId, int cantidad = 1)
    {
        try 
        {
            var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}");
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"Error al eliminar producto del carrito: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> VaciarCarritoAsync(string carritoId)
    {
        try 
        {
            var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"Error al vaciar carrito: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ConfirmarCompraAsync(string carritoId, ConfirmarCompraDto datosCliente)
    {
        try 
        {
            var response = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datosCliente);
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"Error al confirmar compra: {ex.Message}");
            return false;
        }
    }
}
