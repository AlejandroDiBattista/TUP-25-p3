using Microsoft.EntityFrameworkCore;
using TiendaOnline.Models;
using TiendaOnline.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar DbContext con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Endpoints para Compras
app.MapGet("/compras", async (AppDbContext db) =>
{
    return await db.Compras.Include(c => c.Items).ToListAsync();
});

app.MapGet("/compras/{id}", async (int id, AppDbContext db) =>
{
    var compra = await db.Compras.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
    return compra is not null ? Results.Ok(compra) : Results.NotFound();
});

app.MapPost("/compras", async (Compra compra, AppDbContext db) =>
{
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Created($"/compras/{compra.Id}", compra);
});

// Endpoints para ItemsCompra
app.MapGet("/itemscompra", async (AppDbContext db) =>
{
    return await db.ItemsCompra.ToListAsync();
});

app.MapGet("/itemscompra/{id}", async (int id, AppDbContext db) =>
{
    var item = await db.ItemsCompra.FindAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

app.MapPost("/itemscompra", async (ItemCompra item, AppDbContext db) =>
{
    db.ItemsCompra.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/itemscompra/{item.Id}", item);
});


// Obtener todas las compras
app.MapGet("/compras", async (AppDbContext db) =>
{
    return await db.Compras.Include(c => c.ItemsCompra).ToListAsync();
});

// Obtener compra por Id
app.MapGet("/compras/{id}", async (int id, AppDbContext db) =>
{
    var compra = await db.Compras.Include(c => c.ItemsCompra).FirstOrDefaultAsync(c => c.Id == id);
    return compra is not null ? Results.Ok(compra) : Results.NotFound();
});

// Crear compra con items
app.MapPost("/compras", async (Compra compra, AppDbContext db) =>
{
    db.Compras.Add(compra);
    await db.SaveChangesAsync();
    return Results.Created($"/compras/{compra.Id}", compra);
});

app.MapPut("/compras/{id}", async (int id, Compra inputCompra, AppDbContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    if (compra is null) return Results.NotFound();

    compra.Fecha = inputCompra.Fecha;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/compras/{id}", async (int id, AppDbContext db) =>
{
    var compra = await db.Compras.FindAsync(id);
    if (compra is null) return Results.NotFound();

    db.Compras.Remove(compra);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.MapGet("/productos", async (AppDbContext db) =>
{
    return await db.Productos.ToListAsync();
});

app.MapGet("/productos/{id}", async (int id, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    return producto is not null ? Results.Ok(producto) : Results.NotFound();
});

app.MapPost("/productos", async (Producto producto, AppDbContext db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{producto.Id}", producto);
});

app.MapPut("/productos/{id}", async (int id, Producto inputProducto, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = inputProducto.Nombre;
    producto.Precio = inputProducto.Precio;
    // Actualiza otras propiedades necesarias

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/productos/{id}", async (int id, AppDbContext db) =>
{
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    db.Productos.Remove(producto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Endpoints para Usuarios

app.MapGet("/usuarios", async (AppDbContext db) =>
{
    return await db.Usuarios.ToListAsync();
});

app.MapGet("/usuarios/{id}", async (int id, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
});

app.MapPost("/usuarios", async (Usuario usuario, AppDbContext db) =>
{
    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
});

app.MapPut("/usuarios/{id}", async (int id, Usuario inputUsuario, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario is null) return Results.NotFound();

    usuario.Nombre = inputUsuario.Nombre;
    usuario.Email = inputUsuario.Email;
    // Actualiza otras propiedades necesarias

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/usuarios/{id}", async (int id, AppDbContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario is null) return Results.NotFound();

    db.Usuarios.Remove(usuario);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
