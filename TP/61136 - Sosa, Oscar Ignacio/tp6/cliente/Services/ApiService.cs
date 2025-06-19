using System.Net.Http;
using System.Net.Http.Json;
using Cliente.Models2;

namespace Cliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DatosRespuesta?> ObtenerDatosAsync()
        {
            return await _httpClient.GetFromJsonAsync<DatosRespuesta>("/api/datos");
        }
    }
}
