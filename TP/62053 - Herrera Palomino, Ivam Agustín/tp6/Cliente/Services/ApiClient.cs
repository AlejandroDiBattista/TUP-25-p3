using System.Net;
using System.Net.Http.Json;
using TiendaOnline.Frontend.Models; 

namespace TiendaOnline.Frontend.Services;

public class ApiClient(HttpClient http)
{
    // Productos
    public Task<List<Producto>?> GetProductos(string? q = null) =>
        http.GetFromJsonAsync<List<Producto>>($"/productos?search={q}");

    // Carrito
    public async Task<Guid> CrearCarritoAsync()
    {
        var resp = await http.PostAsync("/carritos", null);
        var car  = await resp.Content.ReadFromJsonAsync<Carrito>();
        return car!.Id;
    }
    public async Task<Carrito?> GetCarrito(Guid id)
    {
        var response = await http.GetAsync($"/carritos/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Carrito>();
        }
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode(); 
        return null; 
    }

    public Task Add(Guid cid, int pid, int qty = 1) =>
        http.PutAsync($"/carritos/{cid}/{pid}?qty={qty}", null);

    public Task Remove(Guid cid, int pid, int qty = 1) =>
        http.DeleteAsync($"/carritos/{cid}/{pid}?qty={qty}");

    public Task Vaciar(Guid cid) =>
        http.DeleteAsync($"/carritos/{cid}");

    public async Task<Compra?> Confirmar(Guid cid, string nom, string ape, string mail)
    {
        var url = $"/carritos/{cid}/confirmar" +
                  $"?nombre={Uri.EscapeDataString(nom)}" +
                  $"&apellido={Uri.EscapeDataString(ape)}" +
                  $"&email={Uri.EscapeDataString(mail)}";

        var resp = await http.PutAsync(url, null);
        return await resp.Content.ReadFromJsonAsync<Compra>();
    }
}
