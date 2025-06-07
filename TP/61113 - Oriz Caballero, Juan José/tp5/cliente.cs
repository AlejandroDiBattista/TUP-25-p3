using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var opcionesJson = new JsonSerializerOptions {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};
//¿HICE EL PULL MAL?????????????
while (true)
{
    Console.WriteLine("---- MENÚ ----");
    Console.WriteLine("1 - Ver productos");
    Console.WriteLine("2 - Ver productos a reponer");
    Console.WriteLine("3 - Agregar stock");
    Console.WriteLine("4 - Quitar stock");
    Console.WriteLine("5 - Salir");
    Console.Write("Ingrese opción: ");
    var op = Console.ReadLine();

    if (op == "1")
    {
        var productos = await TraerProductos();
        Console.WriteLine("--- Lista de productos ---");
        foreach (var p in productos)
        {
            Console.WriteLine($"ID: {p.Id} | {p.Nombre} | ${p.Precio} | Stock: {p.Stock}");
        }
    }
    else if (op == "2")
    {
        var pocos = await TraerReposicion();
        Console.WriteLine("--- Productos con poco stock ---");
        foreach (var p in pocos)
        {
            Console.WriteLine($"ID: {p.Id} | {p.Nombre} | Stock: {p.Stock}");
        }
    }
    else if (op == "3")
    {
        Console.Write("ID producto: ");
        int id = int.Parse(Console.ReadLine()!);
        Console.Write("Cantidad a agregar: ");
        int cant = int.Parse(Console.ReadLine()!);
        var r = await http.PostAsync($"{baseUrl}/productos/{id}/agregar?cantidad={cant}", null);
        Console.WriteLine(r.IsSuccessStatusCode ? "Stock agregado." : "Error al agregar.");
    }
    else if (op == "4")
    {
        Console.Write("ID producto: ");
        int id = int.Parse(Console.ReadLine()!);
        Console.Write("Cantidad a quitar: ");
        int cant = int.Parse(Console.ReadLine()!);
        var r = await http.PostAsync($"{baseUrl}/productos/{id}/quitar?cantidad={cant}", null);
        Console.WriteLine(r.IsSuccessStatusCode ? "Stock quitado." : "Error al quitar.");
    }
    else if (op == "5")
    {
        break;
    }
    else
    {
        Console.WriteLine("Opción no válida");
    }

    Console.WriteLine();
}

async Task<List<Producto>> TraerProductos()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, opcionesJson)!;
}

async Task<List<Producto>> TraerReposicion()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos/reposicion");
    return JsonSerializer.Deserialize<List<Producto>>(json, opcionesJson)!;
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}
