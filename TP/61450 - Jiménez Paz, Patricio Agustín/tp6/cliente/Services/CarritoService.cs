using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using cliente.Models;
using cliente.Services;

namespace cliente.Services;

public class CarritoService
{
    private readonly HttpClient _httpClient;
    private readonly NotificationService _notificationService;
    private Carrito _carrito;

    public event Action OnChange;

    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public CarritoService(HttpClient httpClient, NotificationService notificationService)
    {
        _httpClient = httpClient;
        _notificationService = notificationService;
    }

    public int CantidadItems
    {
        get
        {
            if (_carrito == null)
                return 0;

            return _carrito.Items?.Count ?? 0;
        }
    }

    public async Task InicializarCarritoAsync()
    {
        if (_carrito != null)
            return;

        try
        {
            var response = await _httpClient.PostAsJsonAsync<Carrito>(
                "/carritos",
                null,
                _jsonOptions
            );
            response.EnsureSuccessStatusCode();
            _carrito = await response.Content.ReadFromJsonAsync<Carrito>(_jsonOptions);
            NotificarCambio();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Error al inicializar el carrito: {ex.Message}");
        }
    }

    public async Task ObtenerCarritoAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"/carritos/{_carrito.Id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorObj = System.Text.Json.JsonDocument.Parse(errorJson);
                    if (errorObj.RootElement.TryGetProperty("message", out var messageProp))
                    {
                        _notificationService.ShowError(messageProp.GetString());
                    }
                    else
                    {
                        _notificationService.ShowError("Carrito no encontrado.");
                    }
                }
                catch
                {
                    _notificationService.ShowError("Carrito no encontrado.");
                }
                _carrito = null;
                return;
            }
            response.EnsureSuccessStatusCode();
            _carrito = await response.Content.ReadFromJsonAsync<Carrito>(_jsonOptions);
            NotificarCambio();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Error al obtener el carrito: {ex.Message}");
        }
    }

    public async Task AgregarItemACarritoAsync(int productoId)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync<Carrito>(
                $"/carritos/{_carrito.Id}/{productoId}",
                null
            );
            if (response.IsSuccessStatusCode)
            {
                _carrito = await response.Content.ReadFromJsonAsync<Carrito>(_jsonOptions);
                ItemCarrito itemAgreado = _carrito.Items.FirstOrDefault(i =>
                    i.ProductoId == productoId
                );
                _notificationService.ShowSuccess(
                    $"{itemAgreado.Producto.Nombre} agregado al carrito."
                );
                NotificarCambio();
            }
            else if (
                response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.BadRequest
            )
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorObj = System.Text.Json.JsonDocument.Parse(errorJson);
                    if (errorObj.RootElement.TryGetProperty("message", out var messageProp))
                    {
                        _notificationService.ShowError(messageProp.GetString());
                    }
                    else
                    {
                        _notificationService.ShowError(
                            "No se pudo agregar el producto al carrito."
                        );
                    }
                }
                catch
                {
                    _notificationService.ShowError("No se pudo agregar el producto al carrito.");
                }
            }
            else
            {
                _notificationService.ShowError("No se pudo agregar el producto al carrito.");
            }
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("No se pudo agregar el producto al carrito.");
        }
    }

    public async Task EliminarItemDeCarritoAsync(int productoId)
    {
        try
        {
            ItemCarrito itemAEliminar = _carrito.Items.FirstOrDefault(i =>
                i.ProductoId == productoId
            );
            var response = await _httpClient.DeleteAsync($"/carritos/{_carrito.Id}/{productoId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorObj = System.Text.Json.JsonDocument.Parse(errorJson);
                    if (errorObj.RootElement.TryGetProperty("message", out var messageProp))
                    {
                        _notificationService.ShowError(messageProp.GetString());
                    }
                    else
                    {
                        _notificationService.ShowError("Item no encontrado en el carrito.");
                    }
                }
                catch
                {
                    _notificationService.ShowError("Item no encontrado en el carrito.");
                }
                return;
            }
            response.EnsureSuccessStatusCode();
            _carrito = await response.Content.ReadFromJsonAsync<Carrito>(_jsonOptions);
            _notificationService.ShowInfo(
                $"{itemAEliminar.Producto.Nombre} eliminado del carrito."
            );
            NotificarCambio();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Error al eliminar el item del carrito: {ex.Message}");
        }
    }

    public async Task ConfirmarCompraAsync(CompraDto compraDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"/carritos/{_carrito.Id}/confirmar",
                compraDto,
                _jsonOptions
            );
            if (
                response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.BadRequest
            )
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorObj = System.Text.Json.JsonDocument.Parse(errorJson);
                    if (errorObj.RootElement.TryGetProperty("message", out var messageProp))
                    {
                        _notificationService.ShowError(messageProp.GetString());
                    }
                    else
                    {
                        _notificationService.ShowError("Error al confirmar la compra.");
                    }
                }
                catch
                {
                    _notificationService.ShowError("Error al confirmar la compra.");
                }
                return;
            }
            response.EnsureSuccessStatusCode();
            _carrito = null; // Limpiar el carrito después de confirmar la compra
            _notificationService.ShowSuccess("Compra confirmada exitosamente.");
            NotificarCambio();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Error al confirmar la compra: {ex.Message}");
        }
    }

    public async Task VaciarCarritoAsync()
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/carritos/{_carrito.Id}");
            if (
                response.StatusCode == System.Net.HttpStatusCode.NotFound
                || response.StatusCode == System.Net.HttpStatusCode.BadRequest
            )
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorObj = System.Text.Json.JsonDocument.Parse(errorJson);
                    if (errorObj.RootElement.TryGetProperty("message", out var messageProp))
                    {
                        _notificationService.ShowError(messageProp.GetString());
                    }
                    else
                    {
                        _notificationService.ShowError("No se pudo vaciar el carrito.");
                    }
                }
                catch
                {
                    _notificationService.ShowError("No se pudo vaciar el carrito.");
                }
                return;
            }
            response.EnsureSuccessStatusCode();
            _notificationService.ShowInfo("Carrito vaciado exitosamente.");
            _carrito.Items = new List<ItemCarrito>(); // Limpiar los items del carrito
            NotificarCambio();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError($"Error al vaciar el carrito: {ex.Message}");
        }
    }

    private void NotificarCambio() => OnChange?.Invoke();
}
