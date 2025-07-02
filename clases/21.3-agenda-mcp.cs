#!/usr/bin/env dotnet script
#r "sdk:Microsoft.NET.Sdk.Web"
#r "nuget: ModelContextProtocol, 0.2.0-preview.1"
#r "nuget: Microsoft.EntityFrameworkCore, 9.0.4"
#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.4"

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

public class Contacto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public int Edad { get; set; }
}

public class AgendaContext : DbContext {
    public DbSet<Contacto> Contactos { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=agenda.db");
    }
}

// Inicializar la base de datos (migración automática simple)
using (var db = new AgendaContext()) {
    db.Database.EnsureCreated();
    if (!db.Contactos.Any()) {
        Console.WriteLine("[DEBUG] Inicializando base de datos con contactos de ejemplo...");
        db.Contactos.AddRange(
            new Contacto { Nombre = "Ana López", Telefono = "111-1111", Edad = 25 },
            new Contacto { Nombre = "Bruno Díaz", Telefono = "222-2222", Edad = 30 },
            new Contacto { Nombre = "Carla Gómez", Telefono = "333-3333", Edad = 28 },
            new Contacto { Nombre = "Diego Torres", Telefono = "444-4444", Edad = 35 },
            new Contacto { Nombre = "Elena Ruiz", Telefono = "555-5555", Edad = 22 }
        );
        db.SaveChanges();
    }
}

[McpServerToolType]
public static class AgendaTool {
    [McpServerTool, Description("Agrega un contacto a la agenda.")]
    public static Contacto AgregarContacto(string nombre, string telefono, int edad) {
        using var db = new AgendaContext();
        var contacto = new Contacto { Nombre = nombre, Telefono = telefono, Edad = edad };
        db.Contactos.Add(contacto);
        db.SaveChanges();
        return contacto;
    }

    [McpServerTool, Description("Obtiene todos los contactos de la agenda.")]
    public static List<Contacto> ObtenerContactos() {
        try {
            using var db = new AgendaContext();
            var lista = db.Contactos.ToList();
            Console.WriteLine($"[DEBUG] Se obtuvieron {lista.Count} contactos de la base de datos.");
            return lista;
        } catch (Exception ex) {
            Console.WriteLine($"[ERROR] ObtenerContactos: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    [McpServerTool, Description("Obtiene un contacto por Id.")]
    public static Contacto ObtenerContactoPorId(int id) {
        try {
            using var db = new AgendaContext();
            var contacto = db.Contactos.Find(id);
            if (contacto == null) Console.WriteLine($"[DEBUG] No se encontró contacto con id {id}");
            return contacto;
        } catch (Exception ex) {
            Console.WriteLine($"[ERROR] ObtenerContactoPorId: {ex.Message}\n{ex.StackTrace}");
            throw;
        }
    }

    [McpServerTool, Description("Actualiza un contacto existente.")]
    public static Contacto ActualizarContacto(int id, string nombre, string telefono, int edad) {
        using var db = new AgendaContext();
        var contacto = db.Contactos.Find(id);
        if (contacto == null) {
            throw new ArgumentException("Contacto no encontrado.", nameof(id));
        } else {
            contacto.Nombre = nombre;
            contacto.Telefono = telefono;
            contacto.Edad = edad;
            db.SaveChanges();
            return contacto;
        }
    }

    [McpServerTool, Description("Elimina un contacto por Id.")]
    public static bool EliminarContacto(int id) {
        using var db = new AgendaContext();
        var contacto = db.Contactos.Find(id);
        if (contacto == null) {
            return false;
        } else {
            db.Contactos.Remove(contacto);
            db.SaveChanges();
            return true;
        }
    }
}
