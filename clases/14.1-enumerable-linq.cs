using System;
using System.Collections;
using System.Collections.Generic;
using static System.Console;

class Agenda : IEnumerable<Contacto> {
    public IEnumerator<Contacto> GetEnumerator() {
        foreach (var contacto in contactos) {
            yield return contacto;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<Contacto> OrdenadaPorTelefono() {
        contactos.Sort((x, y) => x.Telefono.CompareTo(y.Telefono));
        foreach (var contacto in contactos) {
            yield return contacto;
        }
    }

    public IEnumerable<Contacto> OrdenadaPorEdad() {
        contactos.Sort((x, y) => x.Edad.CompareTo(y.Edad));
        foreach (var contacto in contactos) {
            yield return contacto;
        }
    }

    private List<Contacto> contactos;

    public Agenda() {
        contactos = new List<Contacto>();
    }

    public void Add(Contacto contacto) {
        contactos.Add(contacto);
    }

    public void Show() {
        foreach (var contacto in contactos) {
            WriteLine($"{contacto}");
        }
    }

    public bool Contains(Contacto contacto) {
        return contactos.Contains(contacto);
    }

    public void Remove(Contacto contacto) {
        contactos.Remove(contacto);
    }
}

class Contacto {
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public int Edad { get; set; }

    public Contacto(string nombre, string telefono, int edad) {
        Nombre = nombre;
        Telefono = telefono;
        Edad = edad;
    }

    public override string ToString() {
        return $"{Nombre,-30}  {Telefono}  {Edad}";
    }

    public int CompareTo(Contacto otro) {
        return Nombre.CompareTo(otro.Nombre);
    }
}

var agenda = new Agenda(){ 
    new Contacto("Juan Perez",      "555-1234", 30),
    new Contacto("Ana Maria",       "555-4321", 25),
    new Contacto("Luis Garcia",     "555-6789", 40),
    new Contacto("Maria Sanchez",   "555-9876", 35),
    new Contacto("Carlos López",    "555-1111", 28),
    new Contacto("Sofía Martínez",  "555-2222", 32),
    new Contacto("Pedro Gómez",     "555-3333", 45),
    new Contacto("Lucía Fernández", "555-4444", 27),
    new Contacto("Diego Ramírez",   "555-5555", 50),
    new Contacto("Valentina Ruiz",  "555-6666", 22),
    new Contacto("Ricardo Torres",  "555-7777", 38),
    new Contacto("Isabel Vega",     "555-8888", 41),
    new Contacto("Andrés Castro",   "555-9999", 29),
    new Contacto("Camila Peña",     "555-0000", 34)
};

WriteLine("=== Agenda (Ordenada por telefono) ===");
foreach(var e in agenda.OrdenadaPorTelefono()){
    WriteLine(e);
}

WriteLine("\n=== Métodos comunes de LINQ agrupados por categorías ===\n");

WriteLine("=== Filtrado ===");
WriteLine("Where         - Filtra elementos basado en un predicado");
WriteLine("OfType        - Filtra elementos de un tipo específico");
WriteLine("Distinct      - Elimina duplicados");
WriteLine("DistinctBy    - Elimina duplicados basado en una clave");
WriteLine("Skip          - Omite un número específico de elementos");
WriteLine("SkipWhile     - Omite elementos mientras una condición es verdadera");
WriteLine("Take          - Toma un número específico de elementos");
WriteLine("TakeWhile     - Toma elementos mientras una condición es verdadera");

WriteLine("\n=== Ordenamiento ===");
WriteLine("OrderBy       - Ordena elementos en orden ascendente");
WriteLine("OrderByDescending - Ordena elementos en orden descendente");
WriteLine("ThenBy        - Ordenamiento secundario en orden ascendente");
WriteLine("ThenByDescending - Ordenamiento secundario en orden descendente");
WriteLine("Reverse       - Invierte el orden de los elementos");

WriteLine("\n=== Proyección ===");
WriteLine("Select        - Transforma cada elemento");
WriteLine("SelectMany    - Transforma y aplana colecciones");

WriteLine("\n=== Agregación ===");
WriteLine("Count/LongCount - Cuenta elementos");
WriteLine("Sum           - Suma valores");
WriteLine("Min           - Encuentra el valor mínimo");
WriteLine("Max           - Encuentra el valor máximo");
WriteLine("Average       - Calcula el promedio");
WriteLine("Aggregate     - Aplica una acumulación personalizada");

WriteLine("\n=== Cuantificadores ===");
WriteLine("Any           - Verifica si algún elemento cumple una condición");
WriteLine("All           - Verifica si todos los elementos cumplen una condición");
WriteLine("Contains      - Verifica si la colección contiene un elemento específico");

WriteLine("\n=== Conjuntos ===");
WriteLine("Concat        - Concatena dos secuencias");
WriteLine("Union         - Combina y elimina duplicados de dos secuencias");
WriteLine("Intersect     - Retorna elementos comunes");
WriteLine("Except        - Retorna elementos presentes en la primera pero no en la segunda");
WriteLine("Zip           - Combina elementos de dos secuencias");

WriteLine("\n=== Elementos ===");
WriteLine("First/FirstOrDefault - Retorna el primer elemento");
WriteLine("Last/LastOrDefault   - Retorna el último elemento");
WriteLine("Single/SingleOrDefault - Retorna un único elemento");
WriteLine("ElementAt/ElementAtOrDefault - Retorna el elemento en un índice específico");

WriteLine("\n=== Conversión ===");
WriteLine("ToArray       - Convierte a array");
WriteLine("ToList        - Convierte a List<T>");
WriteLine("ToDictionary  - Convierte a Dictionary<TKey, TValue>");
WriteLine("ToLookup      - Convierte a ILookup<TKey, TElement>");
WriteLine("AsEnumerable  - Trata la fuente como IEnumerable<T>");
WriteLine("Cast          - Convierte elementos al tipo especificado");

WriteLine("\n=== Generación ===");
WriteLine("Range         - Genera una secuencia de números enteros");
WriteLine("Repeat        - Genera una secuencia repitiendo un valor");
WriteLine("Empty         - Retorna una secuencia vacía");