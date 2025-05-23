using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task
using System.Collections.Generic;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

// Codigo de ejemplo: Reemplazar por la implementacion real 

async Task<List<Producto>> TraerAsync(){
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}
async Task<List<Producto>> TraerParaReponerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}
async Task ModificarStockAsync(bool agregar)
{
    Console.Write(agregar 
        ? "Ingrese ID del producto al que desea agregar stock: " 
        : "Ingrese ID del producto al que desea quitar stock: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido. Presione Enter para continuar...");
        Console.ReadLine();
        return;
    }

    Console.Write("Ingrese cantidad: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida (debe ser un número positivo). Presione Enter para continuar...");
        Console.ReadLine();
        return;
    }

    var dto = new { cantidad }; // serializa { "cantidad": N }
    var contenido = new StringContent(
        JsonSerializer.Serialize(dto, jsonOpt),
        System.Text.Encoding.UTF8,
        "application/json"
    );

    string ruta = agregar
        ? $"{baseUrl}/productos/{id}/agregar-stock"
        : $"{baseUrl}/productos/{id}/quitar-stock";

    var response = await http.PostAsync(ruta, contenido);
    if (response.IsSuccessStatusCode)
    {
        var jsonResp = await response.Content.ReadAsStringAsync();
        var actualizado = JsonSerializer.Deserialize<Producto>(jsonResp, jsonOpt)!;
        Console.WriteLine($"\nOperación exitosa. Stock actual: {actualizado.Stock}");
    }
    else
    {
        var errorTexto = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"\nError ({response.StatusCode}): {errorTexto}");
    }

    Console.WriteLine("Presione Enter para continuar...");
    Console.ReadLine();
}
oid MostrarProductos(IEnumerable<Producto> lista)
{
    Console.WriteLine($"{"ID",4} {"Nombre",-20} {"Precio",8} {"Stock",6}");
    Console.WriteLine(new string('-', 45));
    foreach (var p in lista.OrderBy(p => p.Id))
    {
        Console.WriteLine($"{p.Id,4} {p.Nombre,-20} {p.Precio,8:C0} {p.Stock,6}");
    }
}
Console.WriteLine("=== Productos ===");
foreach (var p in await TraerAsync()) {
    Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c}");
}
while (true)
{
    Console.Clear();
    Console.WriteLine("=== ADMINISTRAR STOCK DE TIENDA ===");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Elija una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            Console.Clear();
            Console.WriteLine("=== Productos Disponibles ===");
            var todos = await TraerAsync();
            MostrarProductos(todos);
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
            break;

        case "2":
            Console.Clear();
            Console.WriteLine("=== Productos a Reponer (stock < 3) ===");
            var repuestos = await TraerParaReponerAsync();
            MostrarProductos(repuestos);
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
            break;

        case "3":
            Console.Clear();
            await ModificarStockAsync(agregar: true);
            break;

        case "4":
            Console.Clear();
            await ModificarStockAsync(agregar: false);
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Opción inválida. Presione Enter para volver al menú...");
            Console.ReadLine();
            break;
    }
}
class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
}

// Fin del ejemplo