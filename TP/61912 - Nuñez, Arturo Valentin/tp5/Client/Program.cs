using System.Text.Json;

var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5000/");

while (true)
{
    Console.WriteLine("\n=== ADMINISTRACIÓN DE STOCK ===");
    Console.WriteLine("1. Listar productos disponibles");
    Console.WriteLine("2. Listar productos con stock bajo");
    Console.WriteLine("3. Agregar stock a un producto");
    Console.WriteLine("4. Quitar stock a un producto");
    Console.WriteLine("5. Salir");
    Console.Write("Seleccione una opción: ");

    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
            await ListProducts();
            break;
        case "2":
            await ListLowStockProducts();
            break;
        case "3":
            await AddStock();
            break;
        case "4":
            await RemoveStock();
            break;
        case "5":
            Console.WriteLine("¡Hasta luego!");
            return;
        default:
            Console.WriteLine("Opción no válida");
            break;
    }
}

async Task ListProducts()
{
    try
    {
        var response = await client.GetAsync("products");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(content);
            
            Console.WriteLine("\n=== PRODUCTOS DISPONIBLES ===");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id} | {product.Name} | Precio: ${product.Price} | Stock: {product.Stock}");
            }
        }
        else
        {
            Console.WriteLine("Error al obtener productos");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

async Task ListLowStockProducts()
{
    try
    {
        var response = await client.GetAsync("products/low-stock");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(content);
            
            Console.WriteLine("\n=== PRODUCTOS CON STOCK BAJO ===");
            if (products.Count == 0)
            {
                Console.WriteLine("No hay productos con stock bajo");
            }
            else
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"ID: {product.Id} | {product.Name} | Stock: {product.Stock}");
                }
            }
        }
        else
        {
            Console.WriteLine("Error al obtener productos con stock bajo");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

async Task AddStock()
{
    try
    {
        Console.Write("Ingrese el ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int productId))
        {
            Console.WriteLine("ID inválido");
            return;
        }

        Console.Write("Ingrese la cantidad a agregar: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Cantidad inválida");
            return;
        }

        var response = await client.PostAsync($"products/{productId}/add-stock?quantity={quantity}", null);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(content);
            Console.WriteLine($"Stock actualizado: {product.Name} - Stock: {product.Stock}");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

async Task RemoveStock()
{
    try
    {
        Console.Write("Ingrese el ID del producto: ");
        if (!int.TryParse(Console.ReadLine(), out int productId))
        {
            Console.WriteLine("ID inválido");
            return;
        }

        Console.Write("Ingrese la cantidad a quitar: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Cantidad inválida");
            return;
        }

        var response = await client.PostAsync($"products/{productId}/remove-stock?quantity={quantity}", null);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(content);
            Console.WriteLine($"Stock actualizado: {product.Name} - Stock: {product.Stock}");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
} 