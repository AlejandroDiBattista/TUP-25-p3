using System.Net.Http;
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

async Task ShowAllProducts()
{
    var products = await client.GetFromJsonAsync<List<Product>>("/products");
    foreach (var p in products)
        Console.WriteLine($"{p.Id}: {p.Name} (Stock: {p.Stock})");
}

async Task ShowProductsToReplenish()
{
    var products = await client.GetFromJsonAsync<List<Product>>("/products/replenish");
    foreach (var p in products)
        Console.WriteLine($"{p.Id}: {p.Name} (Stock: {p.Stock})");
}

async Task ModifyStock(string action)
{
    Console.Write("ID del producto: ");
    int id = int.Parse(Console.ReadLine());
    Console.Write("Cantidad: ");
    int amount = int.Parse(Console.ReadLine());

    var response = await client.PostAsync($"/products/{id}/{action}/{amount}", null);
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

bool running = true;
while (running)
{
    Console.WriteLine("\n1. Listar productos");
    Console.WriteLine("2. Listar productos a reponer");
    Console.WriteLine("3. Agregar stock");
    Console.WriteLine("4. Quitar stock");
    Console.WriteLine("5. Salir");
    Console.Write("Opción: ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1": await ShowAllProducts(); break;
        case "2": await ShowProductsToReplenish(); break;
        case "3": await ModifyStock("add"); break;
        case "4": await ModifyStock("remove"); break;
        case "5": running = false; break;
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Stock { get; set; }
}
