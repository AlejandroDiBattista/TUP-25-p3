using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

// Nota: crudcrud.com rechaza los payloads cuyos keys comienzan con mayúscula,
// por eso serializamos con camelCase en todas las peticiones JSON.

public static class CrudContactos {
    private const string ApiKey = "4d458ffac5c64bfc927a43e911628f26";
    private const string recurso = "contactos2025";
    private static readonly string baseUrl = $"https://crudcrud.com/api/{ApiKey}/{recurso}";
    private static readonly HttpClient http = new();
    private static readonly JsonSerializerOptions jsonOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

    private static StringContent ToJson<T>(T objeto) {
        var options = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,          // claves en camelCase
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(objeto, options);
        return new StringContent(json, new UTF8Encoding(false), "application/json");
    }

    private static T FromJson<T>(string json) {
        return JsonSerializer.Deserialize<T>(json, jsonOptions);
    }

    public static async Task<ContactoResponse> Create(ContactoRequest contacto) {
        WriteLine($"POST URL: {baseUrl}");
        WriteLine($"POST Body: {JsonSerializer.Serialize(contacto, jsonOptions)}");
        var response = await http.PostAsync($"{baseUrl}", ToJson(contacto));
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            WriteLine($"⚠️  Error {response.StatusCode}: {error}");
        }
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return FromJson<ContactoResponse>(responseJson);
    }

    public static async Task<List<ContactoResponse>> GetAll() {
        var response = await http.GetAsync($"{baseUrl}");
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            WriteLine($"⚠️  Error {response.StatusCode}: {error}");
        }
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return FromJson<List<ContactoResponse>>(responseJson);
    }

    public static async Task<ContactoResponse> Get(string id) {
        var response = await http.GetAsync($"{baseUrl}/{id}");
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            WriteLine($"⚠️  Error {response.StatusCode}: {error}");
        }
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return FromJson<ContactoResponse>(responseJson);
    }

    public static async Task<ContactoResponse> Update(string id, ContactoRequest contacto) {
        var response = await http.PutAsync($"{baseUrl}/{id}", ToJson(contacto));
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            WriteLine($"⚠️  Error {response.StatusCode}: {error}");
        }
        response.EnsureSuccessStatusCode();
        // crudcrud.com no retorna el objeto actualizado, solo status 200
        // Por lo tanto, devolvemos un ContactoResponse con los datos enviados y el id
        return new ContactoResponse {
            Id = id,
            Nombre = contacto.Nombre,
            Email = contacto.Email,
            Telefono = contacto.Telefono
        };
    }

    public static async Task Delete(string id) {
        var response = await http.DeleteAsync($"{baseUrl}/{id}");
        if (!response.IsSuccessStatusCode) {
            var error = await response.Content.ReadAsStringAsync();
            WriteLine($"⚠️  Error {response.StatusCode}: {error}");
        }
        response.EnsureSuccessStatusCode();
    }
}

public class ContactoRequest {
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
}

public class ContactoResponse : ContactoRequest {
    [JsonPropertyName("_id")]
    public string Id { get; set; }
}

// Ejemplo de uso directo para dotnet script
// Crear un nuevo contacto
var nuevoContacto = new ContactoRequest {
    Nombre   = "Juan Perez",
    Email    = "juan.perez@email.com",
    Telefono = "123456789"
};

// Mostrar el JSON que se enviará para depuración
WriteLine("JSON enviado al crear contacto:");
WriteLine(System.Text.Json.JsonSerializer.Serialize(nuevoContacto, new JsonSerializerOptions { WriteIndented = true }));

// Validar que los datos no sean nulos o vacíos antes de enviar
if (!string.IsNullOrWhiteSpace(nuevoContacto.Nombre) && !string.IsNullOrWhiteSpace(nuevoContacto.Email) && !string.IsNullOrWhiteSpace(nuevoContacto.Telefono)) {
    try {
        var creado = await CrudContactos.Create(nuevoContacto);
        WriteLine($"Contacto creado: {creado.Nombre} - {creado.Id}");
    } catch (HttpRequestException ex) {
        WriteLine($"Error al crear el contacto: {ex.Message}");
    }
} else {
    WriteLine("Datos de contacto inválidos. No se puede crear el contacto.");
}

// Obtener todos los contactos
List<ContactoResponse> contactos = null;
try {
    contactos = await CrudContactos.GetAll();
    WriteLine("Lista de contactos:");
    foreach (var c in contactos) {
        WriteLine($"{c.Id}: {c.Nombre} - {c.Email} - {c.Telefono}");
    }
} catch (HttpRequestException ex) {
    WriteLine($"Error al obtener contactos: {ex.Message}");
}

// Actualizar un contacto
if (contactos != null && contactos.Any()) {
    var primerContacto = contactos.First();
    var contactoActualizado = new ContactoRequest {
        Nombre = primerContacto.Nombre,
        Email = primerContacto.Email,
        Telefono = "987654321"
    };
    try {
        var actualizado = await CrudContactos.Update(primerContacto.Id, contactoActualizado);
        WriteLine($"Contacto actualizado: {actualizado.Nombre} - {actualizado.Telefono}");
    } catch (HttpRequestException ex) {
        WriteLine($"Error al actualizar el contacto: {ex.Message}");
    }
}

// Eliminar un contacto
if (contactos != null && contactos.Any()) {
    var primerContacto = contactos.First();
    try {
        await CrudContactos.Delete(primerContacto.Id);
        WriteLine($"Contacto eliminado: {primerContacto.Id}");
    } catch (HttpRequestException ex) {
        WriteLine($"Error al eliminar el contacto: {ex.Message}");
    }
}
