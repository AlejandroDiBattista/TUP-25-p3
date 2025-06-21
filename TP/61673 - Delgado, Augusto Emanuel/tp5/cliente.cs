using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

while (true)
{
    Console.WriteLine("\n--- Menú Tienda ---");
    Console.WriteLine("1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("0. Salir");
    Console.Write("Opción: ");
    var op = Console.ReadLine();
    switch (op)
    {
        case "1": await ListarProductos(); break;
        case "2": await ListarAReponer(); break;
        case "3": await ModificarStock(true); break;
        case "4": await ModificarStock(false); break;
        case "0": return;
        default: Console.WriteLine("Opción inválida"); break;
    }
}

async Task ListarProductos()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>("/productos");
    Console.WriteLine("\nID\tNombre\tStock");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id}\t{p.Nombre}\t{p.Stock}");
}

async Task ListarAReponer()
{
    var productos = await client.GetFromJsonAsync<List<Producto>>("/productos/reponer");
    Console.WriteLine("\nID\tNombre\tStock");
    foreach (var p in productos)
        Console.WriteLine($"{p.Id}\t{p.Nombre}\t{p.Stock}");
}

async Task ModificarStock(bool agregar)
{
    Console.Write("ID del producto: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID inválido"); return; }
    Console.Write("Cantidad: ");
    if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0) { Console.WriteLine("Cantidad inválida"); return; }
    var url = agregar ? $"/productos/{id}/agregar" : $"/productos/{id}/quitar";
    var resp = await client.PostAsJsonAsync(url, new { cantidad });
    if (resp.IsSuccessStatusCode)
        Console.WriteLine("Operación exitosa");
    else
        Console.WriteLine($"Error: {await resp.Content.ReadAsStringAsync()}");
}

// Clases auxiliares al final del script
public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
}