using Microsoft.AspNetCore.Mvc;
using servidor.Models;
using servidor.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace servidor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly VentaService _ventaService;

        public VentaController(VentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Venta>>> ObtenerVentas()
        {
            try
            {
                var ventas = await _ventaService.ObtenerVentasAsync();
                return Ok(ventas);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error en ObtenerVentas: {ex.Message}");
                return StatusCode(500, "Error al obtener ventas.");
            }
        }
    }
}
