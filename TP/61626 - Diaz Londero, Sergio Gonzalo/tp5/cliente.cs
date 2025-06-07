using System;
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

while (true) {
    Console.WriteLine("\n=== Menú ===");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a producto");
    Console.WriteLine("4. Quitar stock a producto");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var opcion = Console.ReadLine();

    if (opcion == "0") break;
    else if (opcion == "1") {
        var productos = await TraerProductosAsync();
        Mostrar(productos);
    }
    else if (opcion == "2") {
        var productos = await TraerProductosAsync("/productos/reponer");
        Mostrar(productos);
    }
    else if (opcion == "3" || opcion == "4") {
        Console.Write("ID del producto: ");
        int id = int.Parse(Console.ReadLine()!);
        Console.Write("Cantidad: ");
        int cantidad = int.Parse(Console.ReadLine()!);
        string ruta = (opcion == "3") ? "agregar" : "quitar";
        var resp = await http.PostAsync($"{baseUrl}/productos/{ruta}/{id}/{cantidad}", null);
        var msg = await resp.Content.ReadAsStringAsync();
        Console.WriteLine(resp.IsSuccessStatusCode ? "Operación exitosa" : $"Error: {msg}");
    }
}

async Task<List<Producto>> TraerProductosAsync(string path = "/productos") {
    var json = await http.GetStringAsync($"{baseUrl}{path}");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

void Mostrar(List<Producto> productos) {
    Console.WriteLine("\nID  Nombre                 Precio     Stock");
    Console.WriteLine("---------------------------------------------");
    foreach (var p in productos) {
        Console.WriteLine($"{p.Id,-3} {p.Nombre,-20} {p.Precio,8:c} {p.Stock,8}");
    }
}

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
