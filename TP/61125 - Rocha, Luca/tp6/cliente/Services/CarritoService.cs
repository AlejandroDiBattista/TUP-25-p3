using Blazored.LocalStorage;
using System.Collections.Generic;

public class CarritoService
{
    private readonly ILocalStorageService localStorage;
    private const string Key = "carrito";

    public CarritoService(ILocalStorageService localStorageService)
    {
        localStorage = localStorageService;
    }

    public async Task GuardarCarrito(List<Producto> productos)
    {
        await localStorage.SetItemAsync(Key, productos);
    }

    public async Task<List<Producto>> CargarCarrito()
    {
        return await localStorage.GetItemAsync<List<Producto>>(Key) ?? new List<Producto>();
    }
}