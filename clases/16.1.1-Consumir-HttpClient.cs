using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// Como usar un web API en C#

// El sitio de OpenWeatherMap ofrece una API para obtener información del clima.
// Debes registrarte y obtener una API Key gratuita.

// La API Key es un código único que te identifica como usuario de la API.

// En este caso, la API Key es "30d38b26954359266708f92e1317dac0".
// Puedes usarla para hacer peticiones a la API y obtener datos del clima.

// La API devuelve los datos en formato JSON, que es un formato de intercambio de datos muy utilizado.
// Puedes usar la clase JsonDocument para parsear el JSON y acceder a los datos que necesitas.

// En este ejemplo, se obtiene la temperatura y la descripción del clima de una ciudad.
// La ciudad se ingresa por consola y se usa la API Key para hacer la petición.
// La respuesta de la API se parsea y se muestra en consola.

// El siguiente código es un ejemplo de cómo usar la API de OpenWeatherMap para obtener el clima actual de una ciudad.

// El método ObtenerClima() se encarga de hacer la petición a la API y mostrar los datos en consola.
// Se usa la clase HttpClient para hacer la petición y la clase JsonDocument para parsear el JSON.


static async Task ObtenerClima() {
    // Leemos la ciudad ingresada por el usuario
    Write("Ingrese una ciudad: ");
    string ciudad = ReadLine();

    // Armamos la URL de la API con la ciudad y la API Key
    string apiKey = "30d38b26954359266708f92e1317dac0";
    string url = $"https://api.openweathermap.org/data/2.5/weather?q={ciudad}&appid={apiKey}&units=metric&lang=es";

    WriteLine($"\nURL de la API:\n{url}\n");

    // Creamos un cliente HTTP para hacer la petición
    using var http = new HttpClient();

    // Hacemos la petición a la API y leemos la respuesta
    // La respuesta es un string en formato JSON
    var response = await http.GetStringAsync(url);
    WriteLine($"Respuesta:\n{response}");

    // La clase JsonDocument se usa para parsear el JSON y acceder a los datos
    // En este caso, se obtiene la temperatura y la descripción del clima

    var data = JsonDocument.Parse(response);

    // Se usa GetProperty() para acceder a las propiedades del JSON
    var temp   = data.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
    var clima  = data.RootElement.GetProperty("weather")[0].GetProperty("description").GetString();
    var viento = data.RootElement.GetProperty("wind").GetProperty("speed").GetDouble();

    // Se muestra la información en consola
    WriteLine($"""
    
    === Clima Actual en {ciudad.ToUpper()} ===
    
     Temperatura: {temp} °C
     Condición  : {clima}
     Viento     : {viento} m/s

    """);

}

await ObtenerClima();