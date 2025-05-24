using System;                     // Console, etc.
using System.Linq;                // OrderBy
using System.Net.Http;            // HttpClient, StringContent ✔
using System.Text.Json;           // JsonSerializer, JsonNamingPolicy
using System.Threading.Tasks;     // Task

var baseUrl = "http://localhost:5000";
var http = new HttpClient();
var jsonOpt = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};

async Task<List<Producto>> TraerAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<List<Producto>> TraerProductosBajoStockAsync()
{
    var json = await http.GetStringAsync($"{baseUrl}/productos?stockMenorA=3");
    return JsonSerializer.Deserialize<List<Producto>>(json, jsonOpt)!;
}

async Task<Producto> TraerAsync(int id)
{
    var json = await http.GetStringAsync($"{baseUrl}/productos?id={id}");
    return JsonSerializer.Deserialize<Producto>(json, jsonOpt);
}

async Task<Producto> CargarAsync(Producto p)
{
    var json = JsonSerializer.Serialize(p, jsonOpt);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    var response = await http.PostAsync($"{baseUrl}/productos", content);
    return JsonSerializer.Deserialize<Producto>(await response.Content.ReadAsStringAsync(), jsonOpt)!;
}

async Task<Producto> ActualizarAsync(Producto p)
{
    var json = JsonSerializer.Serialize(p, jsonOpt);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    var response = await http.PutAsync($"{baseUrl}/productos", content);
    return JsonSerializer.Deserialize<Producto>(await response.Content.ReadAsStringAsync(), jsonOpt)!;
}

async Task ListarProductosAsync()
{
    Console.Clear();
    Console.WriteLine("=== Productos ===");
    var productos = await TraerAsync();
    if (productos == null || productos.Count == 0)
    {
        Console.WriteLine("No hay productos disponibles.");
        return;
    }
    foreach (var p in productos)
    {
        Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} {p.Stock,5}");
    }
}

async Task ListarProductosRenovarAsync()
{
    Console.Clear();
    Console.WriteLine("=== Productos a renovar ===");
    var productos = await TraerAsync();
    if (productos == null || productos.Count == 0 || !productos.Any(p => p.Stock > 3))
    {
        Console.WriteLine("No hay productos disponibles.");
        return;
    }
    productos = productos.Where(p => p.Stock <= 3).OrderBy(p => p.Stock).ToList();
    foreach (var p in productos)
    {
        Console.WriteLine($"{p.Id} {p.Nombre,-20} {p.Precio,15:c} {p.Stock,5}");
    }
}

async Task CargarStockAsync()
{
    Console.Clear();
    Console.WriteLine("=== Cargar stock ===");
    int id = ValidarEntrada("Ingrese el ID del producto: ", s => int.TryParse(s, out var v) && v > 0 ? (true, v) : (false, 0));
    var producto = await TraerAsync(id);
    if (producto != null)
    {
        Console.WriteLine($"Producto: {producto.Nombre}");
        Console.WriteLine($"Stock actual: {producto.Stock}");
        int stockCargar = ValidarEntrada("Ingrese la cantidad a cargar: ", s => int.TryParse(s, out var v) && v > 0 ? (true, v) : (false, 0));
        var stockSumado = new Producto
        {
            Id = producto.Id,
            Stock = producto.Stock + stockCargar
        };
        var cargarStock = await ActualizarAsync(stockSumado);
        if (cargarStock != null)
        {
            Console.WriteLine($"Producto actualizado: {cargarStock.Nombre}");
            Console.WriteLine($"Nuevo stock: {cargarStock.Stock}");
        }
        else
        {
            Console.WriteLine("Error al cargar el stock.");
        }
    }
    else
    {
        string nombre = ValidarEntrada("Ingrese el nombre del producto: ", s => !string.IsNullOrEmpty(s) ? (true, s) : (false, ""));
        decimal precio = ValidarEntrada("Ingrese el precio del producto: ", s => decimal.TryParse(s, out var v) && v > 0 ? (true, v) : (false, 0));
        int stock = ValidarEntrada("Ingrese el stock del producto: ", s => int.TryParse(s, out var v) && v > 0 ? (true, v) : (false, 0));
        var nuevoProducto = new Producto
        {
            Nombre = nombre,
            Precio = precio,
            Stock = stock
        };
        var cargarProducto = await CargarAsync(nuevoProducto);
        if (cargarProducto != null)
        {
            Console.WriteLine($"Producto actualizado: {cargarProducto.Nombre}");
            Console.WriteLine($"Nuevo stock: {cargarProducto.Stock}");
        }
        else
        {
            Console.WriteLine("Error al cargar el producto.");
        }
    }
}

async Task QuitarStockAsync()
{
    Console.Clear();
    Console.WriteLine("=== Quitar stock ===");
    int id = ValidarEntrada("Ingrese el ID del producto: ", s => int.TryParse(s, out var v) && v > 0 ? (true, v) : (false, 0));
    var producto = await TraerAsync(id);
    if (producto == null)
    {
        Console.WriteLine("Producto no encontrado.");
        return;
    }
    Console.WriteLine($"Producto: {producto.Nombre}");
    Console.WriteLine($"Stock actual: {producto.Stock}");
    int stockQuitar = ValidarEntrada("Ingrese la cantidad a quitar: ", s => int.TryParse(s, out var v) && v > 0 ? (true, v) : (false, 0));
    var stockRestado = new Producto
    {
        Id = producto.Id,
        Stock = producto.Stock - stockQuitar
    };
    var quitarStock = await ActualizarAsync(stockRestado);
    if (quitarStock != null)
    {
        Console.WriteLine($"Producto actualizado: {quitarStock.Nombre}");
        Console.WriteLine($"Nuevo stock: {quitarStock.Stock}");
    }
    else
    {
        Console.WriteLine("Error al quitar el stock.");
    }
}

async void Menu()
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("1. Listar productos");
        Console.WriteLine("2. Listar productos a renovar");
        Console.WriteLine("3. Cargar stock");
        Console.WriteLine("4. Quitar stock");
        Console.WriteLine("5. Salir");
        Console.Write("Ingrese una opción: ");
        var op = Console.ReadLine();
        if (op == "5") break;
        switch (op)
        {
            case "1":
                await ListarProductosAsync();
                break;
            case "2":
                await ListarProductosRenovarAsync();
                break;
            case "3":
                await CargarStockAsync();
                break;
            case "4":
                await QuitarStockAsync();
                break;
            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }
}

class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

static T ValidarEntrada<T>(string mensaje, Func<string, (bool, T)> validador)
{
    T valor;
    bool valido;
    do
    {
        Console.Clear();
        Console.Write(mensaje);
        var entrada = Console.ReadLine() ?? "";
        (valido, valor) = validador(entrada);
        if (!valido)
        {
            Console.WriteLine("Valor inválido. Intente nuevamente.");
        }
    } while (!valido);
    return valor;
}

Menu();
