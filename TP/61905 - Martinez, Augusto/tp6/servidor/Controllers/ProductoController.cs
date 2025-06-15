using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using servidor.Models;
using servidor.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace servidor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductoService _productoService;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(ProductoService productoService, ILogger<ProductoController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Producto>>> ObtenerProductos()
        {
            try
            {
                var productos = await _productoService.ObtenerProductosAsync();

                if (productos == null || productos.Count == 0)
                {
                    _logger.LogWarning("No se encontraron productos en la base de datos.");
                    return NotFound("No hay productos disponibles.");
                }

                return Ok(productos);
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Error en ObtenerProductos: {ex.Message}");
                return StatusCode(500, $"Error interno al obtener productos: {ex.Message}");
            }
        }
    }
}
