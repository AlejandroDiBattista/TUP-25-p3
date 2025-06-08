#r "nuget: System.Net.Http.Json, 7.0.0"

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

var baseUrl = "http://localhost:5000";
var cliente = new HttpClient { BaseAddress = new Uri(baseUrl) };

var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};


class Producto {
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int Stock { get; set; }
}

class StockUpdate {
    public int Id { get; set; }
    public int Cantidad { get; set; }
}


while (true)
{
    Console.WriteLine("---------- MENÚ ----------");
    Console.WriteLine("1. Listar todos los productos");
    Console.WriteLine("2. Listar productos que se deben reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1": Console.Clear();
            await ListarProductos();
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey(true);
            break;
        case "2": Console.Clear();
            await ListarReponer();
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey(true);
            break;
        case "3": Console.Clear();
            await ModificarStock("/productos/agregar", "agregar");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey(true);
            break;
        case "4": Console.Clear();
            await ModificarStock("/productos/quitar", "quitar");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey(true);
            break;
        case "0": Console.Clear();
            return;
        default: Console.Clear();
            Console.WriteLine("Opción inválida , Porfavor ingrese un numero de opción Correcto !!.");
            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey(true);
            break;
    }
}


async Task ListarProductos()
{
    try
    {
        var productos = await cliente.GetFromJsonAsync<List<Producto>>("/productos", jsonOpt);
        Console.WriteLine("---------- Lista de productos ----------");
        foreach (var p in productos!)
            Console.WriteLine($"ID: {p.Id} - {p.Nombre} - Stock: {p.Stock}");
    }
    catch
    {
        Console.WriteLine(" Hay error al obtener la lista de productos.");
    }
}

async Task ListarReponer()
{
    try
    {
        var productos = await cliente.GetFromJsonAsync<List<Producto>>("/productos/reponer", jsonOpt);
        Console.WriteLine("---------- Productos que necesitan reposición ----------");
        if (productos!.Count == 0)
        {
            Console.WriteLine("Todos los productos tienen suficiente stock.");
        }
        else
        {
            foreach (var p in productos)
                Console.WriteLine($"ID: {p.Id} - {p.Nombre} - Stock: {p.Stock}");
        }
    }
    catch
    {
        Console.WriteLine(" Hay error al obtener productos con stock bajo.");
    }
}

async Task ModificarStock(string endpoint, string accion)
{
    Console.Write($"Ingrese el ID del producto al que desea {accion} stock: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido , ingrese uno correcto !!.");
        return;
    }

    Console.Write($"Ingrese la cantidad a {accion}: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida , ingrese una cantidad valida !!.");
        return;
    }

    var data = new StockUpdate { Id = id, Cantidad = cantidad };
    var respuesta = await cliente.PostAsJsonAsync(endpoint, data);

    if (respuesta.IsSuccessStatusCode)
    {
        var productoActualizado = await respuesta.Content.ReadFromJsonAsync<Producto>(jsonOpt);
        Console.WriteLine($"Producto actualizado: {productoActualizado!.Nombre}, Stock: {productoActualizado.Stock}");
    }
    else
    {
        var error = await respuesta.Content.ReadAsStringAsync();
        Console.WriteLine($" Error: {respuesta.StatusCode} - {error}");
    }
}
