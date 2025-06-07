using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Net.Http.Json;       // GetFromJsonAsync, ReadFromJsonAsync
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using System.Collections.Generic; // List<T>

var baseUrl = "http://localhost:5000";
var client = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

bool continuar = true;

while (continuar)
{
    Console.WriteLine("\n--- TIENDA DE ROPA DEPORTIVA ---");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Ver productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("5. Salir");
    Console.Write("Seleccione una opción: ");
    string? opcion = Console.ReadLine();
    Console.WriteLine();

    switch (opcion)
    {
        case "1":
            var productos = await client.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos");
            MostrarProductos(productos);
            break;
        case "2":
            var reponer = await client.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/reponer");
            MostrarProductos(reponer);
            break;
        case "3":
            Console.Write("ID del producto: ");
            int idAgregar = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a agregar: ");
            int cantAgregar = int.Parse(Console.ReadLine()!);
            var r1 = await client.PostAsync($"{baseUrl}/productos/{idAgregar}/agregar/{cantAgregar}", null);
            await MostrarRespuesta(r1);
            break;
        case "4":
            Console.Write("ID del producto: ");
            int idQuitar = int.Parse(Console.ReadLine()!);
            Console.Write("Cantidad a quitar: ");
            int cantQuitar = int.Parse(Console.ReadLine()!);
            var r2 = await client.PostAsync($"{baseUrl}/productos/{idQuitar}/quitar/{cantQuitar}", null);
            await MostrarRespuesta(r2);
            break;
        case "5":
            continuar = false;
            break;
        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

void MostrarProductos(List<Producto>? productos)
{
    if (productos is null || productos.Count == 0)
    {
        Console.WriteLine("No hay productos.");
        return;
    }

    Console.WriteLine("ID\tNombre\t\t\tStock");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id}\t{p.Nombre.PadRight(20)}\t{p.Stock}");
}

async Task MostrarRespuesta(HttpResponseMessage resp)
{
    if (resp.IsSuccessStatusCode)
    {
        var producto = await resp.Content.ReadFromJsonAsync<Producto>();
        Console.WriteLine($"✔ Nuevo stock de '{producto?.Nombre}': {producto?.Stock}");
    }
    else
    {
        var error = await resp.Content.ReadAsStringAsync();
        Console.WriteLine($"✖ Error: {error}");
    }
}

record Producto(int Id, string Nombre, int Stock);

// Fin del ejemplo