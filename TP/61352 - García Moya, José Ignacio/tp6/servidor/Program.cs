using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Modelo;


namespace servidor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar servicios
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowClientApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            builder.Services.AddDbContext<TiendaDbContext>(options =>
                options.UseSqlite("Data Source=tienda.db"));
            builder.Services.AddControllers();

            var app = builder.Build();

            // Middleware
            if (app.Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors("AllowClientApp");

            app.UseStaticFiles(); // ✅ Ahora sí, después de Build

            // Endpoints
            app.MapGet("/", () => "Servidor API está en funcionamiento");

            app.MapGet("/productos", async (string? q, TiendaDbContext db) =>
            {
                var productos = db.Productos.AsQueryable();
                if (!string.IsNullOrWhiteSpace(q))
                    productos = productos.Where(p => p.Nombre.Contains(q) || p.Descripcion.Contains(q));
                return await productos.ToListAsync();
            });

            // Inicializar DB
            DbInitializer.Inicializar(app);
            // Crear nuevo carrito (genera ID vacío)
app.MapPost("/carritos", async (TiendaDbContext db) =>
{
    // No hace falta hacer nada, solo generar un ID ficticio
    var nuevoId = Guid.NewGuid().ToString().Substring(0, 8); // ej: "a1b2c3d4"
    return Results.Ok(nuevoId);
});

// Obtener contenido del carrito
app.MapGet("/carritos/{carritoId}", async (string carritoId, TiendaDbContext db) =>
{
    var items = await db.CarritosTemporales
        .Where(c => c.Id.ToString() == carritoId)
        .Join(db.Productos,
              c => c.ProductoId,
              p => p.Id,
              (c, p) => new {
                  p.Id,
                  p.Nombre,
                  p.Descripcion,
                  p.ImagenUrl,
                  p.Precio,
                  c.Cantidad
              })
        .ToListAsync();

    return Results.Ok(items);
});
// Agregar producto o aumentar cantidad en carrito
app.MapPut("/carritos/{carritoId}/{productoId}", async (
    string carritoId,
    int productoId,
    TiendaDbContext db
) =>
{
    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null) return Results.NotFound("Producto no encontrado");

    var existente = await db.CarritosTemporales
        .FirstOrDefaultAsync(c => c.Id.ToString() == carritoId && c.ProductoId == productoId);

    if (existente is null)
    {
        if (producto.Stock <= 0)
            return Results.BadRequest("No hay stock disponible");

        db.CarritosTemporales.Add(new CarritoTemporal
        {
            Id = int.Parse(carritoId), // si usás string, deberíamos cambiarlo en el modelo a string
            ProductoId = productoId,
            Cantidad = 1
        });

        producto.Stock -= 1;
    }
    else
    {
        if (producto.Stock <= 0)
            return Results.BadRequest("Stock insuficiente");

        existente.Cantidad += 1;
        producto.Stock -= 1;
    }

    await db.SaveChangesAsync();
    return Results.Ok("Producto agregado al carrito");
});
// Quitar o reducir cantidad de un producto del carrito
app.MapDelete("/carritos/{carritoId}/{productoId}", async (
    string carritoId,
    int productoId,
    TiendaDbContext db
) =>
{
    var item = await db.CarritosTemporales
        .FirstOrDefaultAsync(c => c.Id.ToString() == carritoId && c.ProductoId == productoId);

    if (item is null)
        return Results.NotFound("Producto no está en el carrito");

    var producto = await db.Productos.FindAsync(productoId);
    if (producto is null)
        return Results.NotFound("Producto no encontrado");

    item.Cantidad -= 1;
    producto.Stock += 1;

    if (item.Cantidad <= 0)
        db.CarritosTemporales.Remove(item);

    await db.SaveChangesAsync();
    return Results.Ok("Producto actualizado en el carrito");
});
// Vaciar carrito
app.MapDelete("/carritos/{carritoId}", async (string carritoId, TiendaDbContext db) =>
{
    var items = await db.CarritosTemporales
        .Where(c => c.Id.ToString() == carritoId)
        .ToListAsync();

    if (!items.Any())
        return Results.NotFound("El carrito ya está vacío");

    foreach (var item in items)
    {
        var producto = await db.Productos.FindAsync(item.ProductoId);
        if (producto != null)
            producto.Stock += item.Cantidad;

        db.CarritosTemporales.Remove(item);
    }

    await db.SaveChangesAsync();
    return Results.Ok("Carrito vaciado");
});

            app.Run();


        }
    }


}
