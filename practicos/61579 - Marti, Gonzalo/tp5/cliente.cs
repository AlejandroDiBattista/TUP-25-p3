#r "nuget: System.Net.Http.Json, 7.0.0"
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var apiBase = "http://localhost:5000";
using var client = new HttpClient { BaseAddress = new Uri(apiBase) };

var jsonSettings = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task<List<InventoryItem>> FetchAllAsync()
{
    try
    {
        return await client.GetFromJsonAsync<List<InventoryItem>>("/items", jsonSettings)
            ?? new List<InventoryItem>();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error cargando ítems: {e.Message}");
        return new List<InventoryItem>();
    }
}

async Task<List<InventoryItem>> FetchRestockAsync()
{
    try
    {
        return await client.GetFromJsonAsync<List<InventoryItem>>("/items/restock", jsonSettings)
            ?? new List<InventoryItem>();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error cargando ítems para reposición: {e.Message}");
        return new List<InventoryItem>();
    }
}

async Task<(bool ok, string msg)> AdjustStockAsync(int id, int qty, bool increase)
{
    var payload = new { amount = qty };
    var route = increase ? $"/items/{id}/add" : $"/items/{id}/remove";
    try
    {
        var resp = await client.PostAsJsonAsync(route, payload, jsonSettings);
        if (resp.IsSuccessStatusCode)
            return (true, increase ? "Stock aumentado con éxito." : "Stock reducido con éxito.");

        var err = await resp.Content.ReadAsStringAsync();
        return (false, $"Error ajustando stock: {err}");
    }
    catch (Exception e)
    {
        return (false, $"Error de conexión: {e.Message}");
    }
}

void PrintItems(IEnumerable<InventoryItem> items, string header)
{
    Console.WriteLine($"\n=== {header} ===");
    Console.WriteLine("{0,-4} {1,-20} {2,-10} {3,-8}", "ID", "Nombre", "Precio", "Cantidad");
    Console.WriteLine(new string('-', 50));
    foreach (var itm in items)
        Console.WriteLine("{0,-4} {1,-20} ${2,-9:F2} {3,-8}", itm.Id, itm.Name, itm.Price, itm.Quantity);
}

Console.WriteLine("**** SISTEMA DE INVENTARIO ****");

while (true)
{
    Console.WriteLine("\n1. Ver todos los ítems | 2. Ver reposición | 3. Añadir stock | 4. Quitar stock | 5. Salir");
    Console.Write("Opción: ");
    var choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1":
            var all = await FetchAllAsync();
            if (all.Count > 0) PrintItems(all, "LISTA DE ÍTEMS");
            else Console.WriteLine("No hay ítems disponibles.");
            break;

        case "2":
            var restock = await FetchRestockAsync();
            if (restock.Count > 0) PrintItems(restock, "ÍTEMS A REABASTECER");
            else Console.WriteLine("No se necesitan reabastecer ítems.");
            break;

        case "3":
        case "4":
            Console.Write("Ingrese ID y cantidad (ej: 2 5): ");
            var input = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (input?.Length == 2 && int.TryParse(input[0], out var id) && int.TryParse(input[1], out var qty))
            {
                if (qty > 0)
                {
                    var isAdd = choice == "3";
                    var (ok, msg) = await AdjustStockAsync(id, qty, isAdd);
                    Console.WriteLine(ok ? "✅ " + msg : "❌ " + msg);
                    if (ok) PrintItems(await FetchAllAsync(), "ESTADO ACTUALIZADO");
                }
                else Console.WriteLine("❌ La cantidad debe ser positiva.");
            }
            else Console.WriteLine("❌ Entrada inválida.");
            break;

        case "5":
            Console.WriteLine("Cerrando...");
            return;

        default:
            Console.WriteLine("❌ Opción no reconocida.");
            break;
    }
} 

record InventoryItem(int Id, string Name, decimal Price, int Quantity);