using System.Net.Http.Json;

public class CarritoService
{
    private readonly HttpClient _http;
    public int? CarritoId { get; private set; }

    public int CantidadTotal { get; private set; } = 0;

    // Notificador de cambio para que el layout se actualice
    public event Action? OnCambio;
    private void NotificarCambio() => OnCambio?.Invoke();

    public CarritoService(HttpClient http)
    {
        _http = http;
    }

    public async Task InicializarCarritoAsync()
    {
        if (CarritoId == null)
        {
            var response = await _http.PostAsync("http://localhost:5184/carritos", null);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<RespuestaCarrito>();
                CarritoId = data?.Id;
            }
        }
    }

    public async Task<(bool Exito, string Mensaje)> AgregarProducto(int productoId, int cantidad = 1)
{
    if (CarritoId == null)
        await InicializarCarritoAsync();

    var dto = new { Cantidad = cantidad };
    var response = await _http.PutAsJsonAsync($"http://localhost:5184/carritos/{CarritoId}/{productoId}", dto);

    if (response.IsSuccessStatusCode)
    {
        CantidadTotal += cantidad;
        NotificarCambio();
        return (true, "Producto agregado al carrito.");
    }
    var mensajeError = await response.Content.ReadAsStringAsync();
    return (false, mensajeError);
}

    public async Task<bool> EliminarUnidad(int productoId)
    {
        if (CarritoId == null) return false;

        var resultado = await _http.DeleteAsync(
            $"http://localhost:5184/carritos/{CarritoId}/{productoId}");

        if (resultado.IsSuccessStatusCode)
        {
            CantidadTotal = Math.Max(0, CantidadTotal - 1);
            NotificarCambio();
        }

        return resultado.IsSuccessStatusCode;
    }

    public async Task<bool> VaciarCarrito()
    {
        if (CarritoId == null) return false;

        var respuesta = await _http.DeleteAsync(
            $"http://localhost:5184/carritos/{CarritoId}");

        if (respuesta.IsSuccessStatusCode)
        {
            CantidadTotal = 0;
            NotificarCambio(); // ✅ actualizar layout
        }

        return respuesta.IsSuccessStatusCode;
    }

    public void ResetearCarrito()
    {
        CarritoId = null;
        CantidadTotal = 0;
        OnCambio?.Invoke(); // Para actualizar ícono del carrito en caso de ser necesario
    }

    private class RespuestaCarrito
    {
        public int Id { get; set; }
    }
}
