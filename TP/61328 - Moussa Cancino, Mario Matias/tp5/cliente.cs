#nullable enable
#r "nuget: System.Net.Http.Json, 7.0.0"

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

const string baseUrl = "http://localhost:5000";

HttpClient client = new HttpClient();

await Menu();

async Task Menu()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("/// Programa Cliente-servidor ///");
        Console.WriteLine("1. Lista de productos");
        Console.WriteLine("2. Lista de productos con bajo stock");
        Console.WriteLine("3. Agregar stock de un producto");
        Console.WriteLine("4. Quitar stock de un producto");
        Console.WriteLine("0. Salir");
        Console.Write("Elija una opción: ");

        string? opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                await ListarProductos();
                break;
            case "2":
                await ListarProductosBajoStock();
                break;
            case "3":
                await ModificarStock(true);
                break;
            case "4":
                await ModificarStock(false);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opción inválida. Presione una tecla para continuar...");
                Console.ReadKey();
                break;
        }
    }
}

async Task ListarProductos()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos");
    Console.WriteLine("\n--- Productos Disponibles ---");
    if (productos == null || productos.Count == 0)
    {
        Console.WriteLine("No hay productos disponibles.");
    }
    else
    {
        foreach (var p in productos)
        {
            Console.WriteLine($"ID: {p.Id} | Nombre: {p.Nombre} | Precio: ${p.Precio} | Stock: {p.Stock}");
        }
    }
    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

async Task ListarProductosBajoStock()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>($"{baseUrl}/productos/bajo-stock");
    Console.WriteLine("\n--- Productos Bajo Stock0 ---");
    if (productos == null || productos.Count == 0)
    {
        Console.WriteLine("No hay productos con stock bajo.");
    }
    else
    {
        foreach (var p in productos)
        {
            Console.WriteLine($"ID: {p.Id} | Nombre: {p.Nombre} | Precio: ${p.Precio} | Stock: {p.Stock}");
        }
    }
    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

async Task ModificarStock(bool agregar)
{
    Console.Write("\nIngrese el ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido. Presione una tecla para continuar...");
        Console.ReadKey();
        return;
    }

    Console.Write("Ingrese la cantidad: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida. Presione una tecla para continuar...");
        Console.ReadKey();
        return;
    }

    string accion = agregar ? "agregar" : "quitar";
    string url = $"{baseUrl}/productos/{accion}/{id}/{cantidad}";

    HttpResponseMessage response;

    try
    {
        response = await client.PutAsync(url, null);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error conectando al servidor: {ex.Message}");
        Console.WriteLine("Presione una tecla para continuar...");
        Console.ReadKey();
        return;
    }

    if (response.IsSuccessStatusCode)
    {
        var producto = await response.Content.ReadFromJsonAsync<Producto>();
        Console.WriteLine($"\nStock actualizado: {producto!.Nombre} tiene ahora {producto.Stock} unidades.");
    }
    else
    {
        var errorMsg = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"\nError: {errorMsg}");
    }

    Console.WriteLine("\nPresione una tecla para continuar...");
    Console.ReadKey();
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
