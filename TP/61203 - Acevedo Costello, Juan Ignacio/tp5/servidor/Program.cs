using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;        // <- para Results.*
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using servidor.Data;
using servidor.Models;

namespace servidor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Configurar DbContext
            builder.Services.AddDbContext<InventarioContext>();

            // 2) Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // 3) Middleware de Swagger en desarrollo
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 4) Semillado de productos
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<InventarioContext>();
                db.Database.Migrate();

                if (!db.Productos.Any())
                {
                    db.Productos.AddRange(
                        new Producto { Nombre = "Camiseta Deportiva", Descripcion = "Transpirable de poliéster", Stock = 10, Precio = 1500m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Zapatillas Urbanas", Descripcion = "Suela de goma y plantilla acolchada", Stock = 10, Precio = 3200m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Reloj Digital", Descripcion = "Resistente al agua con cronómetro", Stock = 10, Precio = 2400m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Mochila Escolar", Descripcion = "Varios compartimentos de nylon", Stock = 10, Precio = 1800m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Auriculares Bluetooth", Descripcion = "Cancelación de ruido", Stock = 10, Precio = 2700m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Lámpara de Escritorio", Descripcion = "LED con brazo flexible", Stock = 10, Precio = 1300m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Gorra Ajustable", Descripcion = "Cotton con visera curva", Stock = 10, Precio = 900m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Botella Térmica", Descripcion = "Acero inoxidable 500 ml", Stock = 10, Precio = 1100m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Juego de Cubiertos", Descripcion = "Set de 24 piezas acero inoxidable", Stock = 10, Precio = 2200m, UrlImagen = "https://via.placeholder.com/150" },
                        new Producto { Nombre = "Cargador Portátil", Descripcion = "Power bank 10000 mAh", Stock = 10, Precio = 2000m, UrlImagen = "https://via.placeholder.com/150" }
                    );
                    db.SaveChanges();
                }
            }

            // 5) Endpoints REST
            app.MapGet("/productos", async (InventarioContext db) =>
                await db.Productos.ToListAsync());

            app.MapGet("/productos/bajo-stock", async (InventarioContext db) =>
                await db.Productos.Where(p => p.Stock < 3).ToListAsync());

            app.MapPost("/productos/{id}/agregar", async (int id, int cantidad, InventarioContext db) =>
            {
                var p = await db.Productos.FindAsync(id);
                if (p == null) return Results.NotFound();
                p.Stock += cantidad;
                await db.SaveChangesAsync();
                return Results.Ok(p);
            });

            app.MapPost("/productos/{id}/quitar", async (int id, int cantidad, InventarioContext db) =>
            {
                var p = await db.Productos.FindAsync(id);
                if (p == null) return Results.NotFound();
                if (p.Stock < cantidad) return Results.BadRequest("Stock insuficiente");
                p.Stock -= cantidad;
                await db.SaveChangesAsync();
                return Results.Ok(p);
            });

            app.Run();
        }
    }
}

