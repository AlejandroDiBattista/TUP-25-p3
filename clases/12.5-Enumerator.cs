using System.Collections;
using System.Collections.Generic;
using System.Linq; // Necesario para usar Sum, Min, etc.

class Contacto {
    public string Nombre { get; }
    public int Edad { get; }

    public Contacto(string nombre, int edad) {
        Nombre = nombre;
        Edad = edad;
    }

    public override string ToString() => $"{Nombre, -10} ({Edad})";
}

class Agenda : IEnumerable<Contacto> {
    private List<Contacto> contactos = new List<Contacto>();

    public Agenda() { }
   
    public void Agregar(Contacto contacto) => contactos.Add(contacto);

    public IEnumerator<Contacto> GetEnumerator() => contactos.GetEnumerator();
    
    public IEnumerator<Contacto> GetEnumerator() {
        foreach (var contacto in contactos.Reverse()) {
            yield return contacto;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); // Necesario para IEnumerable
}

var agenda = new Agenda();

agenda.Agregar(new Contacto("Juan", 20));
agenda.Agregar(new Contacto("Pedro", 30));
agenda.Agregar(new Contacto("Ana", 35));

foreach(var contacto in agenda) {
    WriteLine(contacto);
}

var suma = agenda.Sum(c => c.Edad);
WriteLine($"Suma de edades: {suma}"); // Suma de edades: 85

var min = agenda.Min(c => c.Edad);
WriteLine($"Edad mínima: {min}"); // Edad mínima: 20
WriteLine($"Edad máxima: {agenda.Max(c => c.Edad)}"); // Edad máxima: 35
WriteLine($"Promedio de edades: {agenda.Average(c => c.Edad)}"); // Promedio de edades: 28.333333333333332
WriteLine($"Cantidad de contactos: {agenda.Count()}"); // Cantidad de contactos: 3
WriteLine($"Cantidad de contactos mayores de 25: {agenda.Count(c => c.Edad > 25)}"); // Cantidad de contactos mayores de 25: 2
WriteLine($"Cantidad de contactos menores de 25: {agenda.Count(c => c.Edad < 25)}"); // Cantidad de contactos menores de 25: 1
WriteLine($"Cantidad de contactos con nombre Juan: {agenda.Count(c => c.Nombre == "Juan")}"); // Cantidad de contactos con nombre Juan: 1

var numeros = new List<int>(){1, 2, 3, 4, 5};
var sumaNumeros = agenda.Sum( persona => persona.Edad);         // Func
var countNumero = agenda.Count( persona => persona.Edad > 2);   // Predicate
var buscar = agenda.Find( persona => persona.Nombre == "Juan");
agenda.foreach( persona => WriteLine(persona) );                // Action

