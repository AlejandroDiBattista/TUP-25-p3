using System;

// Libreria para trabajar con JSON
using System.Text.Json;
using System.Collections.Generic;

// Ejemplo de serialización y deserialización de un objeto a JSON

// Cuando creamos una clase en c# hacer muy comodo el acceso a los datos. 
// pero cuando queremos guardar los datos en un archivo o ser enviado a través de una red
// es necesario convertirlo a otro formato, los objetos son un pedazo de memoria y no es posible
// guardarlos en un archivo o enviarlos a través de una red.

// Usa de las formas en convertirlos a un string y luego guardar o enviar dicho string.
// Luego se puede hacer la funcion inversa para volver a convertir el string a un objeto.

// La serializacion y deserializacion es el proceso de convertir un objeto a un string y viceversa.

// Un formato de intercambio muy usado y efeciente es JSON (JavaScript Object Notation), es la misma vez
// facil de leer y escribir para los humanos y para las máquinas.

// En C# podemos usar la clase JsonSerializer para serializar y deserializar objetos a JSON.


// Creamos una clase Contacto con propiedades Nombre, Apellido y una lista de teléfonos
public class Contacto {
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public List<Telefono> Telefonos { get; set; }

    public class Telefono {
        public string Numero { get; set; }
        public string Tipo { get; set; }
    }
}

// Inicializamos un objeto de la clase Contacto
// y le asignamos valores a sus propiedades

// Observación: para inicializar una lista de objetos
// podemos usar la sintaxis de inicializador de objetos
// que es la sintaxis de llaves {}

// The JSON equivalent of this object would be:

// C#                                               -> JSON
Contacto contacto = new() {                         // {
    Nombre = "Juan",                                //     "Nombre": "Juan",
    Apellido = "Perez",                             //     "Apellido": "Perez",
    Telefonos = new(){                              //     "Telefonos": [
        new Telefono() {                            //         {
            Numero = "(381) 123-4567",              //             "Numero": "(381) 123-4567",
            Tipo = "Casa"                           //             "Tipo": "Casa"
        },                                          //         },  
        new() {                                     //         {
            Numero = "(381) 765-4321",              //             "Numero": "(381) 765-4321",
            Tipo = "Trabajo"                        //             "Tipo": "Trabajo"     
        }                                           //         }
    }                                               //     ]     
};                                                  // }


// Usamos la clase JsonSerializer para serializar el objeto
Clear();
string jsonString = JsonSerializer.Serialize(contacto);
WriteLine("\n=== Objeto serializado a JSON ===");
WriteLine(jsonString);
WriteLine();

// Serializar con formato bonito (pretty print)
var options = new JsonSerializerOptions { WriteIndented = true };
string jsonBonito = JsonSerializer.Serialize(contacto, options);
WriteLine("\n=== JSON con formato bonito ===");
WriteLine(jsonBonito);


// Deserializar el JSON de vuelta a un objeto
// La deserialización convierte una cadena de texto en formato JSON de vuelta a un objeto
// Esto es útil para leer datos de un archivo o recibirlos a través de una red

// Usamos la clase JsonSerializer para deserializar el JSON
var nuevo = JsonSerializer.Deserialize<Contacto>(jsonString);
WriteLine("\n=== Objeto deserializado desde JSON ===");
WriteLine($" - Nombre  : {nuevo.Nombre}");
WriteLine($" - Apellido: {nuevo.Apellido}");
foreach (var telefono in nuevo.Telefonos) {
    WriteLine($"   - Teléfono: {telefono.Numero} ({telefono.Tipo})");
}

