// ATENCION -> Correr con `dotnet script 18.1-minimal-api.csx`
// Instalar el SDK de .NET 6.0 o superior
#r "sdk:Microsoft.NET.Sdk.Web"

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

// Construye un objeto de aplicación web (WebApplication) usando los argumentos de línea de comandos.
var builder = WebApplication.CreateBuilder();
var app = builder.Build();

// La ruta esta definida por el metodos HTTP y la URL
// En este caso la ruta es la misma para todos los metodos HTTP
app.MapGet("probar/",    () => "GET desde /probar");
app.MapPost("/probar",   () => "POST desde /probar");
app.MapPut("/probar",    () => "PUT desde /probar");
app.MapDelete("/probar", () => "DELETE desde /probar");


// Se le puede pasar parametros en el query string
// (la variables se reciben como argumentos)
// GET /buscar?q=texto
app.MapGet("/buscar", (string q) =>
    $"El texto a buscar es '{q}' (Usando query string)"
);

// O por route parameters
// Las variable se recibe como argumento pero tambien debe definirse en la ruta
// GET /buscar/texto
app.MapGet("/buscar/{texto}", (string texto) =>
    $"El texto a buscar es '{texto}' (Usando route parameter)"
);

// O combinando ambos
// GET /traer/texto?page=2
app.MapGet("/traer/{id}", (string id, int pagina) =>
    $"""
    El id a buscar es '{id}' (Usando route parameter) 
    de la pagina {pagina} (usando query string)
    """
);


// Un ejemplo mas complejo (Especificidad de las rutas)
// La ruta es estatica
app.MapGet("/noticias", () =>
    "Lista de noticias"
);

// La ruta es dinamica (noticias es estatica {seccion} es dinamica)
app.MapGet("/noticias/{seccion}", (string seccion) =>
    $"Noticias de la sección {seccion}"
);

// La ruta es dinamica (noticias es estatica {año} es dinamica {mes} es dinamica {dia} es dinamica)
app.MapGet("/noticias/{año}/{mes}/{dia}", (int año, int mes, int dia, int pagina) => 
    $"La fecha es {new DateTime(año, mes, dia):dd/MM/yyyy} en la pagina {pagina}"
);

// Para recibir datos complejos se puede usar una clase como paramertros.
// En esta caso asume que recibe un json en el cuerpo de la peticion y lo carga automaticamente
//
// POST /persona
// Content-Type: application/json
// 
// { "nombre": "Juan","edad": 30 }
// 
app.MapPost("/persona", (Persona persona) => 
    $"La persona es {persona.Nombre} y tiene {persona.Edad} años"
);


// Recibir servicios
// Cuando se pone como parametro una clase que ha sido
// registrada como un servicio, la misma es automaticamente inicializa
// y pasada como parametro.
// HttpRequest -> Toda la informacion del pedido realizado por el cliente
app.MapGet("/solicitud", (HttpRequest req) => {
    return $"""
        Metodo : {req.Method}
        Camino : {req.Path}
        Heads  : {req.Headers["Content-Type"]}
        Query  : {req.Query["q"]}
    """;
});


// HttpResponse -> Controla la respuesta al cliente
// En este caso implementa manualmente esta funcion ->`Results.Ok(new {nombre="Juan", edad=30})`
app.MapGet("/respuesta", (HttpResponse res) => {
    res.StatusCode = 200;
    res.ContentType = "application/json";
    return res.WriteAsync($$"""
        { 
            "nombre" : "juan",
            "edad" : 30
        }
    """);
});

// HttpContext -> Toda la información (Especialmente util en middlerware)
app.MapGet("/contexto", (HttpContext ctx) => {
    var metodo  = ctx.Request.Method;
    var ruta    = ctx.Request.Path;
    var headers = ctx.Request.Headers.Select(h => $"{h.Key}: {h.Value}").ToList();
    var usuario = ctx.User?.Identity?.IsAuthenticated == true ? ctx.User.Identity.Name : "Anónimo";
    var ip      = ctx.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
    var query   = ctx.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

    return Results.Ok(new {
        Metodo = metodo,
        Ruta = ruta,
        Headers = headers,
        Usuario = usuario,
        IP = ip,
        Query = query
    });
});

// Si devuelvo un objeto lo convierte a json automaticamente
// (Y lo señala como json)
app.MapGet("/persona/generica", () => 
    new Persona("Juan Perez", 30)
);


// Devolver un json de forma automatica (Cuando se devuelve un objeto convierte a json automaticamente)
app.MapGet("/persona", () => {
    var id = Guid.NewGuid();
    var persona = new {
        Id = id,
        Nombre = "Juan",
        Edad = 30
    };
    return Results.Accepted($"/persona/{id}", persona);
});


// Retornando un json usando una clase anonima
app.MapGet("/persona/{nombre}/{apellido}/{telefono}", (string nombre, string apellido, string telefono) =>
    Results.Ok(new {nombre, apellido, telefono})
);


app.Run();


record Persona(string Nombre, int Edad);