using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

#r "nuget: System.Net.Http.Json, 7.0.0"

var baseUrl = "http://localhost:5000";
var cliente = new HttpClient { BaseAddress = new Uri(baseUrl) };

var jsonOpt = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

Console.WriteLine("=== Prendas de Tienda ===");

foreach (var p in await TraerAsync()) {
    Console.WriteLine($"{p.Id} {p.Nombre,-15} {p.Precio,8:c} Stock: {p.Stock}");
}

while (true)
{
    Console.WriteLine("\n---------- MENÚ ----------");
    Console.WriteLine("1. Listar todas las prendas");
    Console.WriteLine("2. Listar prendas que se deben reponer");
    Console.WriteLine("3. Agregar stock a una prenda");
    Console.WriteLine("4. Quitar stock a una prenda");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            Console.Clear();
            await ListarPrendas();
            break;
        case "2":
            Console.Clear();
            await ListarReponer();
            break;
        case "3":
            Console.Clear();
            await ModificarStock("/prendas/agregar", "agregar");
            break;
        case "4":
            Console.Clear();
            await ModificarStock("/prendas/quitar", "quitar");
            break;
        case "0":
            Console.Clear();
            return;
        default:
            Console.Clear();
            Console.WriteLine("Opción inválida. Por favor ingrese una opción válida.");
            break;
    }

    Console.WriteLine("Presione una tecla para continuar...");
    Console.ReadKey(true);
}

// === FUNCIONES ===

async Task<List<Prenda>> TraerAsync()
{
    try
    {
        return await cliente.GetFromJsonAsync<List<Prenda>>("/prendas", jsonOpt) ?? new List<Prenda>();
    }
    catch
    {
        Console.WriteLine("Error al obtener las prendas.");
        return new List<Prenda>();
    }
}

async Task ListarPrendas()
{
    var prendas = await TraerAsync();
    Console.WriteLine("---------- Lista de prendas ----------");
    foreach (var p in prendas)
        Console.WriteLine($"ID: {p.Id} - {p.Nombre} - Stock: {p.Stock}");
}

async Task ListarReponer()
{
    try
    {
        var prendas = await cliente.GetFromJsonAsync<List<Prenda>>("/prendas/reponer", jsonOpt);
        Console.WriteLine("---------- Prendas que necesitan reposición ----------");
        if (prendas!.Count == 0)
            Console.WriteLine("Todas las prendas tienen suficiente stock.");
        else
        {
            foreach (var p in prendas)
                Console.WriteLine($"ID: {p.Id} - {p.Nombre} - Stock: {p.Stock}");
        }
    }
    catch
    {
        Console.WriteLine("Error al obtener prendas con stock bajo.");
    }
}

async Task ModificarStock(string endpoint, string accion)
{
    Console.Write($"Ingrese el ID de la prenda a la que desea {accion} stock: ");
    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    Console.Write($"Ingrese la cantidad a {accion}: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
    {
        Console.WriteLine("Cantidad inválida.");
        return;
    }

    var data = new StockUpdate { Id = id, Cantidad = cantidad };
    var respuesta = await cliente.PostAsJsonAsync(endpoint, data);

    if (respuesta.IsSuccessStatusCode)
    {
        var prendaActualizada = await respuesta.Content.ReadFromJsonAsync<Prenda>(jsonOpt);
        Console.WriteLine($"Prenda actualizada: {prendaActualizada!.Nombre}, Stock: {prendaActualizada.Stock}");
    }
    else
    {
        var error = await respuesta.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {respuesta.StatusCode} - {error}");
    }
}

class Prenda
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

class StockUpdate
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
}