using Microsoft.EntityFrameworkCore;
using servidor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
        policy.WithOrigins("http://localhost:5177", "https://localhost:7221")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TiendaDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowClientApp");

app.MapGet("/api/productos", async (TiendaDbContext db, string? q) => {
    
    var productosQuery = db.Productos.AsQueryable();
    if (!string.IsNullOrWhiteSpace(q))
    {
        productosQuery = productosQuery.Where(p =>
            p.Nombre.ToLower().Contains(q.ToLower()) ||
            p.Descripcion.ToLower().Contains(q.ToLower())
        );
    }
    var productos = await productosQuery.ToListAsync();
    return Results.Ok(productos);
});

app.MapPost("/api/carritos", async (TiendaDbContext db) => {
    var nuevoCarrito = new Carrito();
    db.Carritos.Add(nuevoCarrito);
    await db.SaveChangesAsync();
    return Results.Created($"/api/carritos/{nuevoCarrito.Id}", new { carritoId = nuevoCarrito.Id });
});


app.Run();