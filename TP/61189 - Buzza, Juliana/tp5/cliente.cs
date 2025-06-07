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

async Task<List<Producto>> ObtenerProductosAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt);
}

async Task<List<Producto>> ObtenerProductosReponerAsync() {
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt);
}

async Task<Producto> AgregarStockAsync(int id, int cantidad) {
    var response = await http.PutAsync($"{baseUrl}/productos/{id}/agregar/{cantidad}", null);
    if (!response.IsSuccessStatusCode) return null;
    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt);
}

async Task<Producto> QuitarStockAsync(int id, int cantidad) {
    var response = await http.PutAsync($"{baseUrl}/productos/{id}/quitar/{cantidad}", null);
    if (!response.IsSuccessStatusCode) {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Error: " + error);
        return null;
    }
    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt);
}

async Task MostrarMenu() {
    while (true) {
        Console.Clear();
        Console.WriteLine("=== GESTIÓN DE STOCK ===");
        Console.WriteLine("1. Listar productos");
        Console.WriteLine("2. Listar productos a reponer");
        Console.WriteLine("3. Agregar stock");
        Console.WriteLine("4. Quitar stock");
        Console.WriteLine("5. Salir");
        Console.Write("\nSeleccione una opción: ");

        var opcion = Console.ReadLine();
        Console.WriteLine();

        switch (opcion) {
            case "1":
                var productos = await ObtenerProductosAsync();
                Console.WriteLine("=== PRODUCTOS DISPONIBLES ===");
                foreach (var p in productos) {
                    Console.WriteLine($"ID: {p.Id}, {p.Nombre,-20} Precio: {p.Precio,8:C} Stock: {p.Stock,3}");
                }
                break;

            case "2":
                var productosReponer = await ObtenerProductosReponerAsync();
                Console.WriteLine("=== PRODUCTOS A REPONER (Stock < 3) ===");
                foreach (var p in productosReponer) {
                    Console.WriteLine($"ID: {p.Id}, {p.Nombre,-20} Stock actual: {p.Stock,3}");
                }
                break;

            case "3":
                Console.Write("ID del producto: ");
                if (int.TryParse(Console.ReadLine(), out int idAgregar)) {
                    Console.Write("Cantidad a agregar: ");
                    if (int.TryParse(Console.ReadLine(), out int cantidadAgregar)) {
                        var resultado = await AgregarStockAsync(idAgregar, cantidadAgregar);
                        if (resultado != null)
                            Console.WriteLine($"Stock actualizado. Nuevo stock: {resultado.Stock}");
                        else
                            Console.WriteLine("Error al actualizar el stock");
                    }
                }
                break;

            case "4":
                Console.Write("ID del producto: ");
                if (int.TryParse(Console.ReadLine(), out int idQuitar)) {
                    Console.Write("Cantidad a quitar: ");
                    if (int.TryParse(Console.ReadLine(), out int cantidadQuitar)) {
                        var resultado = await QuitarStockAsync(idQuitar, cantidadQuitar);
                        if (resultado != null)
                            Console.WriteLine($"Stock actualizado. Nuevo stock: {resultado.Stock}");
                        else
                            Console.WriteLine("Error al actualizar el stock");
                    }
                }
                break;

            case "5":
                return;
        }

        Console.WriteLine("\nPresione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

await MostrarMenu();