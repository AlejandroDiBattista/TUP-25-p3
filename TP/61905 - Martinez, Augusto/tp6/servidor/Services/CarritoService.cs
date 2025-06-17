using servidor.Data;
using servidor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace servidor.Services
{
    public class CarritoService
    {
        private readonly AppDbContext _context;

        public CarritoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Carrito>> ObtenerCarritosAsync()
        {
            return await _context.Carritos.ToListAsync();
        }
    }
}
