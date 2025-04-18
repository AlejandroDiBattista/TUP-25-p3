using System;
using System.Collections;
using System.Collections.Generic;

class Agenda : IEnumerable<Persona> {

    private List<Persona> contactos;

    public Agenda(){
        contactos = new List<Persona>();
    }

    public void Add(Persona contacto){
        contactos.Add(contacto);
        Ordenar();
    }

    public void Show(){
        foreach (var contacto in contactos){
            WriteLine(contacto);
        }
    }

    public void Ordenar(){
        contactos.Sort();
    }

    public IEnumerator<Persona> GetEnumerator(){
        return contactos.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator(){
        return GetEnumerator();
    }
}

class Persona : IComparable<Persona> {
    public string Nombre;
    public string Telefono;
    public int Edad;

    public Persona(string nombre, string telefono, int edad = 0){
        Nombre = nombre;
        Telefono = telefono;
        Edad = edad;
    }

    public override string ToString(){
        return $"{ Nombre,-10} {Telefono} ({Edad})";
    }

    public int CompareTo(Persona otra){
        return Nombre.CompareTo(otra.Nombre);
    }

    public override bool Equals(object obj){
        if (obj is Persona otra){
            return Nombre == otra.Nombre && Telefono == otra.Telefono;
        }
        return false;
    }

    public override int GetHashCode(){
        return HashCode.Combine(Nombre, Telefono);
    }

}


var agenda = new Agenda();
agenda.Add(new Persona("Juan",  "123456789", 20));
agenda.Add(new Persona("Ana",   "987654321", 30));
agenda.Add(new Persona("Pedro", "456789123", 20));
agenda.Add(new Persona("Ana",   "987654321", 15)); // Duplicado

WriteLine("=== Agenda ordenada ===");
agenda.Show();

WriteLine("=== Iterando sobre la agenda ===");
foreach (var contacto in agenda){
    Console.WriteLine($"Iterando: {contacto}");
}

WriteLine("=== Filtrado por edad ===");
foreach(var c in agenda.Where(x => x.Edad > 20)){
    Console.WriteLine($"Filtrado: {c}");
}

WriteLine("=== Seleccionando edades ===");
foreach(var n in agenda.Select(x => x.Edad)){
    WriteLine($"Seleccionado: {n}");
}

var min = agenda.Select(x => x.Edad).Min();  //map -> transormar -> SQL donde SELECT te dice que campo va a tomar de la tabla
var max = agenda.Select(x => x.Edad).Max();
var promedio = agenda.Select(x => x.Edad).Average();

var mayores = agenda.Where(x => x.Edad >= 18);

WriteLine($"=== Estadísticas de edad ===");
WriteLine($"Edad mínima: {min}");
WriteLine($"Edad máxima: {max}");
WriteLine($"Edad promedio: {promedio}");
WriteLine($"Cantidad de Mayores: {mayores.Count()}");

// Select -> Dada una enumeracion dame una nueva transformada (selecion que usar)
// Where -> Dada una enumeracion dame una nueva con todos los elemento que complan la condicion
// OrderBy -> Dada una enumeracion me devuelve otra con lo elementos ordedanos segun el criterio indicado

WriteLine("=== Agenda ordenada por Telefono ===");
foreach(var a in agenda.OrderBy(x => x.Telefono)){
    WriteLine(a);
}

WriteLine("=== Contactos hasta encontrar un menor ===");
foreach(var a in agenda.TakeWhile(x => x.Edad >= 18)){
    WriteLine(a);
}

var edadMinimaDeMayores = agenda
                            .Select(x => x.Edad)            // Map
                            .Where(x => x >= 20)       // Filter
                            .Min();                         // Min

// Equivalente a SQL: SELECT Edad FROM Agenda WHERE Edad >= ORDER BY Telefono;

WriteLine("=== Lista agruados ===");
foreach(var e in agenda.GroupBy(x => x.Edad).ToList()){
    foreach( var (x,y) in e){
        WriteLine(x,y);

    }
}