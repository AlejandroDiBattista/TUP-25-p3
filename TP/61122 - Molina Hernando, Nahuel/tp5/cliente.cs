using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task<List<Producto>> TraerProductosAsync() {
    try {
        var json = await http.GetStringAsync($"{baseUrl}/productos");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    } catch {
        Console.WriteLine("No se pudo conectar con el servidor.");
        return new List<Producto>();
    }
}

async Task<List<Producto>> TraerParaReponerAsync() {
    try {
        var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    } catch {
        Console.WriteLine("No se pudo conectar con el servidor.");
        return new List<Producto>();
    }
}

async Task AgregarStockAsync(int id, int cantidad) {
    var url = $"{baseUrl}/productos/{id}/agregar?cantidad={cantidad}";
    var resp = await http.PostAsync(url, null);
    if (resp.IsSuccessStatusCode)
        Console.WriteLine("Stock agregado correctamente.");
    else
        Console.WriteLine("Error: " + await resp.Content.ReadAsStringAsync());
}

async Task QuitarStockAsync(int id, int cantidad) {
    var url = $"{baseUrl}/productos/{id}/quitar?cantidad={cantidad}";
    var resp = await http.PostAsync(url, null);
    if (resp.IsSuccessStatusCode)
        Console.WriteLine("Stock quitado correctamente.");
    else
        Console.WriteLine("Error: " + await resp.Content.ReadAsStringAsync());
}

// === Menú ===

while (true) {
    Console.WriteLine("\n=== MENÚ ===");
    Console.WriteLine("1. Ver productos");
    Console.WriteLine("2. Ver productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var op = Console.ReadLine();

    switch (op) {
        case "1":
            var productos = await TraerProductosAsync();
            if (productos.Count == 0) Console.WriteLine("No hay productos.");
            foreach (var p in productos)
                Console.WriteLine($"{p.Id,2} {p.Nombre,-20} {p.Stock,5} unidades - {p.Precio,10:C}");
            break;

        case "2":
            var pocos = await TraerParaReponerAsync();
            if (pocos.Count == 0) Console.WriteLine("No hay productos a reponer.");
            foreach (var p in pocos)
                Console.WriteLine($"{p.Id,2} {p.Nombre,-20} {p.Stock,5} unidades");
            break;

        case "3":
            Console.Write("ID producto: ");
            if (!int.TryParse(Console.ReadLine(), out int idAdd)) {
                Console.WriteLine("ID inválido.");
                break;
            }
            Console.Write("Cantidad a agregar: ");
            if (!int.TryParse(Console.ReadLine(), out int cantAdd) || cantAdd <= 0) {
                Console.WriteLine("Cantidad inválida.");
                break;
            }
            await AgregarStockAsync(idAdd, cantAdd);
            break;

        case "4":
            Console.Write("ID producto: ");
            if (!int.TryParse(Console.ReadLine(), out int idSub)) {
                Console.WriteLine("ID inválido.");
                break;
            }
            Console.Write("Cantidad a quitar: ");
            if (!int.TryParse(Console.ReadLine(), out int cantSub) || cantSub <= 0) {
                Console.WriteLine("Cantidad inválida.");
                break;
            }
            await QuitarStockAsync(idSub, cantSub);
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Opción inválida.");
            break;
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
