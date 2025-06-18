using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace servidor.Services
{
    public class CarritoService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CarritoService> _logger;

        public CarritoService(AppDbContext context, ILogger<CarritoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Obtener todos los carritos
        public async Task<List<Carrito>> ObtenerCarritosAsync()
        {
            try
            {
                var carritos = await _context.Carritos
                                             .Include(c => c.CarritoItems)
                                             .ThenInclude(ci => ci.Producto) // 🔥 Carga los productos asociados a los ítems
                                             .AsNoTracking()
                                             .ToListAsync();

                if (!carritos.Any())
                {
                    _logger.LogWarning("No se encontraron carritos en la base de datos.");
                }

                return carritos;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al obtener carritos: {ex.Message}");
                return new List<Carrito>();
            }
        }

        // ✅ Obtener carrito por CarritoId (NO por UsuarioId)
        public async Task<Carrito> ObtenerCarritoPorIdAsync(Guid carritoId)
        {
            try
            {
                var carrito = await _context.Carritos
                                            .Include(c => c.CarritoItems)
                                            .ThenInclude(ci => ci.Producto)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(c => c.Id == carritoId);

                if (carrito == null)
                {
                    _logger.LogWarning($"No se encontró carrito con ID {carritoId}.");
                }

                return carrito;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al obtener carrito con ID {carritoId}: {ex.Message}");
                return null;
            }
        }

        // ✅ Agregar un producto al carrito
        public async Task<bool> AgregarAlCarritoAsync(Guid carritoId, CarritoItem item)
        {
            try
            {
                var carrito = await _context.Carritos.Include(c => c.CarritoItems)
                                                     .FirstOrDefaultAsync(c => c.Id == carritoId);
                
                if (carrito == null)
                {
                    _logger.LogWarning("Carrito no encontrado.");
                    return false;
                }

                carrito.CarritoItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al agregar producto al carrito ID {carritoId}: {ex.Message}");
                return false;
            }
        }

        // ✅ Vaciar el carrito
        public async Task<bool> VaciarCarritoAsync(Guid carritoId)
        {
            try
            {
                var carrito = await _context.Carritos.Include(c => c.CarritoItems)
                                                     .FirstOrDefaultAsync(c => c.Id == carritoId);

                if (carrito == null) return false;

                _context.CarritoItems.RemoveRange(carrito.CarritoItems);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error al vaciar carrito ID {carritoId}: {ex.Message}");
                return false;
            }
        }
    }
}
