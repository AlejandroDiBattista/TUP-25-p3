using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Servidor.Models;
using MYContext;

namespace Services;

public interface IPruductServices
{
    Task<List<Producto>> GetPorducts(string? query);

}

public class ProductService : IPruductServices
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Producto>> GetPorducts(string? busqueda)
    {
        Console.WriteLine(busqueda);
        var query = _context.Productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            query = query.Where(p =>
                p.Nombre.Contains(busqueda)
               );
        }

        return await query.ToListAsync();
    }

}