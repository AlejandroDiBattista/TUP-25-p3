using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services
    .AddDbContext<ContactosDb>(opt => opt.UseSqlite("Data Source=contactos.db"));

var app = builder.Build();

// Habilitar CORS
app.UseCors();

// Endpoint para obtener todos los contactos
app.MapGet("/contactos", async (ContactosDb db) => await db.Contactos.ToListAsync());

// Endpoint para obtener un contacto por id
app.MapGet("/contactos/{id}", async (ContactosDb db, int id) => {
    var contacto = await db.Contactos.FindAsync(id);
    return contacto is not null ? Results.Ok(contacto) : Results.NotFound();
});

// Endpoint para crear un nuevo contacto
app.MapPost("/contactos", async (ContactosDb db, Contacto contacto) => {
    db.Contactos.Add(contacto);
    await db.SaveChangesAsync();
    return Results.Created($"/contactos/{contacto.Id}", contacto);
});

// Endpoint para actualizar un contacto existente
app.MapPut("/contactos/{id}", async (ContactosDb db, int id, Contacto datos) => {
    var contacto = await db.Contactos.FindAsync(id);
    if (contacto is null) return Results.NotFound();
    contacto.Nombre = datos.Nombre;
    contacto.Apellido = datos.Apellido;
    contacto.Telefono = datos.Telefono;
    contacto.Email = datos.Email;
    await db.SaveChangesAsync();
    return Results.Ok(contacto);
});

// Endpoint para eliminar un contacto
app.MapDelete("/contactos/{id}", async (ContactosDb db, int id) => {
    var contacto = await db.Contactos.FindAsync(id);
    if (contacto is null) return Results.NotFound();
    db.Contactos.Remove(contacto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Inicialización de la base de datos y datos de ejemplo
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<ContactosDb>();
    db.Database.EnsureCreated();
    if (!db.Contactos.Any()) {
        db.Contactos.AddRange(
            new Contacto { Nombre = "Juan", Apellido = "Pérez", Telefono = "123456", Email = "juan@mail.com" },
            new Contacto { Nombre = "Ana", Apellido = "García", Telefono = "654321", Email = "ana@mail.com" }
        );
        db.SaveChanges();
    }
}

app.MapGet("/", () => "Hello World!");

app.Run();

public class Contacto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class ContactosDb : DbContext
{
    public ContactosDb(DbContextOptions<ContactosDb> options) : base(options) { }
    public DbSet<Contacto> Contactos { get; set; }
}

