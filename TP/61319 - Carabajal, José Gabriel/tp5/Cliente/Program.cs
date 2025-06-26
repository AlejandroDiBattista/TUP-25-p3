using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Ajusta esta URL si cambias el puerto del servidor
        var client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

        async Task<List<Product>> GetProducts(string path) =>
            await client.GetFromJsonAsync<List<Product>>(path) ?? new();

        async Task ModifyStock(string action)
        {
            Console.Write("Ingrese ID de producto: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) return;
            Console.Write("Cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out var qty)) return;

            var endpoint = action == "add"
                ? $"/products/{id}/add?quantity={qty}"
                : $"/products/{id}/remove?quantity={qty}";

            var resp = await client.PostAsync(endpoint, null);
            if (resp.IsSuccessStatusCode)
            {
                var prod = await resp.Content.ReadFromJsonAsync<Product>();
                Console.WriteLine(JsonSerializer.Serialize(prod));
            }
            else
            {
                Console.WriteLine($"Error: {resp.StatusCode} - {await resp.Content.ReadAsStringAsync()}");
            }
        }

        while (true)
        {
            Console.WriteLine("\n========== M E N Ú ===========");
            Console.WriteLine("1. Listar productos disponibles");
            Console.WriteLine("2. Listar productos que deben reponerse");
            Console.WriteLine("3. Agregar stock a un producto");
            Console.WriteLine("4. Quitar stock de un producto");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
            var opt = Console.ReadLine();

            switch (opt)
            {
                case "1":
                    var all = await GetProducts("/products");
                    all.ForEach(p => Console.WriteLine($"{p.Id}: {p.Name} - Stock: {p.Stock} - Precio: {p.Price:C}"));
                    break;
                case "2":
                    var rep = await GetProducts("/products/reorder");
                    rep.ForEach(p => Console.WriteLine($"{p.Id}: {p.Name} - Stock: {p.Stock} - Precio: {p.Price:C}"));
                    break;
                case "3":
                    await ModifyStock("add");
                    break;
                case "4":
                    await ModifyStock("remove");
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida");
                    break;
            }
        }
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int Stock { get; set; }
    public decimal Price { get; set; }
}
