using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

Console.WriteLine("Cliente iniciado.");

bool salir = false;

while (!salir)
{
    Console.WriteLine("\nMenu:");
    Console.WriteLine("1 - Listar productos");
    Console.WriteLine("2 - Listar productos para reponer (stock < 3)");
    Console.WriteLine("3 - Agregar stock a un producto");
    Console.WriteLine("4 - Quitar stock a un producto");
    Console.WriteLine("5 - Salir");
    Console.Write("Elija una opcion: ");

    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await ListarProductos();
            break;

        case "2":
            await ListarProductosParaReponer();
            break;

        case "3":
            await ModificarStock(true);
            break;

        case "4":
            await ModificarStock(false);
            break;

        case "5":
            salir = true;
            break;

        default:
            Console.WriteLine("Opción inválida, intente de nuevo.");
            break;
    }
}

Console.WriteLine("Cliente finalizado.");

async Task ListarProductos()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>("/productos");
    Console.WriteLine("\nListado de productos:");
    foreach (var p in productos)
    {
        Console.WriteLine($"Id: {p.Id}, Nombre: {p.Nombre}, Precio: {p.Precio}, Stock: {p.Stock}");
    }
}

async Task ListarProductosParaReponer()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>("/productos/bajo-stock");
    Console.WriteLine("\nProductos para reponer (stock < 3):");
    if (productos == null || productos.Count == 0)
    {
        Console.WriteLine("No hay productos para reponer.");
        return;
    }
    foreach (var p in productos)
    {
        Console.WriteLine($"Id: {p.Id}, Nombre: {p.Nombre}, Stock: {p.Stock}");
    }
}

async Task ModificarStock(bool agregar)
{
    Console.Write("Ingrese el Id del producto: ");
    var idStr = Console.ReadLine();
    if (!int.TryParse(idStr, out int id))
    {
        Console.WriteLine("Id inválido.");
        return;
    }

    Console.Write("Ingrese la cantidad: ");
    var cantidadStr = Console.ReadLine();
    if (!int.TryParse(cantidadStr, out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida.");
        return;
    }

    string accion = agregar ? "agregar" : "quitar";
    var response = await client.PostAsync($"/productos/{id}/{accion}?cantidad={cantidad}", null);

    if (response.IsSuccessStatusCode)
    {
        var productoActualizado = await response.Content.ReadFromJsonAsync<Producto>();
        Console.WriteLine($"Producto actualizado: Id: {productoActualizado.Id}, Stock: {productoActualizado.Stock}");
    }
    else
    {
        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Error al {accion} stock: {response.StatusCode} - {error}");
    }
}