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

Console.WriteLine("=== Artículos de Librería ===");
foreach (var a in await TraerAsync()) {
    Console.WriteLine($"{a.Id} {a.Nombre,-25} {a.Precio,10:c} Stock: {a.Stock}");
}

while (true)
{
    Console.WriteLine("\n---------- MENÚ ----------");
    Console.WriteLine("1. Listar todos los artículos");
    Console.WriteLine("2. Listar artículos que se deben reponer");
    Console.WriteLine("3. Agregar stock a un artículo");
    Console.WriteLine("4. Quitar stock a un artículo");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    var opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            Console.Clear();
            await ListarArticulos();
            break;
        case "2":
            Console.Clear();
            await ListarReponer();
            break;
        case "3":
            Console.Clear();
            await ModificarStock("/articulos/agregar", "agregar");
            break;
        case "4":
            Console.Clear();
            await ModificarStock("/articulos/quitar", "quitar");
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

async Task<List<Articulo>> TraerAsync()
{
    try
    {
        return await cliente.GetFromJsonAsync<List<Articulo>>("/articulos", jsonOpt) ?? new List<Articulo>();
    }
    catch
    {
        Console.WriteLine("Error al obtener los artículos.");
        return new List<Articulo>();
    }
}

async Task ListarArticulos()
{
    var articulos = await TraerAsync();
    Console.WriteLine("---------- Lista de artículos ----------");
    foreach (var a in articulos)
        Console.WriteLine($"ID: {a.Id} - {a.Nombre} - Stock: {a.Stock}");
}

async Task ListarReponer()
{
    try
    {
        var articulos = await cliente.GetFromJsonAsync<List<Articulo>>("/articulos/reponer", jsonOpt);
        Console.WriteLine("---------- Artículos que necesitan reposición ----------");
        if (articulos!.Count == 0)
            Console.WriteLine("Todos los artículos tienen suficiente stock.");
        else
        {
            foreach (var a in articulos)
                Console.WriteLine($"ID: {a.Id} - {a.Nombre} - Stock: {a.Stock}");
        }
    }
    catch
    {
        Console.WriteLine("Error al obtener artículos con stock bajo.");
    }
}

async Task ModificarStock(string endpoint, string accion)
{
    Console.Write($"Ingrese el ID del artículo al que desea {accion} stock: ");
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
        var articuloActualizado = await respuesta.Content.ReadFromJsonAsync<Articulo>(jsonOpt);
        Console.WriteLine($"Artículo actualizado: {articuloActualizado!.Nombre}, Stock: {articuloActualizado.Stock}");
    }
    else
    {
        var error = await respuesta.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {respuesta.StatusCode} - {error}");
    }
}

// === CLASES ===

class Articulo
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
