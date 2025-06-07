using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlite("Data Source=tienda.db"));


builder.Services.AddControllers();

var carritos = new List<Carrito>();
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TiendaContext>();
    db.Database.EnsureCreated();
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//permite que el cliente pueda acceder a la API
app.UseCors("AllowClientApp");

//Es para que la api reciva peteciones
app.MapGet("/", () => "Servidor API está en funcionamiento");

//Endpoint de prueba que devuelve un mensaje y la fecha actual
app.MapGet("/api/datos", () => new { Mensaje = "Datos desde el servidor", Fecha = DateTime.Now });

//Endpoint devuelve los productos desde la base de datos
app.MapGet("/api/productos", async (TiendaContext db, string? buscar) =>

// Busca productos por nombre, descripción, marca o precio
{
    if (string.IsNullOrEmpty(buscar))
        return Results.Ok(await db.Productos.ToListAsync());

    var filtrados = await db.Productos
        .Where(p =>
            p.Nombre.Contains(buscar) ||
            p.Descripcion.Contains(buscar) ||
            p.Marca.Contains(buscar) ||
            p.Precio.ToString().Contains(buscar)
        )
        .ToListAsync();

    return Results.Ok(filtrados);
});

//endpoint para inicializar el carrito
app.MapPost("/carritos", () =>
{
    var carrito = new Carrito();
    carritos.Add(carrito);
    return Results.Ok(new { carrito.Id });
});
//llama al Carrito por el ID y devuelve los detalles
app.MapGet("/carritos/{id}", (Guid id) =>
{
    var carrito = carritos.FirstOrDefault(c => c.Id == id);

    if (carrito == null)
        return Results.NotFound(new { Mensaje = "Carrito no encontrado" });

    return Results.Ok(carrito.Items);
});

app.Run();