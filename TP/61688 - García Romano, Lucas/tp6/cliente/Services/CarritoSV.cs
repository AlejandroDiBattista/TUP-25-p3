#nullable enable
//evita el warning de referencia nula

using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace cliente.Services;

//Maneja el carrito de compras
public class CarritoService
{
    private readonly HttpClient _httpClient;
    private string carritoId = string.Empty;

//recibe el HttpClient y hace una peticion a la API
    public CarritoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

//un evento que se dispara cuando el carrito se actualiza
    public event Action? OnCarritoActualizado;

    //Obtiene o crea el carrito

    public async Task<string> ObtenerOCrearCarritoAsync()
    //si existe el carrito lo devuelve sino lo crea
    {
        //Intenta obtener el carrito
        if (!string.IsNullOrEmpty(carritoId))

            //Si ya tiene un carrito, lo devuelve
            return carritoId;

        //Hace una petición a la API para crear un nuevo carrito
        var response = await _httpClient.PostAsync("/carritos", null);
        // Si el carrito existe lo devuelve
        if (response.IsSuccessStatusCode)
        {
            // Lee la respuesta y obtiene el ID del carrito
            var json = await response.Content.ReadAsStringAsync();

            // Analiza el JSON para obtener el ID del carrito
            carritoId = JsonDocument.Parse(json).RootElement.GetProperty("id").GetString() ?? string.Empty;
        }
        return carritoId;
        //Si no se encontro devolvera un string vacio
    }

    public async Task<List<ItemCarritoDto>> ObtenerItemsAsync() //Obtiene los items del carrito
    {
        var id = await ObtenerOCrearCarritoAsync();// Obtiene o crea el carrito
        if (string.IsNullOrEmpty(id)) return new List<ItemCarritoDto>(); //retorna si no hay carrito
        return await _httpClient.GetFromJsonAsync<List<ItemCarritoDto>>($"/carritos/{id}") ?? new List<ItemCarritoDto>(); //si no hay items da una lista vacia
    }

    public async Task AgregarProductoAsync(int productoId, int cantidad)//Agrega un producto al carrito
    {
        var id = await ObtenerOCrearCarritoAsync();// Obtiene o crea el carrito
        if (string.IsNullOrEmpty(id)) return;//retorna si es nulo

        var datos = new { Cantidad = cantidad };//crea un objeto en blanco con la cantidad del producto
        var contenido = new StringContent(JsonSerializer.Serialize(datos), Encoding.UTF8, "application/json");//serializa los objetos a JSON
        var response = await _httpClient.PutAsync($"/carritos/{id}/{productoId}", contenido);//Hace una peticion para agregar el producto al carrito
        response.EnsureSuccessStatusCode();//se asegura que sea exitosa la peticion
        OnCarritoActualizado?.Invoke();//lanza el evento de carrito actualizado
    }

    public async Task VaciarCarritoAsync()//vacia el carrtito
    {
        var id = await ObtenerOCrearCarritoAsync();//obtiene o crea el carrito
        if (string.IsNullOrEmpty(id)) return;//retorna si es nulo
        await _httpClient.GetAsync($"/carritos/vaciar/{id}");//hace una peticion para vaciar el carrito
        OnCarritoActualizado?.Invoke();//lanza el evento de carrito actualizado
    }

    public async Task ConfirmarCompraAsync(DatosCliente cliente)//confirma la compra
    {
        var id = await ObtenerOCrearCarritoAsync();// Obtiene o crea el carrito
        if (string.IsNullOrEmpty(id)) return;//retorna si es nulo

        var contenido = new StringContent(JsonSerializer.Serialize(cliente), Encoding.UTF8, "application/json");//serializa los objetos a JSON
        await _httpClient.PutAsync($"/carritos/{id}/finalizar", contenido);// Hace una petición para confirmar la compra
        carritoId = string.Empty; //limpia el carritoId
    }

    public async Task<List<ItemCarritoDto>> ObtenerItemsDelCarritoAsync()// Obtiene los items del carrito
    {
        return await ObtenerItemsAsync();//retorna los items del carrito
    }

    public async Task ModificarCantidadAsync(int productoId, int nuevaCantidad)//cambia la cantidad de un producto en el carrito
    {
        var id = await ObtenerOCrearCarritoAsync(); //crea o obtiene el carrito
        if (string.IsNullOrEmpty(id)) return; //retorna si es nulo

        var datos = new { Cantidad = nuevaCantidad }; //crea un objeto blanco con la cantidad del producto
        var contenido = new StringContent(JsonSerializer.Serialize(datos), Encoding.UTF8, "application/json");//serializa los objetos a json
        var response = await _httpClient.PutAsync($"/carritos/{id}/{productoId}", contenido); //hace una peticion para modificar la cantidad en el carrito
        var responseContent = await response.Content.ReadAsStringAsync(); //lee el contenido de la respuesta
        if (!response.IsSuccessStatusCode)//si no hay respuesta exitosa manda una exepcion
        {
            try
            {
                var errorObj = JsonSerializer.Deserialize<JsonElement>(responseContent);//deserializa el contenido de la respuesta
                var mensaje = errorObj.TryGetProperty("Mensaje", out var prop) ? prop.GetString() : response.ReasonPhrase;//obtiene el mensaje de error
                throw new Exception(mensaje);//lanza una exepcion con el msj de error
            }
            catch
            {
                throw new Exception(response.ReasonPhrase); //si no se pudo deserializar,lanza una exepcion con el mensaje de la respuesta
            }
        }
        OnCarritoActualizado?.Invoke(); //lanza el evento de carrito actualizado
    }
}

// DTO para los items del carrito
public class ItemCarritoDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;// Nombre del producto
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Stock { get; set; } // Stock disponible
    public decimal Subtotal => Cantidad * PrecioUnitario; //calcula el subtotal del item
}

// DTO para datos del cliente
public class DatosCliente
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El email debe ser válido y contener @")]
    public string Email { get; set; } = string.Empty;
}
