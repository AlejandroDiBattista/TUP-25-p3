using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContactsDbContext>(options => {
    options.UseSqlite("Data Source=contacts.db");
});
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Crear la base de datos y poblarla si no existe
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/contacts", async (ContactsDbContext db) =>
    await db.Contacts.ToListAsync());

app.MapGet("/contacts/{id}", async (int id, ContactsDbContext db) =>
    await db.Contacts.FindAsync(id) is Contact contact ? Results.Ok(contact) : Results.NotFound());

app.MapPost("/contacts", async (Contact contact, ContactsDbContext db) => {
    db.Contacts.Add(contact);
    await db.SaveChangesAsync();
    return Results.Created($"/contacts/{contact.Id}", contact);
});

app.MapPut("/contacts/{id}", async (int id, Contact input, ContactsDbContext db) => {
    var contact = await db.Contacts.FindAsync(id);
    if (contact is null) {
        return Results.NotFound();
    }
    contact.Nombre = input.Nombre;
    contact.Apellido = input.Apellido;
    contact.Telefono = input.Telefono;
    contact.Email = input.Email;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/contacts/{id}", async (int id, ContactsDbContext db) => {
    var contact = await db.Contacts.FindAsync(id);
    if (contact is null) {
        return Results.NotFound();
    }
    db.Contacts.Remove(contact);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
