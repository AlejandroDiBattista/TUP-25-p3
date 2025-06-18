using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace servidor.Services
{
    public class VentaService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VentaService> _logger;

        public VentaService(AppDbContext context, ILogger<VentaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 🔹 Obtener todas las ventas con sus productos asociados
        public async Task<List<Venta>> ObtenerVentasAsync()
        {
            try
            {
                var ventas = await _context.Ventas
                                           .Include(v => v.VentaItems)
                                           .ThenInclude(vi => vi.Producto) // Carga los productos de cada venta
                                           .AsNoTracking()
                                           .ToListAsync();

                if (ventas.Count == 0)
                {
                    _logger.LogWarning("No se encontraron ventas en la base de datos.");
                }

                return ventas;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al obtener ventas: {ex.Message}");
                return new List<Venta>();
            }
        }

        // 🔹 Registrar una venta con validaciones y actualización de stock
        public async Task<bool> RegistrarVentaAsync(Venta venta)
        {
            try
            {
                // Validar que la venta tenga productos
                if (venta.VentaItems == null || venta.VentaItems.Count == 0)
                {
                    _logger.LogWarning("Intento de registrar una venta sin productos.");
                    return false;
                }

                _context.Ventas.Add(venta);

                // Actualizar stock de cada producto
                foreach (var item in venta.VentaItems)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null || producto.Stock < item.Cantidad)
                    {
                        _logger.LogError($"Stock insuficiente para el producto ID {item.ProductoId}.");
                        return false;
                    }

                    producto.Stock -= item.Cantidad;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Venta registrada correctamente con ID {venta.Id}.");
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al registrar la venta: {ex.Message}");
                return false;
            }
        }
    }
}
