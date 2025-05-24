using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

class Program {
    static readonly HttpClient http = new();
    static readonly string baseUrl = "http://localhost:5000";
    static readonly JsonSerializerOptions jsonOpt = new(JsonSerializerDefaults.Web);

    static async Task<List<Producto>> TraerProductosAsync() {
        var json = await http.GetStringAsync($"{baseUrl}/productos");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    }

    static async Task<List<Producto>> TraerProductosParaReponerAsync() {
        var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
        return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
    }

    static async Task ModificarStockAsync(int id, int cantidad, bool agregar) {
        string endpoint = agregar ? "agregar" : "quitar";
        var datos = JsonSerializer.Serialize(new { cantidad });
        var content = new StringContent(datos, Encoding.UTF8, "application/json");

        var resp = await http.PutAsync($"{baseUrl}/productos/{id}/{endpoint}", content);
        if (resp.IsSuccessStatusCode)
            Console.WriteLine("Stock actualizado correctamente.");
        else
            Console.WriteLine($"Error al modificar stock: {resp.StatusCode}");
    }

    static async Task Main() {
        while (true) {
            Console.WriteLine("\n=== Menú ===");
            Console.WriteLine("1. Listar productos");
            Console.WriteLine("2. Listar productos a reponer");
            Console.WriteLine("3. Agregar stock");
            Console.WriteLine("4. Quitar stock");
            Console.WriteLine("5. Salir");
            Console.Write("Opción: ");
            var op = Console.ReadLine();

            switch (op) {
                case "1":
                    Console.WriteLine("=== Productos ===");
                    foreach (var p in await TraerProductosAsync())
                        Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,10:c}  Stock: {p.Stock}");
                    break;

                case "2":
                    Console.WriteLine("=== Productos a reponer ===");
                    foreach (var p in await TraerProductosParaReponerAsync())
                        Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Stock,5} unidades");
                    break;

                case "3":
                    Console.Write("ID producto: ");
                    int idAgregar = int.Parse(Console.ReadLine()!);
                    Console.Write("Cantidad a agregar: ");
                    int cantAgregar = int.Parse(Console.ReadLine()!);
                    await ModificarStockAsync(idAgregar, cantAgregar, true);
                    break;

                case "4":
                    Console.Write("ID producto: ");
                    int idQuitar = int.Parse(Console.ReadLine()!);
                    Console.Write("Cantidad a quitar: ");
                    int cantQuitar = int.Parse(Console.ReadLine()!);
                    await ModificarStockAsync(idQuitar, cantQuitar, false);
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }
}
