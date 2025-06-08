using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task<List<Producto>> TraerProductosAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerProductosReponerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reponer");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task AgregarStockAsync()
{
    Console.Write("ID del producto: ");
    var id = int.Parse(Console.ReadLine()!);
    Console.Write("Cantidad a agregar: ");
    var cantidad = int.Parse(Console.ReadLine()!);

    var res = await http.PostAsync($"{baseUrl}/productos/{id}/agregar?cantidad={cantidad}", null);
    var msg = await res.Content.ReadAsStringAsync();

    Console.WriteLine(res.IsSuccessStatusCode ? "✅ Stock agregado correctamente." : $"❌ Error: {msg}");
}

async Task QuitarStockAsync()
{
    Console.Write("ID del producto: ");
    var id = int.Parse(Console.ReadLine()!);
    Console.Write("Cantidad a quitar: ");
    var cantidad = int.Parse(Console.ReadLine()!);

    var res = await http.PostAsync($"{baseUrl}/productos/{id}/quitar?cantidad={cantidad}", null);
    var msg = await res.Content.ReadAsStringAsync();

    Console.WriteLine(res.IsSuccessStatusCode ? "✅ Stock quitado correctamente." : $"❌ Error: {msg}");
}

void MostrarProductos(List<Producto> productos)
{
    Console.WriteLine("\nID | Nombre                | Precio     | Stock");
    Console.WriteLine("-----------------------------------------------");
    foreach (var p in productos)
    {
        Console.WriteLine($"{p.Id,2} | {p.Nombre,-20} | {p.Precio,8:c} | {p.Stock,5}");
    }
}


while (true)
{
    Console.WriteLine("\n--- Menú de opciones ---");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    try
    {
        switch (opcion)
        {
            case "1":
                MostrarProductos(await TraerProductosAsync());
                break;

            case "2":
                MostrarProductos(await TraerProductosReponerAsync());
                break;

            case "3":
                await AgregarStockAsync();
                break;

            case "4":
                await QuitarStockAsync();
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
