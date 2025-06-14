using Microsoft.EntityFrameworkCore;
using servidor.Modelos;

var builder = WebApplication.CreateBuilder(args);

// Conexión con SQLite
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

// ENDPOINTS PRODUCTOS

app.MapGet("/productos", async (TiendaContext db) =>
    await db.Productos.ToListAsync()
);

app.MapGet("/productos/{id}", async (int id, TiendaContext db) =>
    await db.Productos.FindAsync(id) is Producto producto
        ? Results.Ok(producto)
        : Results.NotFound()
);

app.MapPost("/productos", async (Producto producto, TiendaContext db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{producto.Id}", producto);
});

app.MapPut("/productos/{id}", async (int id, Producto datosProducto, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = datosProducto.Nombre;
    producto.Precio = datosProducto.Precio;
    producto.Stock = datosProducto.Stock;
    await db.SaveChangesAsync();
    return Results.Ok(producto);
});

app.MapDelete("/productos/{id}", async (int id, TiendaContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ENDPOINTS COMPRAS (CARRITO)

app.MapPost("/compras", async (Compra compra, TiendaContext db) =>
{
    // Validaciones y lógica aquí...
    // Por ejemplo:
    compra.Fecha = DateTime.Now;
    compra.Total = compra.Items.Sum(item =>
    {
        var producto = db.Productos.Find(item.ProductoId);
        return producto != null ? producto.Precio * item.Cantidad : 0;
    });

    db.Compras.Add(compra);
    await db.SaveChangesAsync();

    return Results.Created($"/compras/{compra.Id}", new { compra.Id, compra.Total, compra.Fecha });
});

app.MapGet("/compras", async (TiendaContext db) =>
    await db.Compras.Include(c => c.Items).ToListAsync()
);

app.MapGet("/compras/{id}", async (int id, TiendaContext db) =>
    await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id) is Compra compra
        ? Results.Ok(compra)
        : Results.NotFound()
);

app.MapGet("/", () => "Servidor API está en funcionamiento");

app.Run();
