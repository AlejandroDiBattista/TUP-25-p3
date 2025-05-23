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
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    // Mostrar los nombres para depuración
    foreach (var p in productos) Console.WriteLine($"DEBUG: {p.Id} {p.Nombre}");
    return productos;
}

async Task<List<Producto>> TraerProductosABajoStockAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    var productos = JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    // Mostrar los nombres para depuración
    foreach (var p in productos) Console.WriteLine($"DEBUG: {p.Id} {p.Nombre}");
    return productos;
}

async Task AgregarStockAsync(string id, int cantidad) {
    var url = $"{baseUrl}/productos/{id}/agregar?cantidad={cantidad}";
    var response = await http.PostAsync(url, null);
    Console.WriteLine(response.IsSuccessStatusCode
        ? "Stock agregado correctamente."
        : $"Error al agregar stock: {await response.Content.ReadAsStringAsync()}");
}

async Task QuitarStockAsync(string id, int cantidad) {
    var url = $"{baseUrl}/productos/{id}/quitar?cantidad={cantidad}";
    var response = await http.PostAsync(url, null);
    Console.WriteLine(response.IsSuccessStatusCode
        ? "Stock quitado correctamente."
        : $"Error al quitar stock: {await response.Content.ReadAsStringAsync()}");
}

async Task MenuAsync() {
    while (true) {
        Console.WriteLine("""
            === Menú ===
            1. Listar productos
            2. Listar productos a reponer
            3. Agregar stock
            4. Quitar stock
            0. Salir
            """);

        Console.Write("Opción: ");
        var op = Console.ReadLine();

        switch (op) {
            case "1":
                foreach (var p in await TraerProductosAsync())
                    Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Stock,5} unidades");
                break;

            case "2":
                foreach (var p in await TraerProductosABajoStockAsync())
                    Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Stock,5} unidades");
                break;

            case "3":
                Console.Write("ID del producto: ");
                string idAdd = Console.ReadLine()!;
                Console.Write("Cantidad a agregar: ");
                int cantidadAdd = int.Parse(Console.ReadLine()!);
                await AgregarStockAsync(idAdd, cantidadAdd);
                break;

            case "4":
                Console.Write("ID del producto: ");
                string idQuitar = Console.ReadLine()!;
                Console.Write("Cantidad a quitar: ");
                int cantidadQuitar = int.Parse(Console.ReadLine()!);
                await QuitarStockAsync(idQuitar, cantidadQuitar);
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }

        Console.WriteLine();
    }
}

await MenuAsync();

class Producto {
    public string Id { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public int Stock { get; set; }
}
