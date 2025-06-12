#nullable enable
using System.Net.Http.Json;

namespace Cliente.Services;

public class CarritoService
{
    private readonly HttpClient _httpClient;
    private int _carritoId = 0;

    public CarritoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> ObtenerCarritoIdAsync()
    {
        if (_carritoId == 0)
        {
            var response = await _httpClient.PostAsync("/carritos", null);
            if (response.IsSuccessStatusCode)
            {
                _carritoId = await response.Content.ReadFromJsonAsync<int>();
            }
        }
        return _carritoId;
    }    public async Task<Models.Compra?> ObtenerCarritoAsync()
    {
        var carritoId = await ObtenerCarritoIdAsync();
        var response = await _httpClient.GetAsync($"/carritos/{carritoId}");
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Models.Compra>();
        }
        
        // Si el carrito no existe (404), crear uno nuevo
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _carritoId = 0; // Reset para forzar la creación de un nuevo carrito
            carritoId = await ObtenerCarritoIdAsync();
            response = await _httpClient.GetAsync($"/carritos/{carritoId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Models.Compra>();
            }
        }
        
        return null;
    }    public async Task<bool> AgregarProductoAsync(int productoId, int cantidad = 1)
    {
        var carritoId = await ObtenerCarritoIdAsync();
        var response = await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        
        // Si el carrito no existe, crear uno nuevo e intentar de nuevo
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _carritoId = 0; // Reset para forzar la creación de un nuevo carrito
            carritoId = await ObtenerCarritoIdAsync();
            response = await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        }
        
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ModificarCantidadAsync(int productoId, int cantidad)
    {
        var carritoId = await ObtenerCarritoIdAsync();
        var response = await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        
        // Si el carrito no existe, crear uno nuevo e intentar de nuevo
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _carritoId = 0; // Reset para forzar la creación de un nuevo carrito
            carritoId = await ObtenerCarritoIdAsync();
            response = await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null);
        }
        
        return response.IsSuccessStatusCode;
    }    public async Task<bool> EliminarProductoAsync(int productoId)
    {
        var carritoId = await ObtenerCarritoIdAsync();
        var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}");
        
        // Si el carrito no existe, no hay nada que eliminar, devolvemos true
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return true; // No hay error si no existe el carrito o producto
        }
        
        return response.IsSuccessStatusCode;
    }    public async Task<bool> VaciarCarritoAsync()
    {
        var carritoId = await ObtenerCarritoIdAsync();
        var response = await _httpClient.DeleteAsync($"/carritos/{carritoId}");
        
        // Si el carrito no existe, consideramos que ya está vacío
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return true; // Ya está vacío, no hay error
        }
        
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ConfirmarCompraAsync(string nombre, string apellido, string email)
    {
        var carritoId = await ObtenerCarritoIdAsync();
        var datos = new Models.Compra 
        { 
            NombreCliente = nombre, 
            ApellidoCliente = apellido, 
            EmailCliente = email 
        };
        
        var response = await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datos);
        if (response.IsSuccessStatusCode)
        {
            _carritoId = 0; // Resetear para crear nuevo carrito
            return true;
        }
        return false;
    }    public async Task<int> ObtenerCantidadItemsAsync()
    {
        try
        {
            var carrito = await ObtenerCarritoAsync();
            return carrito?.Items?.Sum(i => i.Cantidad) ?? 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener cantidad de items: {ex.Message}");
            return 0;
        }
    }

    public void ResetearCarrito()
    {
        _carritoId = 0; // Esto forzará la creación de un nuevo carrito
    }
}
