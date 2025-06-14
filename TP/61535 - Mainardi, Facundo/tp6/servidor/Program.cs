using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servidor.Modelos;
using Servidor.Stock;

var builder = WebApplication.CreateBuilder(args);

var carrito = new Dictionary<Guid, List<ItemCompra>>();

builder.Services.AddDbContext<Tienda>(options =>
    options.UseSqlite("Data Source=tienda.db"));

builder.Services.AddScoped<Tienda>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Tienda>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowClientApp");

app.MapControllers();

app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.MapGet("/productos", async (Tienda db, string? q) =>
{
    var query = db.Productos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
    {
        string filtro = q.ToLower();
        query = query.Where(p => p.Nombre.ToLower().Contains(filtro) || p.Descripcion.ToLower().Contains(filtro));
    }

    var productos = await query.ToListAsync();
    return Results.Ok(productos);
});
app.MapPost("/carrito", () =>
{
    var nuevoId = Guid.NewGuid(); 
    carrito[nuevoId] = new List<ItemCompra>();
    return Results.Ok(nuevoId);
});
app.MapGet("/", () => "Servidor API está en funcionamiento");
app.Run();