using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Servidor.Models;
using MYContext;
using Servidor.Dto;

namespace Services;

public interface IPruductServices
{
    Task<List<Producto>> GetPorducts(string? query);
    Task<List<CarritoGtDto>> GetPorductsCarrito();
    Task CarritoInit(CarritoDto dto);

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

        var query = _context.Productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            query = query.Where(p =>
                p.Nombre.Contains(busqueda)
               );
        }

        return await query.ToListAsync();
    }
    public async Task<List<CarritoGtDto>> GetPorductsCarrito()
    {
        var res = await _context.Carrito
                .Join(
                    _context.Productos,
                    carrito => carrito.ProductoId,
                    producto => producto.Id_producto,
                    (carrito, producto) => new CarritoGtDto
                    {
                        Id_Carrito = carrito.Id_Carrito,
                        Cantidad = carrito.Cantidad,
                        ProductoId = producto.Id_producto,
                        NombreProducto = producto.Nombre,
                        PrecioProducto = producto.Precio
                    }
                )
                .ToListAsync();

        return res;
    }

    public async Task CarritoInit(CarritoDto dto)
    {
        var buscar = await _context.Productos.FindAsync(dto.ProductoId);

        if (buscar != null && buscar.Stock >= dto.Cantidad)
        {
            buscar.Stock -= dto.Cantidad;

            var carrito = new Carrito { Cantidad = dto.Cantidad, ProductoId = dto.ProductoId };
            _context.Carrito.Add(carrito);
            await _context.SaveChangesAsync();
        }

    }

}