using System.Net.Http.Json;
public class ContactoServicio: IContactoServicio {
    private HttpClient Http { get; set; } // Corrected typo: HttpClient

    public ContactoServicio(HttpClient http) {
        Http = http;
    }

    public async Task<List<Contacto>> TraerTodosAsync() {
        try {
            var result = await Http.GetFromJsonAsync<List<Contacto>>("contactos");
            return result ?? new List<Contacto>();
        } catch (Exception ex) {
            Console.WriteLine($"Error al cargar contactos: {ex.Message}");
            return new List<Contacto>();
        }
    }

    public async Task<Contacto> TraerPorIdAsync(int id) {
        return await Http.GetFromJsonAsync<Contacto>($"contactos/{id}");
    }

    public async Task<Contacto> CrearAsync(Contacto contacto) {
        try {
            var response = await Http.PostAsJsonAsync("contactos", contacto);
            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadFromJsonAsync<Contacto>();
            }
            Console.WriteLine($"Error al crear contacto: {response.StatusCode}");
            return null;
        } catch (Exception ex) {
            Console.WriteLine($"Error al crear contacto: {ex.Message}");
            return null;
        }
    }

    public async Task<Contacto> ActualizarAsync(Contacto contacto) {
        try {
            var response = await Http.PutAsJsonAsync($"contactos/{contacto.Id}", contacto);
            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadFromJsonAsync<Contacto>();
            }
            Console.WriteLine($"Error al actualizar contacto: {response.StatusCode}");
            return null;
        } catch (Exception ex) {
            Console.WriteLine($"Error al actualizar contacto: {ex.Message}");
            return null;
        }
    }
    
    public async Task<bool> EliminarAsync(int id) {
        try {
            var response = await Http.DeleteAsync($"contactos/{id}");
            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Console.WriteLine($"Error al eliminar contacto: {ex.Message}");
            return false;
        }
    }
}