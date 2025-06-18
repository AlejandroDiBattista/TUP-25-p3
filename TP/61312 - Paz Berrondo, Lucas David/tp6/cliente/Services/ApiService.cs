using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using cliente.Models;

namespace cliente.Services;

/// <summary>
/// Servicio para comunicación con la API del servidor.
/// Proporciona métodos para todas las operaciones de la tienda online.
/// </summary>
public class ApiService 
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient) 
    {
        _httpClient = httpClient;
        
        // Configurar opciones de JSON para ser case-insensitive
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // ========================================
    // MÉTODOS DE PRODUCTOS
    // ========================================

    /// <summary>
    /// Obtiene todos los productos disponibles.
    /// </summary>
    /// <returns>Lista de productos</returns>
    public async Task<List<ProductoDto>> ObtenerProductosAsync()
    {
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<ProductosRespuestaDto>("/api/productos", _jsonOptions);
            return response?.Productos ?? new List<ProductoDto>();
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al obtener productos: {ex.Message}");
            return new List<ProductoDto>();
        }
    }

    /// <summary>
    /// Busca productos por nombre.
    /// </summary>
    /// <param name="termino">Término de búsqueda</param>
    /// <returns>Lista de productos que coinciden con el término</returns>
    public async Task<List<ProductoDto>> BuscarProductosAsync(string termino)
    {
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<ProductosRespuestaDto>($"/api/productos?buscar={Uri.EscapeDataString(termino)}", _jsonOptions);
            return response?.Productos ?? new List<ProductoDto>();
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al buscar productos: {ex.Message}");
            return new List<ProductoDto>();
        }
    }

    /// <summary>
    /// Obtiene un producto específico por ID.
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <returns>Producto o null si no se encuentra</returns>
    public async Task<ProductoDto?> ObtenerProductoPorIdAsync(int id)
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<ProductoDto>($"/api/productos/{id}", _jsonOptions);
        } 
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            Console.WriteLine($"⚠️ Producto {id} no encontrado");
            return null;
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al obtener producto {id}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Obtiene el stock disponible de un producto específico.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <returns>Cantidad de stock disponible, -1 si hay error</returns>
    public async Task<int> ObtenerStockDisponibleAsync(int productoId)
    {
        try 
        {
            var producto = await ObtenerProductoPorIdAsync(productoId);
            return producto?.Stock ?? -1;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al obtener stock del producto {productoId}: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Valida si hay suficiente stock para una cantidad solicitada.
    /// </summary>
    /// <param name="productoId">ID del producto</param>
    /// <param name="cantidadSolicitada">Cantidad que se quiere agregar</param>
    /// <returns>Tupla con (esValido, stockDisponible, nombreProducto)</returns>
    public async Task<(bool esValido, int stockDisponible, string nombreProducto)> ValidarStockDisponibleAsync(int productoId, int cantidadSolicitada)
    {
        try 
        {
            var producto = await ObtenerProductoPorIdAsync(productoId);
            if (producto == null)
            {
                return (false, 0, "Producto no encontrado");
            }
            
            return (producto.Stock >= cantidadSolicitada, producto.Stock, producto.Nombre);
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al validar stock del producto {productoId}: {ex.Message}");
            return (false, 0, "Error de validación");
        }
    }

    // ========================================
    // MÉTODOS DE CARRITO
    // ========================================

    /// <summary>
    /// Crea un nuevo carrito de compras.
    /// </summary>
    /// <returns>ID del carrito creado o null si hay error</returns>
    public async Task<string?> CrearCarritoAsync()
    {
        try 
        {
            var response = await _httpClient.PostAsync("/api/carritos", null);
            if (response.IsSuccessStatusCode)
            {
                var carritoCreado = await response.Content.ReadFromJsonAsync<CarritoCreado>(_jsonOptions);
                return carritoCreado?.CarritoId;
            }
            return null;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al crear carrito: {ex.Message}");
            return null;
        }
    }    /// <summary>
    /// Obtiene el contenido de un carrito específico.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <returns>Carrito con sus items o null si no se encuentra</returns>
    public async Task<CarritoDto?> ObtenerCarritoAsync(string carritoId)
    {
        try 
        {
            var response = await _httpClient.GetAsync($"/api/carritos/{carritoId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"⚠️ Carrito {carritoId} no encontrado (404)");
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CarritoDto>(_jsonOptions);
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al obtener carrito {carritoId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Vacía completamente un carrito.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <returns>True si se vació exitosamente</returns>
    public async Task<bool> VaciarCarritoAsync(string carritoId)
    {
        try 
        {
            var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}");
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al vaciar carrito {carritoId}: {ex.Message}");
            return false;
        }
    }    /// <summary>
    /// Agrega o actualiza un producto en el carrito.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <param name="productoId">ID del producto</param>
    /// <param name="cantidad">Cantidad del producto</param>
    /// <returns>True si se agregó exitosamente, False si el carrito no existe</returns>
    public async Task<bool> AgregarProductoAlCarritoAsync(string carritoId, int productoId, int cantidad)
    {
        try 
        {
            var requestDto = new ActualizarItemCarritoDto { Cantidad = cantidad };
            var json = JsonSerializer.Serialize(requestDto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/carritos/{carritoId}/{productoId}", content);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"⚠️ Carrito {carritoId} no encontrado al agregar producto {productoId}");
                return false;
            }
            
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al agregar producto {productoId} al carrito {carritoId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Elimina un producto específico del carrito.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <param name="productoId">ID del producto a eliminar</param>
    /// <returns>True si se eliminó exitosamente</returns>
    public async Task<bool> EliminarProductoDelCarritoAsync(string carritoId, int productoId)
    {
        try 
        {
            var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/{productoId}");
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al eliminar producto {productoId} del carrito {carritoId}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Confirma una compra convirtiendo el carrito en una compra persistente.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <param name="datosCliente">Datos del cliente para la compra</param>
    /// <returns>Detalles de la compra confirmada o null si hay error</returns>
    public async Task<CompraConfirmadaDto?> ConfirmarCompraAsync(string carritoId, ConfirmarCompraDto datosCliente)
    {
        try 
        {
            var json = JsonSerializer.Serialize(datosCliente, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/carritos/{carritoId}/confirmar", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CompraConfirmadaDto>(_jsonOptions);
            }
            
            // Si hay error, leer el mensaje de error
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Error al confirmar compra: {errorContent}");
            return null;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al confirmar compra del carrito {carritoId}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Verifica si un carrito existe en el servidor.
    /// </summary>
    /// <param name="carritoId">ID del carrito a verificar</param>
    /// <returns>True si el carrito existe, False si no</returns>
    public async Task<bool> CarritoExisteAsync(string carritoId)
    {
        try 
        {
            var response = await _httpClient.GetAsync($"/api/carritos/{carritoId}");
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al verificar carrito {carritoId}: {ex.Message}");
            return false;
        }
    }

    // ========================================
    // MÉTODOS DE UTILIDAD
    // ========================================

    /// <summary>
    /// Método legacy para obtener datos del servidor (ejemplo).
    /// </summary>
    /// <returns>Datos de ejemplo del servidor</returns>
    public async Task<DatosRespuesta> ObtenerDatosAsync() 
    {
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos", _jsonOptions);
            return response ?? new DatosRespuesta { Mensaje = "No se recibieron datos del servidor", Fecha = DateTime.Now };
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al obtener datos: {ex.Message}");
            return new DatosRespuesta { Mensaje = $"Error: {ex.Message}", Fecha = DateTime.Now };
        }
    }
}

/// <summary>
/// DTO para respuesta de datos del servidor (ejemplo legacy).
/// </summary>
public class DatosRespuesta 
{
    public string Mensaje { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
