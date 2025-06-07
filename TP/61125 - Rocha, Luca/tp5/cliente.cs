using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// See https://aka.ms/new-console-template for more information
await MainAsync();

async Task MainAsync()
{
    var baseUrl = "http://localhost:5000";
    var http = new HttpClient();
    var jsonOpt = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    while (true)
    {
        Console.WriteLine("\n========================");
        Console.WriteLine("\n---- MENÚ ----");
        Console.WriteLine("1. Listar productos");
        Console.WriteLine("2. Ver productos a reponer");
        Console.WriteLine("3. Agregar stock");
        Console.WriteLine("4. Quitar stock");
        Console.WriteLine("0. Salir");
        Console.Write("Seleccione una opción: ");
        Console.WriteLine("\n========================");
        var opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                await ListarProductosAsync();
                break;

            case "2":
                await ListarAReponerAsync();
                break;

            case "3":
                await CambiarStockAsync("agregar");
                break;

            case "4":
                await CambiarStockAsync("quitar");
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }

    async Task ListarProductosAsync()
    {
        var json = await http.GetStringAsync($"{baseUrl}/productos");
        var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
        Console.WriteLine("\n=== Productos ===");
        foreach (var p in productos)
            Console.WriteLine($"{p.Id,2} {p.Nombre,-20} {p.Precio,10:c}  Stock: {p.Stock}");
    }

    async Task ListarAReponerAsync()
    {
        var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
        var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;

        if (productos?.Count == 0)
        {
            Console.WriteLine("Todos los productos tienen suficiente stock.");
            return;
        }

        Console.WriteLine("\n=== A reponer (stock < 3) ===");
        foreach (var p in productos)
            Console.WriteLine($"{p.Id,2} {p.Nombre,-20} Stock: {p.Stock}");
    }

    async Task CambiarStockAsync(string accion)
    {
        Console.Write("ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        Console.Write("Cantidad: ");
        if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad < 1)
        {
            Console.WriteLine("Cantidad inválida.");
            return;
        }

        var url = $"{baseUrl}/productos/{id}/{accion}?cantidad={cantidad}";
        var resp = await http.PostAsync(url, null);
        var resultado = await resp.Content.ReadAsStringAsync();

        if (resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"✅ Stock {accion}do correctamente.");
        }
        else
        {
            Console.WriteLine($"❌ Error: {resultado}");
        }
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
