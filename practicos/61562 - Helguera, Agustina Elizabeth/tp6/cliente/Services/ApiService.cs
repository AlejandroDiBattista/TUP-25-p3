using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using cliente.Models;

namespace cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient http;

        public ApiService(HttpClient http)
        {
            this.http = http;
        }


        public async Task<DatosRespuesta> ObtenerDatosAsync()
        {
            return await http.GetFromJsonAsync<DatosRespuesta>("http://localhost:5184/api/datos");
        }


        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await http.GetFromJsonAsync<List<Producto>>("http://localhost:5184/api/productos");
        }
        
       public async Task<bool> RegistrarCompraAsync(Compra compra)
{
    var response = await http.PostAsJsonAsync("http://localhost:5184/api/compras", compra);
    return response.IsSuccessStatusCode;
}



       
    }
}
