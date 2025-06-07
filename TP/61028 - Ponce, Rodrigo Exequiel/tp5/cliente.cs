using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

while (true)
{
    Console.Clear();
    Console.WriteLine("=== GESTIÓN DE STOCK ===");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos a reponer (stock < 3)");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("5. Salir");
    Console.Write("Elija una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await MostrarProductos();
            break;
        case "2":
            await MostrarProductosReponer();
            break;
        case "3":
            await ModificarStock("agregar-stock");
            break;
        case "4":
            await ModificarStock("quitar-stock");
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Opción no válida.");
            break;
    }

    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

async Task MostrarProductos()
{
    try
    {
        var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos", jsonOpt);
        Console.WriteLine("\n--- Productos disponibles ---");
        foreach (var p in productos!)
            Console.WriteLine($"{p.Id,2} {p.Nombre,-20} ${p.Precio,8} | Stock: {p.Stock}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al traer productos: {ex.Message}");
    }
}

async Task MostrarProductosReponer()
{
    try
    {
        var productos = await http.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/reponer", jsonOpt);
        Console.WriteLine("\n--- Productos a reponer ---");
        foreach (var p in productos!)
            Console.WriteLine($"{p.Id,2} {p.Nombre,-20} | Stock: {p.Stock}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al traer productos: {ex.Message}");
    }
}

async Task ModificarStock(string accion)
{
    try
    {
        Console.Write("Ingrese ID del producto: ");
        int id = int.Parse(Console.ReadLine()!);
        Console.Write("Ingrese cantidad: ");
        int cantidad = int.Parse(Console.ReadLine()!);

        var url = $"{baseUrl}/productos/{accion}/{id}?cantidad={cantidad}";
        var response = await http.PutAsync(url, null);
        if (response.IsSuccessStatusCode)
        {
            var producto = await response.Content.ReadFromJsonAsync<Producto>(jsonOpt);
            Console.WriteLine($"\n✔ Stock actualizado: {producto!.Nombre} ahora tiene {producto.Stock} unidades.");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Error: {error}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al modificar stock: {ex.Message}");
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
