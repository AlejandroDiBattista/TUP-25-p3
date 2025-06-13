using System.Net.Http.Json;
namespace cliente.Services;
using cliente.Models;


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
    public async Task<List<Compra>> ObtenerComprasAsync()
{
    try
    {
        var compras = await _httpClient.GetFromJsonAsync<List<Compra>>("/api/compras");
        return compras ?? new List<Compra>();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al obtener compras: {ex.Message}");
        return new List<Compra>();
    }
}
public async Task<bool> AgregarCompraAsync(Compra nuevaCompra)
{
    try
    {
        var respuesta = await _httpClient.PostAsJsonAsync("/api/compras", nuevaCompra);
        return respuesta.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al agregar compra: {ex.Message}");
        return false;
    }
}
public async Task<bool> EliminarCompraAsync(int id)
{
    try
    {
        var respuesta = await _httpClient.DeleteAsync($"/api/compras/{id}");
        return respuesta.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al eliminar compra: {ex.Message}");
        return false;
    }
}

}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}
