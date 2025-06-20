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
        /// Llama al endpoint "api/datos" del backend y devuelve una respuesta tipada.
        /// </summary>
        /// <returns>Una instancia de <see cref="DatosRespuesta"/> o null si ocurre un error.</returns>
        public async Task<DatosRespuesta?> ObtenerDatosAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DatosRespuesta>("api/datos");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[ApiService] Error HTTP: {ex.Message}");
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine($"[ApiService] Tipo de contenido no soportado: {ex.Message}");
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"[ApiService] Error al deserializar JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ApiService] Error inesperado: {ex.Message}");
            }

            return null;
        }
    }
}
// Este código define un servicio que se encarga de realizar peticiones HTTP al backend
// y manejar las respuestas. Utiliza HttpClient para enviar solicitudes y recibir respuestas.