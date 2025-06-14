using System.Net.Http.Json;


namespace cliente.Services;

public class ApiService
{
    // Realiza peticones de HTTP a la API
    private readonly HttpClient _httpClient;
    // Ek httpCliente realiza peticones a la API
    public ApiService(HttpClient httpClient)
    {
        //Inicia el httpCliente
        _httpClient = httpClient;
        //Configura la URL de la API
    }

    public async Task<DatosRespuesta> ObtenerDatosAsync()
    {
        //Obitiene datos del servidor
        try
        // Realiza una petición GET a la API para obtener datos
        {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
            //SI es null, devuelve erro
        }
        catch (Exception ex)
        //Si hay un error manda exepcion
        {
            //Muestra el error en la consola
            Console.WriteLine($"Error al obtener datos: {ex.Message}");
            //Devuelve un mensaje con el error y la fecha
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }
}

public class DatosRespuesta
{
    //es el mensaje que da el servidor
    public string Mensaje { get; set; }
    public DateTime Fecha { get; set; }
}

public class ProductoService
{
    private readonly HttpClient _httpClient;

    public ProductoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //obtener productos
    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        try
        {
            //Realiza una peticion a la API para obtener la lista de productos
            var productos = await _httpClient.GetFromJsonAsync<List<Producto>>("/api/producto");
            //Si la lista es null, devuelve una lista vacía
            return productos ?? new List<Producto>();
            // Si no es null, devuelve la lista de productos
        }
        catch (Exception ex)
        {
            //Si hay un error lanza una expecion y lo muestra en la consola
            Console.WriteLine($"Error al obtener productos: {ex.Message}");
            return new List<Producto>();
        }
    }
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
