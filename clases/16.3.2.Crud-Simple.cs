using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

public record ContactoRequest(string Nombre, string Apellido, string Telefono);
public record ContactoResponse(string _id, string Nombre, string Apellido, string Telefono);

const string ApiKey = "4d458ffac5c64bfc927a43e911628f26";

// crudcrud.com es un servicio gratuito para crear APIs REST
// simula una API CRUD (Create, Read, Update, Delete) 
// y permite crear recursos temporales para pruebas
// En este caso, creamos un recurso "contactos"
// y usamos la API para crear, listar y obtener contactos
// La URL base es: https://crudcrud.com/api/{ApiKey}/contactos

var baseUrl = $"https://crudcrud.com/api/{ApiKey}/contactos1";

// Creamos un cliente HTTP para enviar peticiones a la API
var http = new HttpClient();

// Configuramos las opciones de serialización JSON 
var jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, WriteIndented = true };

async Task Probar() {
    // Crear contactos
    // Creamos contactos de ejemplo para probar la API
    WriteLine("=== Probando API Rest ===");

    WriteLine("1. Creando contacto Juan... POST /contacto");
    string idJuan   = await CrearContacto("Juan", "Perez", "555-1234");
    
    WriteLine("2. Creando contacto Maria... POST /contacto");
    string idMaria  = await CrearContacto("Maria", "Gomez", "555-5678");
    
    WriteLine("3. Creando contacto Carlos... POST /contacto");
    string idCarlos = await CrearContacto("Carlos", "Lopez", "555-9012");

    WriteLine($"""
    === IDs creados ===
        Juan: {idJuan}
        Maria: {idMaria}
        Carlos: {idCarlos}
    """);

    // Listar todos
    WriteLine("\n4. Listamos los contactos GET /contacto");
    // Trae todos los contactos
    // Si no hay contactos, devuelve una lista vacía
    // Si hay contactos, devuelve la lista de contactos
    var todos = await ListarContactos();
    foreach (var c in todos) {
        WriteLine($"- {c._id}: {c.Nombre,-10} {c.Apellido, 10} ({c.Telefono})");
    }

    // Solo intentar obtener si tenemos un ID
    
    WriteLine($"\n5. Traemos un contacto guardado GET /contacto/:id");
    // Trae especificamente el contacto de Carlos
    // Si no existe, devuelve null
    // Si existe, devuelve el contacto
    var carlos = await ObtenerContactoPorId(idCarlos);
    if (carlos is not null) {
        WriteLine($"- {carlos._id}: {carlos.Nombre,-10} {carlos.Apellido, 10} ({carlos.Telefono})");
    } else {
        WriteLine("Contacto no encontrado");
    }

    WriteLine($"\n6. Borramosun contacto Carlos (ID: {idCarlos}) DELETE /contacto/:id");
    await Borrar(idCarlos);

    WriteLine($"\7 Listamos los contactos sin el borrado (GET /contactos/:id )");
    todos = await ListarContactos();
    foreach (var c in todos) {
        WriteLine($"- {c._id}: {c.Nombre,-10} {c.Apellido, 10} ({c.Telefono})");
    }
    
}

async Task<string> CrearContacto(string nombre, string apellido, string telefono) {
    // Creamos el contacto POST /contacto
    
    // Usamos ContactoRequest para no enviar el campo _id
    // Preparamos los datos para enviar.
    var nuevo = new ContactoRequest(nombre, apellido, telefono);                // 1. Creamos un nuevo contacto
    string json = JsonSerializer.Serialize(nuevo, jsonOptions);                 // 2. Lo convertimos a JSON
    var contenido = new StringContent(json, Encoding.UTF8, "application/json"); // 3. Lo convertimos a StringContent
    // contenido es lo que vamos a enviar mediante POST

    WriteLine($"Enviando POST a {baseUrl}");
    var respuesta = await http.PostAsync(baseUrl, contenido);                           // 1. Enviamos el POST (y esperamos la respuesta)
    string cuerpo = await respuesta.Content.ReadAsStringAsync();                        // 2. Traemos el cuerpo de la respuesta
    var creado    = JsonSerializer.Deserialize<ContactoResponse>(cuerpo, jsonOptions);  // 3. Extraemos el contacto creado

    WriteLine($"✅ Contacto creado: {creado._id}\n");
    return creado?._id ?? string.Empty;
}

async Task<List<ContactoResponse>> ListarContactos() {
    
    var respuesta = await http.GetAsync(baseUrl);               // 1. Solicitamos la lista de contactos GET /contactos
    string json = await respuesta.Content.ReadAsStringAsync();  // 2. Leemos el contenido de la respuesta
    
    var lista = JsonSerializer.Deserialize<List<ContactoResponse>>(json, jsonOptions); // 3. Deserializamos el JSON a una lista de ContactoResponse

    return lista ?? new List<ContactoResponse>();
}

async Task<ContactoResponse> ObtenerContactoPorId(string id) {
    // Solicitamos el contacto GET /contactos/{id}
    var respuesta = await http.GetAsync($"{baseUrl}/{id}");                 //1. Solicitamos el contacto
    string json = await respuesta.Content.ReadAsStringAsync();              //2. Traemos el contacto enviado
    return JsonSerializer.Deserialize<ContactoResponse>(json, jsonOptions); //3. Convertimos a una clase
}

async Task Borrar(string id){
    var respuesta = await http.DeleteAsync($"{baseUrl}/{id}");  // 1. Solictamos borrar un contacto
    WriteLine($"✅ Contacto borrado: {id}");
}

Clear();
WriteLine("Ejecutando CRUD de contactos...");
await Probar();
