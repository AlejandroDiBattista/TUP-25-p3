using System.Net.Http.Json;
using System.Net.Http;
using cliente.Models;

namespace cliente.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        Console.WriteLine("Llamando a /productos");

        try
        {
            var response = await _httpClient.GetAsync("/productos");

            response.EnsureSuccessStatusCode();
            var productos = await response.Content.ReadFromJsonAsync<List<Producto>>();
            return productos ?? new List<Producto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
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

    public async Task<bool> ConfirmarCompraAsync(Orden orden)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/ordenes", orden);
            Console.WriteLine($"POST /api/ordenes => {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al confirmar compra: {ex.Message}");
            return false;
        }
    }



    
}
