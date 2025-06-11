using servidor.Models;
using servidor.DTOs;
using Microsoft.EntityFrameworkCore;
using servidor.Data;

namespace servidor.Services;

/// <summary>
/// Servicio para manejar carritos de compra temporales en memoria.
/// Los carritos se almacenan temporalmente hasta que se confirme la compra.
/// </summary>
public class CarritoService
{
    private static readonly Dictionary<string, Carrito> _carritos = new();
    private readonly TiendaContext _context;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos para validar productos y stock.
    /// </summary>
    /// <param name="context">Contexto de Entity Framework</param>
    public CarritoService(TiendaContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Crea un nuevo carrito vacío y retorna su ID único.
    /// </summary>
    /// <returns>ID único del carrito creado</returns>
    public string CrearCarrito()
    {
        var carrito = new Carrito();
        _carritos[carrito.Id] = carrito;
        
        Console.WriteLine($"🛒 Carrito creado con ID: {carrito.Id}");
        return carrito.Id;
    }

    /// <summary>
    /// Obtiene un carrito por su ID.
    /// </summary>
    /// <param name="carritoId">ID del carrito a buscar</param>
    /// <returns>Carrito encontrado o null si no existe</returns>
    public async Task<Carrito?> ObtenerCarritoAsync(string carritoId)
    {
        if (!_carritos.TryGetValue(carritoId, out var carrito))
        {
            return null;
        }

        // Cargar datos actualizados de productos para cada item del carrito
        foreach (var item in carrito.Items)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto != null)
            {
                item.Producto = producto;
                // Actualizar precio en caso de que haya cambiado
                item.PrecioUnitario = producto.Precio;
            }
        }

        return carrito;
    }

    /// <summary>
    /// Convierte un carrito en memoria a un DTO para enviar al cliente.
    /// </summary>
    /// <param name="carrito">Carrito a convertir</param>
    /// <returns>DTO del carrito con datos completos</returns>
    public CarritoDto ConvertirADto(Carrito carrito)
    {
        return new CarritoDto
        {
            Id = carrito.Id,
            Items = carrito.Items.Select(item => new ItemCarritoDto
            {
                ProductoId = item.ProductoId,
                NombreProducto = item.Producto?.Nombre ?? "Producto no encontrado",
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Subtotal = item.Subtotal,
                ImagenUrl = item.Producto?.ImagenUrl ?? ""
            }).ToList(),
            Total = carrito.Total,
            TotalItems = carrito.TotalItems
        };
    }

    /// <summary>
    /// Vacía completamente un carrito eliminando todos sus items.
    /// </summary>
    /// <param name="carritoId">ID del carrito a vaciar</param>
    /// <returns>True si se vació exitosamente, false si el carrito no existe</returns>
    public bool VaciarCarrito(string carritoId)
    {
        if (!_carritos.TryGetValue(carritoId, out var carrito))
        {
            return false;
        }

        carrito.Items.Clear();
        Console.WriteLine($"🗑️ Carrito {carritoId} vaciado exitosamente");
        return true;
    }

    /// <summary>
    /// Elimina un carrito completamente del sistema.
    /// </summary>
    /// <param name="carritoId">ID del carrito a eliminar</param>
    /// <returns>True si se eliminó exitosamente, false si no existía</returns>
    public bool EliminarCarrito(string carritoId)
    {
        var eliminado = _carritos.Remove(carritoId);
        if (eliminado)
        {
            Console.WriteLine($"🗑️ Carrito {carritoId} eliminado del sistema");
        }
        return eliminado;
    }

    /// <summary>
    /// Obtiene estadísticas generales de carritos activos.
    /// Útil para debugging y monitoreo.
    /// </summary>
    /// <returns>Objeto con estadísticas de carritos</returns>
    public object ObtenerEstadisticas()
    {
        return new
        {
            CarritosActivos = _carritos.Count,
            ItemsTotales = _carritos.Values.Sum(c => c.TotalItems),
            ValorTotal = _carritos.Values.Sum(c => c.Total)
        };
    }

    /// <summary>
    /// Limpia carritos antiguos (más de 24 horas) para liberar memoria.
    /// Se puede ejecutar periódicamente en un background service.
    /// </summary>
    /// <returns>Número de carritos eliminados</returns>
    public int LimpiarCarritosAntiguos()
    {
        var ahora = DateTime.Now;
        var carritosAEliminar = _carritos
            .Where(kvp => (ahora - kvp.Value.FechaCreacion).TotalHours > 24)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var carritoId in carritosAEliminar)
        {
            _carritos.Remove(carritoId);
        }

        if (carritosAEliminar.Count > 0)
        {
            Console.WriteLine($"🧹 Limpieza automática: {carritosAEliminar.Count} carritos antiguos eliminados");
        }

        return carritosAEliminar.Count;
    }
}
