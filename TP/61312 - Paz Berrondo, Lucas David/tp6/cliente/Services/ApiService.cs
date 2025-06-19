using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using cliente.Models;

namespace cliente.Services;

public class ApiService 
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient) 
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    // PRODUCTOS

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
        }    }

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
        }    }

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
        }    }

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
        }    }

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

    public async Task<(int stockTotal, int cantidadEnCarrito, int stockDisponible, string nombreProducto)> ObtenerStockDisponibleAsync(int productoId, string carritoId)
    {
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<StockDisponibleDto>($"/api/productos/{productoId}/stock-disponible/{carritoId}", _jsonOptions);
            if (response != null)
            {
                return (response.StockTotal, response.CantidadEnCarrito, response.StockDisponible, response.NombreProducto);
            }
            return (0, 0, 0, "Error");
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al obtener stock disponible del producto {productoId}: {ex.Message}");
            return (0, 0, 0, "Error de conexión");
        }
    }

    // CARRITO

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
        }    }

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
    }

    public async Task<bool> AgregarProductoAlCarritoAsync(string carritoId, int productoId, int cantidad)
    {
        try 
        {
            var response = await _httpClient.PutAsync($"/api/carritos/{carritoId}/productos/{productoId}?cantidad={cantidad}", null);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"⚠️ Carrito {carritoId} no encontrado al agregar producto {productoId}");
                return false;
            }
            
            return response.IsSuccessStatusCode;        }
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al agregar producto {productoId} al carrito {carritoId}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EliminarProductoDelCarritoAsync(string carritoId, int productoId)
    {
        try 
        {
            var response = await _httpClient.DeleteAsync($"/api/carritos/{carritoId}/productos/{productoId}");
            return response.IsSuccessStatusCode;
        } 
        catch (Exception ex) 
        {
            Console.WriteLine($"❌ Error al eliminar producto {productoId} del carrito {carritoId}: {ex.Message}");
            return false;
        }
    }

    public async Task<CompraConfirmadaDto?> ConfirmarCompraAsync(string carritoId, ConfirmarCompraDto datosCliente)
    {
        try 
        {
            var json = JsonSerializer.Serialize(datosCliente, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/carritos/{carritoId}/confirmar", content);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CompraConfirmadaDto>(_jsonOptions);
            }
            
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
