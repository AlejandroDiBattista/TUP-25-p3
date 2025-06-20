using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        builder => builder
            .WithOrigins("https://localhost:71xx", "http://localhost:5xxx")
                                                                         
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection(); 


app.UseCors("AllowBlazorClient");


app.MapGet("/productos", async (ApplicationDbContext db) =>
{
    return await db.Productos.ToListAsync();
})
.WithName("GetProductos")
.WithOpenApi();


app.MapPost("/productos", async (Producto producto, ApplicationDbContext db) =>
{
    db.Productos.Add(producto);
    await db.SaveChangesAsync();
    return Results.Created($"/productos/{producto.Id}", producto);
});



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
   
    db.Database.Migrate(); 

   
    if (!db.Productos.Any())
    {
        db.Productos.AddRange(
            new Producto { Nombre = "Café Expresso", Descripcion = "Café de origen colombiano, tostado oscuro.", Precio = 2.50m, ImagenUrl = "cafe_expresso.jpg", Stock = 100 },
            new Producto { Nombre = "Té Verde Matcha", Descripcion = "Té verde japonés, ideal para energizar.", Precio = 3.00m, ImagenUrl = "te_verde_matcha.jpg", Stock = 75 },
            new Producto { Nombre = "Medialuna de Manteca", Descripcion = "Clásica medialuna de manteca, hojaldrada.", Precio = 1.80m, ImagenUrl = "medialuna_manteca.jpg", Stock = 50 },
            new Producto { Nombre = "Jugo de Naranja Natural", Descripcion = "Recién exprimido, sin azúcares añadidos.", Precio = 3.50m, ImagenUrl = "jugo_naranja.jpg", Stock = 60 }
        );
        await db.SaveChangesAsync();
    }
}


app.Run();