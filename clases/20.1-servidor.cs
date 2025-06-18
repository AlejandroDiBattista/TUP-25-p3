#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using System.Text.Json;                     
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

//  CONFIGURACIÓN
var builder = WebApplication.CreateBuilder();

builder.Services
    .AddDbContext<AppDb>(opt => opt.UseSqlite("Data Source=agenda.db")) // agregar servicios : Instalar EF Core y SQLite
    .Configure<JsonOptions>(opt => opt.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

var app = builder.Build();

// crear BD si no existe
using (var scope = app.Services.CreateScope()) {
    // Crea la DB usando el servicio registrado
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    db.Database.EnsureCreated();
}

//  END-POINTS
app.MapGet("/contactos", async (string q, AppDb db) => {
    var consulta = db.Contactos.AsQueryable();

    if (!string.IsNullOrWhiteSpace(q)) {
        q = q.ToLowerInvariant();
        // Si q es un número, buscar por ID
        if (int.TryParse(q, out int idBuscado)) {
            consulta = consulta.Where(c => c.Id == idBuscado);
        } else {
            consulta = consulta.Where(c =>
                c.Nombre.ToLower().Contains(q) ||
                c.Apellido.ToLower().Contains(q) ||
                c.Telefono.ToLower().Contains(q) ||
                c.Email.ToLower().Contains(q)
            );
        }
    }

    return await consulta.OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ToListAsync();
});

app.MapGet("/contactos/{id}", async (int id, AppDb db) => {
    return await db.Contactos.FindAsync(id) is { } contacto
        ? Results.Ok(contacto)
        : Results.NotFound(new { mensaje = "Contacto no encontrado" });
});

app.MapPost("/contactos", async (Contacto nuevo, AppDb db) => {
    db.Contactos.Add(nuevo);
    await db.SaveChangesAsync();
    return Results.Created($"/contactos/{nuevo.Id}", nuevo);
});

app.MapPut("/contactos/{id}", async (int id, Contacto datos, AppDb db) => {
    if (await db.Contactos.FindAsync(id) is not Contacto c) {
        return Results.NotFound(new { mensaje = "Contacto no encontrado" });
    }
    if (datos.Nombre is { Length: > 0 } nombre) {
        c.Nombre = nombre;
    }
    if (datos.Apellido is { Length: > 0 } apellido) {
        c.Apellido = apellido;
    }
    if (datos.Telefono is { Length: > 0 } telefono) {
        c.Telefono = telefono;
    }
    if (datos.Email is { Length: > 0 } email) {
        c.Email = email;
    }
    if (datos.Edad is > 0) {
        c.Edad = datos.Edad;
    }
    await db.SaveChangesAsync();
    return Results.Ok(c);
});

app.MapDelete("/contactos/{id}", async (int id, AppDb db) => {
    if (await db.Contactos.FindAsync(id) is not Contacto c) {
        return Results.NotFound(new { mensaje = "Contacto no encontrado" });
    }
    db.Contactos.Remove(c);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run("http://localhost:5000"); // Si falla la primera vez, corralo nuevamente.

//  MODELOS
class Contacto {
    public int    Id       { get; set; }
    public string Nombre   { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Email    { get; set; } = "";
    public int    Edad     { get; set; }
}

class AppDb : DbContext {
    public AppDb(DbContextOptions<AppDb> opt) : base(opt) { }
    public DbSet<Contacto> Contactos => Set<Contacto>();

    
}