using Cliente.Models;
using Cliente.Services;

namespace Cliente.Services;

public class CarritoEstado
{
    private readonly ApiService _apiService;
    public event Action OnChange;

    public string CarritoId { get; private set; }
    public CarritoDto Carrito { get; private set; }

    public CarritoEstado(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task InicializarCarritoAsync()
    {
        if (string.IsNullOrEmpty(CarritoId))
        {
            CarritoId = await _apiService.CrearCarritoAsync();
            await ActualizarCarritoAsync();
        }
    }

    public async Task ActualizarCarritoAsync()
    {
        if (!string.IsNullOrEmpty(CarritoId))
        {
            Carrito = await _apiService.ObtenerCarritoAsync(CarritoId);
            NotificarCambio();
        }
    }

    public async Task<bool> AgregarProductoAsync(int productoId, int cantidad = 1)
    {
        await InicializarCarritoAsync();
        
        if (!string.IsNullOrEmpty(CarritoId))
        {
            var exito = await _apiService.AgregarProductoCarritoAsync(CarritoId, productoId, cantidad);
            if (exito)
            {
                await ActualizarCarritoAsync();
            }
            return exito;        }
        return false;
    }

    public async Task<bool> EliminarProductoAsync(int productoId, int cantidad = 1)
    {
        if (!string.IsNullOrEmpty(CarritoId))
        {
            var exito = await _apiService.EliminarProductoCarritoAsync(CarritoId, productoId, cantidad);
            if (exito)
            {
                await ActualizarCarritoAsync();
            }
            return exito;
        }
        return false;
    }

    public async Task<bool> VaciarCarritoAsync()
    {
        if (!string.IsNullOrEmpty(CarritoId))
        {
            var exito = await _apiService.VaciarCarritoAsync(CarritoId);
            if (exito)
            {
                await ActualizarCarritoAsync();
            }
            return exito;
        }
        return false;
    }

    public async Task<bool> ConfirmarCompraAsync(ConfirmarCompraDto datosCliente)
    {
        if (!string.IsNullOrEmpty(CarritoId))
        {
            var exito = await _apiService.ConfirmarCompraAsync(CarritoId, datosCliente);
            if (exito)
            {
                // Limpiar el carrito local después de la confirmación
                Carrito = new CarritoDto { Id = CarritoId };
                CarritoId = null; // Forzar la creación de un nuevo carrito en la próxima operación
                NotificarCambio();
            }
            return exito;
        }
        return false;
    }

    private void NotificarCambio()
    {
        OnChange?.Invoke();
    }
}
