using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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

        /// <summary>
        /// Llama a la API REST para obtener datos.
        /// </summary>
        /// <returns>Objeto DatosRespuesta o null si falla.</returns>
        public async Task<DatosRespuesta?> ObtenerDatosAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DatosRespuesta>("api/datos");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[ApiService] Error HTTP: {ex.Message}");
                return null;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"[ApiService] Tipo de contenido no soportado: {ex.Message}");
                return null;
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"[ApiService] Error al deserializar JSON: {ex.Message}");
                return null;
            }
        }
    }
}

