using System.Net.Http.Json;
using System.Collections.Generic;
using cliente.Models;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
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
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }

    public async Task<string> CrearCarritoAsync()
    {
        var response = await _httpClient.PostAsync("/api/carritos", null);
        if (response.IsSuccessStatusCode)
        {
            var id = await response.Content.ReadAsStringAsync();
            return id.Trim('"');
        }
        return "";
    }

    public async Task<List<CarritoItemDto>> ObtenerCarritoAsync(string carritoId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/carritos/{carritoId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new List<CarritoItemDto>();

            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadFromJsonAsync<List<CarritoItemDto>>();
            return items ?? new List<CarritoItemDto>();
        }
        catch
        {
            return new List<CarritoItemDto>();
        }
    }

    public async Task AgregarAlCarritoAsync(string carritoId, int productoId)
    {
        await _httpClient.PutAsync($"/api/carritos/{carritoId}/{productoId}", null);
    }

    public async Task QuitarDelCarritoAsync(string carritoId, int productoId)
    {
        await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
    }

    public async Task VaciarCarritoAsync(string carritoId)
    {
        await _httpClient.DeleteAsync($"/api/carritos/{carritoId}");
    }

    public async Task<bool> ConfirmarCompraAsync(string carritoId, object compra)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/carritos/{carritoId}/confirmar", compra);
        await ObtenerProductosAsync();
        return response.IsSuccessStatusCode;
    }

    public async Task CambiarCantidadCarritoAsync(string carritoId, int productoId, int delta)
    {
        var response = await _httpClient.PutAsync(
            $"/api/carritos/{carritoId}/cantidad/{productoId}?delta={delta}", null);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new Exception($"No se pudo cambiar la cantidad: {errorMsg}");
        }
    }
}

public class DatosRespuesta
{
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}

public class CarritoItemDto
{
    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; }
    public int Cantidad { get; set; }
    public decimal ProductoPrecio { get; set; }
    public string ProductoImagenUrl { get; set; }
}