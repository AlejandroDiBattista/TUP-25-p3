using Microsoft.AspNetCore.Mvc;
using servidor.Data;
using servidor.Modelos;

[Route("api/[controller]")]
[ApiController]
public class CarritoController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarritoController(AppDbContext context)
    {
        _context = context;
    }
}