using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using servidor.Models;

namespace servidor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Producto>> Get()
        {
            return Ok(TiendaData.Productos);
        }
    }
}