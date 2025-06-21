#nullable enable
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Cliente.Constants;

namespace Cliente.Services;

public class CarritoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CarritoService> _logger;
    private int _carritoId = 0;

    public CarritoService(HttpClient httpClient, ILogger<CarritoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }    public async Task<int> ObtenerCarritoIdAsync()
    {
        if (_carritoId == 0)
        {
            try
            {
                var response = await _httpClient.PostAsync("/carritos", null);
                if (response.IsSuccessStatusCode)
                {
                    _carritoId = await response.Content.ReadFromJsonAsync<int>();
                    _logger.LogInformation("Nuevo carrito creado con ID: {CarritoId}", _carritoId);
                }
                else
                {
                    _logger.LogError("Error al crear carrito. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carrito ID");
                throw;
            }
        }
        return _carritoId;
    }

    private async Task<HttpResponseMessage> ExecuteWithCarritoRetryAsync(
        Func<int, Task<HttpResponseMessage>> operation)
    {
        try
        {
            var carritoId = await ObtenerCarritoIdAsync();
            var response = await operation(carritoId);
            

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Carrito {CarritoId} no encontrado, creando nuevo carrito", carritoId);
                _carritoId = 0; // Reset para forzar la creación de un nuevo carrito
                carritoId = await ObtenerCarritoIdAsync();
                response = await operation(carritoId);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en operación de carrito");
            throw;
        }
    }    public async Task<Models.Compra?> ObtenerCarritoAsync()
    {
        try
        {
            var response = await ExecuteWithCarritoRetryAsync(async carritoId =>
                await _httpClient.GetAsync($"/carritos/{carritoId}"));
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Models.Compra>();
            }
            
            _logger.LogWarning("No se pudo obtener el carrito. Status: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener carrito");
            return null;
        }
    }    public async Task<bool> AgregarProductoAsync(int productoId, int cantidad = 1)
    {
        try
        {
            var response = await ExecuteWithCarritoRetryAsync(async carritoId =>
                await _httpClient.PutAsync($"/carritos/{carritoId}/{productoId}?cantidad={cantidad}", null));
            
            var success = response.IsSuccessStatusCode;
            if (success)
            {
                _logger.LogInformation("Producto {ProductoId} agregado al carrito (cantidad: {Cantidad})", productoId, cantidad);
            }
            else
            {
                _logger.LogWarning("Error al agregar producto {ProductoId}. Status: {StatusCode}", productoId, response.StatusCode);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar producto {ProductoId} al carrito", productoId);
            return false;
        }
    }    public async Task<bool> ModificarCantidadAsync(int productoId, int cantidad)
    {
        try
        {
            var response = await ExecuteWithCarritoRetryAsync(async carritoId =>
                await _httpClient.PostAsync($"/carritos/{carritoId}/{productoId}/modificar?cantidad={cantidad}", null));
            
            var success = response.IsSuccessStatusCode;
            if (success)
            {
                _logger.LogInformation("Cantidad modificada para producto {ProductoId}: {Cantidad}", productoId, cantidad);
            }
            else
            {
                _logger.LogWarning("Error al modificar cantidad para producto {ProductoId}. Status: {StatusCode}", productoId, response.StatusCode);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al modificar cantidad para producto {ProductoId}", productoId);
            return false;
        }
    }public async Task<bool> EliminarProductoAsync(int productoId)
    {
        try
        {
            var response = await ExecuteWithCarritoRetryAsync(async carritoId =>
                await _httpClient.DeleteAsync($"/carritos/{carritoId}/{productoId}"));
            

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Producto {ProductoId} no encontrado en carrito (puede estar ya eliminado)", productoId);
                return true;
            }
            
            var success = response.IsSuccessStatusCode;
            if (success)
            {
                _logger.LogInformation("Producto {ProductoId} eliminado del carrito", productoId);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto {ProductoId} del carrito", productoId);
            return false;
        }
    }    public async Task<bool> VaciarCarritoAsync()
    {
        try
        {
            var response = await ExecuteWithCarritoRetryAsync(async carritoId =>
                await _httpClient.DeleteAsync($"/carritos/{carritoId}"));
            

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Carrito ya vacío o no existente");
                return true;
            }
            
            var success = response.IsSuccessStatusCode;
            if (success)
            {
                _logger.LogInformation("Carrito vaciado exitosamente");
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al vaciar carrito");
            return false;
        }
    }

    public async Task<bool> ConfirmarCompraAsync(string nombre, string apellido, string email)
    {
        try
        {
            var datos = new Models.Compra 
            { 
                NombreCliente = nombre, 
                ApellidoCliente = apellido, 
                EmailCliente = email 
            };
            
            var response = await ExecuteWithCarritoRetryAsync(async carritoId =>
                await _httpClient.PutAsJsonAsync($"/carritos/{carritoId}/confirmar", datos));
            
            if (response.IsSuccessStatusCode)
            {
                _carritoId = 0; // Resetear para crear nuevo carrito
                _logger.LogInformation("Compra confirmada para cliente: {Nombre} {Apellido} ({Email})", 
                    nombre, apellido, email);
                return true;
            }
            
            _logger.LogWarning("Error al confirmar compra. Status: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al confirmar compra para cliente: {Nombre} {Apellido}", nombre, apellido);
            return false;
        }
    }    public async Task<int> ObtenerCantidadItemsAsync()
    {
        try
        {
            var carrito = await ObtenerCarritoAsync();
            var cantidad = carrito?.Items?.Sum(i => i.Cantidad) ?? 0;
            _logger.LogDebug("Cantidad de items en carrito: {Cantidad}", cantidad);
            return cantidad;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cantidad de items del carrito");
            return 0;
        }
    }

    public void ResetearCarrito()
    {
        _carritoId = 0; // Esto forzará la creación de un nuevo carrito
        _logger.LogInformation("Carrito reseteado");
    }
}
