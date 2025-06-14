using Microsoft.EntityFrameworkCore;
using TiendaOnline.Models;
using TiendaOnline.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar DbContext con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Endpoints para Productos

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

// Endpoints similares para Usuarios

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
