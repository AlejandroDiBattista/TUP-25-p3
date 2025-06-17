using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class clienteApp
{
    static async Task Main()
    {
        var baseUrl = "http://localhost:5000";
        var http = new HttpClient();
        var jsonOpt = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        await MostrarMenu();

        async Task MostrarMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Menú ---");
                Console.WriteLine("1. Listar todos los productos");
                Console.WriteLine("2. Listar productos a reponer (stock < 3)");
                Console.WriteLine("3. Agregar stock a un producto");
                Console.WriteLine("4. Quitar stock a un producto");
                Console.WriteLine("5. Salir");
                Console.Write("Elegí una opción: ");

                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        await ListarProductos();
                        break;
                    case "2":
                        await ListarProductosAReponer();
                        break;
                    case "3":
                        await ModificarStock(true);
                        break;
                    case "4":
                        await ModificarStock(false);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción inválida, probá de nuevo.");
                        break;
                }
            }
        }

        async Task ListarProductos()
        {
            var productos = await TraerProductos($"{baseUrl}/productos");
            Console.WriteLine("\nProductos disponibles:");
            foreach (var p in productos)
                Console.WriteLine($"ID: {p.Id} - {p.Nombre} - Precio: {p.Precio:C} - Stock: {p.Stock}");
        }

        async Task ListarProductosAReponer()
        {
            var productos = await TraerProductos($"{baseUrl}/productos/reponer");
            Console.WriteLine("\nProductos para reponer (stock < 3):");
            foreach (var p in productos)
                Console.WriteLine($"ID: {p.Id} - {p.Nombre} - Stock: {p.Stock}");
        }

        async Task ModificarStock(bool agregar)
        {
            Console.Write("ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }
            Console.Write("Cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidad) || cantidad <= 0)
            {
                Console.WriteLine("Cantidad inválida.");
                return;
            }

            var url = agregar
                ? $"{baseUrl}/productos/agregar-stock/{id}/{cantidad}"
                : $"{baseUrl}/productos/quitar-stock/{id}/{cantidad}";

            var response = await http.PostAsync(url, null);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var producto = JsonSerializer.Deserialize<Producto>(json, jsonOpt);
                Console.WriteLine($"Stock actualizado: {producto?.Nombre} - Nuevo stock: {producto?.Stock}");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error}");
            }
        }

        async Task<List<Producto>> TraerProductos(string url)
        {
            var json = await http.GetStringAsync(url);
            return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
        }
    }

    class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}