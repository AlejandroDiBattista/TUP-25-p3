using System.Net.Http.Json;

namespace cliente.Services;

public class ApiService {
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    
    public async Task<DatosRespuesta> ObtenerDatosAsync() {
        try {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } catch (Exception ex) {
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }

 
    public async Task<List<Producto>> GetProductosAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Producto>>("/api/productos");
    }


    public async Task<int> CrearCarritoAsync()
    {
        var response = await _httpClient.PostAsync("/api/carritos", null);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CarritoIdResponse>();
            return result.carritoId;
        }
        throw new Exception("No se pudo crear el carrito");
    }


}

public class DatosRespuesta {
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string ImagenUrl { get; set; }
}

public class CarritoIdResponse
{
    public int carritoId { get; set; }
}
