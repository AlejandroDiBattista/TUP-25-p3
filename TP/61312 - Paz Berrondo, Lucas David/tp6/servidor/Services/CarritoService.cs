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
        }        // Actualizar datos de productos desde BD para obtener precios y stock actuales
        foreach (var item in carrito.Items)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto != null)
            {
                item.Producto = producto;
                item.PrecioUnitario = producto.Precio; // Sincronizar precio actual
            }
        }

        return carrito;
    }

    // Convierte un carrito en memoria a DTO para enviar al cliente
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

    // Vacía completamente un carrito eliminando todos sus items
    public bool VaciarCarrito(string carritoId)
    {
        if (!_carritos.TryGetValue(carritoId, out var carrito))
        {
            return false;
        }

        carrito.Items.Clear();
        Console.WriteLine($"🗑️ Carrito {carritoId} vaciado exitosamente");
        return true;
    }    // Elimina un carrito completamente del sistema
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
    }    /// <summary>
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
    }    /// <summary>
    /// Agrega un producto al carrito o actualiza la cantidad si ya existe.
    /// Valida stock disponible antes de agregar.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <param name="productoId">ID del producto a agregar</param>
    /// <param name="cantidad">Cantidad a establecer (reemplaza la cantidad existente)</param>
    /// <returns>Resultado de la operación con detalles</returns>
    public async Task<(bool Exito, string Mensaje, CarritoDto? Carrito)> AgregarProductoAsync(string carritoId, int productoId, int cantidad = 1)
    {
        // Validar que el carrito existe
        if (!_carritos.TryGetValue(carritoId, out var carrito))
        {
            return (false, $"Carrito con ID {carritoId} no encontrado", null);
        }

        // Validar cantidad positiva
        if (cantidad <= 0)
        {
            return (false, "La cantidad debe ser mayor a 0", null);
        }

        // Buscar el producto en la base de datos
        var producto = await _context.Productos.FindAsync(productoId);
        if (producto == null)
        {
            return (false, $"Producto con ID {productoId} no encontrado", null);
        }

        // Verificar si el producto ya está en el carrito
        var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

        // Validar stock disponible (usar la cantidad que se quiere establecer, no sumar)
        if (cantidad > producto.Stock)
        {
            return (false, $"Stock insuficiente. Stock disponible: {producto.Stock}, cantidad solicitada: {cantidad}", null);
        }        // Agregar o actualizar item en el carrito
        if (itemExistente != null)
        {
            // Actualizar cantidad del item existente (reemplazar, no sumar)
            itemExistente.Cantidad = cantidad;
            itemExistente.PrecioUnitario = producto.Precio; // Actualizar precio por si cambió
            itemExistente.Producto = producto;
            
            Console.WriteLine($"🛒 Producto {producto.Nombre} actualizado en carrito {carritoId}. Nueva cantidad: {cantidad}");
        }
        else
        {
            // Crear nuevo item en el carrito
            var nuevoItem = new ItemCarrito
            {
                ProductoId = productoId,
                Producto = producto,
                Cantidad = cantidad,
                PrecioUnitario = producto.Precio
            };
            
            carrito.Items.Add(nuevoItem);
            Console.WriteLine($"🛒 Producto {producto.Nombre} agregado al carrito {carritoId}. Cantidad: {cantidad}");
        }

        var carritoDto = ConvertirADto(carrito);
        return (true, "Producto agregado exitosamente", carritoDto);
    }

    /// <summary>
    /// Elimina un producto del carrito o reduce su cantidad.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <param name="productoId">ID del producto a eliminar</param>
    /// <param name="cantidad">Cantidad a eliminar (opcional, por defecto 1)</param>
    /// <returns>Resultado de la operación con detalles</returns>
    public async Task<(bool Exito, string Mensaje, CarritoDto? Carrito)> EliminarProductoAsync(string carritoId, int productoId, int cantidad = 1)
    {
        // Validar que el carrito existe
        if (!_carritos.TryGetValue(carritoId, out var carrito))
        {
            return (false, $"Carrito con ID {carritoId} no encontrado", null);
        }

        // Validar cantidad positiva
        if (cantidad <= 0)
        {
            return (false, "La cantidad debe ser mayor a 0", null);
        }

        // Buscar el item en el carrito
        var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null)
        {
            return (false, $"Producto con ID {productoId} no está en el carrito", null);
        }

        // Obtener datos actualizados del producto
        var producto = await _context.Productos.FindAsync(productoId);
        var nombreProducto = producto?.Nombre ?? $"Producto {productoId}";

        // Determinar si eliminar completamente o solo reducir cantidad
        if (cantidad >= item.Cantidad)
        {
            // Eliminar completamente el item del carrito
            carrito.Items.Remove(item);
            Console.WriteLine($"🗑️ Producto {nombreProducto} eliminado completamente del carrito {carritoId}");
        }
        else
        {
            // Reducir cantidad del item
            item.Cantidad -= cantidad;
            Console.WriteLine($"🗑️ Cantidad reducida del producto {nombreProducto} en carrito {carritoId}. Nueva cantidad: {item.Cantidad}");
        }

        var carritoDto = ConvertirADto(carrito);
        return (true, "Producto actualizado exitosamente", carritoDto);
    }

    /// <summary>
    /// Elimina completamente un producto del carrito sin importar la cantidad.
    /// </summary>
    /// <param name="carritoId">ID del carrito</param>
    /// <param name="productoId">ID del producto a eliminar completamente</param>
    /// <returns>Resultado de la operación</returns>
    public async Task<(bool Exito, string Mensaje, CarritoDto? Carrito)> EliminarProductoCompletoAsync(string carritoId, int productoId)
    {
        if (!_carritos.TryGetValue(carritoId, out var carrito))
        {
            return (false, $"Carrito con ID {carritoId} no encontrado", null);
        }

        var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null)
        {
            return (false, $"Producto con ID {productoId} no está en el carrito", null);
        }

        var producto = await _context.Productos.FindAsync(productoId);
        var nombreProducto = producto?.Nombre ?? $"Producto {productoId}";

        carrito.Items.Remove(item);
        Console.WriteLine($"🗑️ Producto {nombreProducto} eliminado completamente del carrito {carritoId}");        var carritoDto = ConvertirADto(carrito);
        return (true, "Producto eliminado completamente", carritoDto);
    }

    /// <summary>
    /// Confirma una compra convirtiendo el carrito en una compra persistente.
    /// Actualiza stock, crea registros en BD y limpia el carrito.
    /// </summary>
    /// <param name="carritoId">ID del carrito a confirmar</param>
    /// <param name="datosCliente">Datos del cliente para la compra</param>
    /// <returns>Resultado de la confirmación con detalles de la compra</returns>
    public async Task<(bool Exito, string Mensaje, CompraConfirmadaDto? Compra)> ConfirmarCompraAsync(string carritoId, ConfirmarCompraDto datosCliente)
    {
        // Validar que el carrito existe
        if (!_carritos.TryGetValue(carritoId, out var carrito))
        {
            return (false, $"Carrito con ID {carritoId} no encontrado", null);
        }

        // Validar que el carrito no esté vacío
        if (!carrito.Items.Any())
        {
            return (false, "No se puede confirmar una compra con el carrito vacío", null);
        }

        // Validar datos del cliente
        if (string.IsNullOrWhiteSpace(datosCliente.NombreCliente) ||
            string.IsNullOrWhiteSpace(datosCliente.ApellidoCliente) ||
            string.IsNullOrWhiteSpace(datosCliente.EmailCliente))
        {
            return (false, "Todos los datos del cliente son obligatorios (Nombre, Apellido, Email)", null);
        }

        // Validar formato de email básico
        if (!datosCliente.EmailCliente.Contains("@") || !datosCliente.EmailCliente.Contains("."))
        {
            return (false, "El formato del email no es válido", null);
        }

        // Verificar stock disponible para todos los productos
        var validacionStock = await ValidarStockDisponibleAsync(carrito);
        if (!validacionStock.Exito)
        {
            return (false, validacionStock.Mensaje, null);
        }

        // Usar transacción para asegurar consistencia
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Crear la compra
            var compra = new Compra
            {
                Fecha = DateTime.Now,
                NombreCliente = datosCliente.NombreCliente.Trim(),
                ApellidoCliente = datosCliente.ApellidoCliente.Trim(),
                EmailCliente = datosCliente.EmailCliente.Trim().ToLower(),
                Items = new List<ItemCompra>()
            };

            // Agregar la compra al contexto para obtener el ID
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            decimal totalCompra = 0;

            // Crear items de compra y actualizar stock
            foreach (var item in carrito.Items)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto == null)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Producto con ID {item.ProductoId} no encontrado", null);
                }

                // Verificar stock una vez más (por si cambió durante la transacción)
                if (producto.Stock < item.Cantidad)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}, solicitado: {item.Cantidad}", null);
                }

                // Crear item de compra
                var itemCompra = new ItemCompra
                {
                    CompraId = compra.Id,
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio // Usar precio actual del producto
                };

                compra.Items.Add(itemCompra);
                totalCompra += itemCompra.Subtotal;

                // Actualizar stock del producto
                producto.Stock -= item.Cantidad;
                
                Console.WriteLine($"📦 Stock actualizado para {producto.Nombre}: {producto.Stock + item.Cantidad} → {producto.Stock}");
            }

            // Actualizar el total de la compra
            compra.Total = totalCompra;

            // Guardar todos los cambios
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Limpiar el carrito de memoria
            _carritos.Remove(carritoId);

            Console.WriteLine($"🎉 Compra #{compra.Id} confirmada exitosamente para {datosCliente.NombreCliente} {datosCliente.ApellidoCliente}. Total: ${totalCompra:F2}");

            var compraConfirmada = new CompraConfirmadaDto
            {
                CompraId = compra.Id,
                Total = totalCompra,
                Fecha = compra.Fecha,
                Mensaje = $"Compra confirmada exitosamente. ¡Gracias {datosCliente.NombreCliente}!"
            };

            return (true, "Compra confirmada exitosamente", compraConfirmada);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"❌ Error al confirmar compra: {ex.Message}");
            return (false, "Error interno al procesar la compra. Inténtalo nuevamente.", null);
        }
    }

    /// <summary>
    /// Valida que haya stock suficiente para todos los productos del carrito.
    /// </summary>
    /// <param name="carrito">Carrito a validar</param>
    /// <returns>Resultado de la validación</returns>
    private async Task<(bool Exito, string Mensaje)> ValidarStockDisponibleAsync(Carrito carrito)
    {
        foreach (var item in carrito.Items)
        {
            var producto = await _context.Productos.FindAsync(item.ProductoId);
            if (producto == null)
            {
                return (false, $"Producto con ID {item.ProductoId} no encontrado");
            }

            if (producto.Stock < item.Cantidad)
            {
                return (false, $"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}, en carrito: {item.Cantidad}");
            }
        }

        return (true, "Stock disponible para todos los productos");
    }
}
