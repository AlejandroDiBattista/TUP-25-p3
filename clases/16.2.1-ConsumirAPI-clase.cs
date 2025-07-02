using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// Creo la estructura de datos para el clima
// con los campos que nos interesan para el clima
class ClimaInfo {
    public MainInfo Main { get; set; }
    public WeatherInfo[] Weather { get; set; }
    public WindInfo Wind { get; set; }

    public class MainInfo {
        public double Temp { get; set; }
    }

    public class WeatherInfo {
        public string Description { get; set; }
    }

    public class WindInfo {
        public double Speed { get; set; }
    }
}

// Tambien lo podemos hacer con records para ser mucho mas sucinto
// record ClimaInfo(MainInfo Main, WeatherInfo[] Weather, WindInfo Wind);
// record MainInfo(double Temp);
// record WeatherInfo(string Description);
// record WindInfo(double Speed);

#region Ejemplo de respuesta de la API (En JSON)
// {
//   "coord": {
//     "lon": -75.4557,
//     "lat": 43.2128
//   },
//   "weather": [
//     {
//       "id": 804,
//       "main": "Clouds",
//       "description": "nubes",
//       "icon": "04d"
//     }
//   ],
//   "base": "stations",
//   "main": {
//     "temp": 9.99,
//     "feels_like": 9.6,
//     "temp_min": 9.27,
//     "temp_max": 10.62,
//     "pressure": 1019,
//     "humidity": 91,
//     "sea_level": 1019,
//     "grnd_level": 996
//   },
//   "visibility": 10000,
//   "wind": {
//     "speed": 1.54,
//     "deg": 310
//   },
//   "clouds": {
//     "all": 100
//   },
//   "dt": 1746715662,
//   "sys": {
//     "type": 1,
//     "id": 5681,
//     "country": "US",
//     "sunrise": 1746697542,
//     "sunset": 1746749455
//   },
//   "timezone": -14400,
//   "id": 5134295,
//   "name": "Rome",
//   "cod": 200
// }
#endregion

#region Ejemplo de respuesta de la API (En C#)
var ejemploRespuesta = new {
    coord = new { lon = -75.4557, lat = 43.2128 },
    weather = new[] {
        new {
            id = 804,
            main = "Clouds",
            description = "nubes",
            icon = "04d"
        }
    },
    @base = "stations",
    main = new {
        temp = 9.99,
        feels_like = 9.6,
        temp_min = 9.27,
        temp_max = 10.62,
        pressure = 1019,
        humidity = 91,
        sea_level = 1019,
        grnd_level = 996
    },
    visibility = 10000,
    wind = new { speed = 1.54, deg = 310 },
    clouds = new { all = 100 },
    dt = 1746715662,
    sys = new {
        type = 1,
        id = 5681,
        country = "US",
        sunrise = 1746697542,
        sunset = 1746749455
    },
    timezone = -14400,
    id = 5134295,
    name = "Rome",
    cod = 200
};
#endregion

static async Task ObtenerClima() {
    Write("Ingrese una ciudad: ");
    string ciudad = ReadLine();
    string apiKey = "30d38b26954359266708f92e1317dac0";
    string url    = $"https://api.openweathermap.org/data/2.5/weather?q={ciudad}&appid={apiKey}&units=metric&lang=es";

    using var clienteHttp = new HttpClient();
    var respuesta = await clienteHttp.GetStringAsync(url);

    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var datos    = JsonSerializer.Deserialize<ClimaInfo>(respuesta, opciones);

    Clear();
    var infoClima = $"""
        === Clima Actual en {ciudad.ToUpper()} ===

         Temperatura: {datos.Main.Temp} °C
         Condición  : {datos.Weather[0].Description}
         Viento     : {datos.Wind.Speed} m/s

        """;
    WriteLine(infoClima);
}

await ObtenerClima();