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

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

// <<< CÓDIGO AÑADIDO INICIA AQUÍ >>>
// =================================================================
// DEFINICIÓN DE ENDPOINTS DE LA API
// =================================================================

// Endpoint para obtener todos los productos (con búsqueda opcional)
app.MapGet("/api/productos", async (TiendaDbContext db, string? q) => {
    // La variable 'q' es el parámetro de búsqueda opcional (query)
    // db.Productos.AsQueryable() crea una consulta a la base de datos que aún no se ha ejecutado
    var productosQuery = db.Productos.AsQueryable();

    // Si el parámetro 'q' no está vacío, filtramos la búsqueda
    if (!string.IsNullOrWhiteSpace(q))
    {
        // Añadimos una condición 'Where' para buscar 'q' en el Nombre o Descripción del producto.
        // Usamos ToLower() para que la búsqueda no distinga entre mayúsculas y minúsculas.
        productosQuery = productosQuery.Where(p =>
            p.Nombre.ToLower().Contains(q.ToLower()) ||
            p.Descripcion.ToLower().Contains(q.ToLower())
        );
    }

    // Ejecutamos la consulta y la convertimos en una lista.
    var productos = await productosQuery.ToListAsync();

    // Devolvemos un 200 OK con la lista de productos en formato JSON.
    return Results.Ok(productos);
});
// <<< CÓDIGO AÑADIDO TERMINA AQUÍ >>>


app.Run();