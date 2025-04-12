class Alumno : IComparable<Alumno> {
    public int Legajo { get; set; } // Identidad del alumno
    public string Nombre { get; set; }

    public Alumno(string nombre, int legajo) {
        Nombre = nombre;
        Legajo = legajo;
    }

    public override string ToString() {
        return $"{Nombre,-10} ({Legajo})";
    }

    public override bool Equals(object obj) {
        if (obj is Alumno otro) {
            return Legajo == otro.Legajo;
        }
        return false;
    }

    public override int GetHashCode() {
        return Legajo.GetHashCode();
    }

    // Implementación del método CompareTo para la interfaz IComparable
    // public int CompareTo(Alumno otro) {
    //     return Legajo.CompareTo(otro.Legajo);
    // }

    public int CompareTo(Alumno otro) {
        return Nombre.CompareTo(otro.Nombre);
    }
}

var l1 = new Alumno("Juan",  456);
var l2 = new Alumno("Pedro", 123);
var l3 = new Alumno("Ana",   789);
var l4 = new Alumno("Ana",   567);

if(l1 is IComparable<Alumno> ) {
    WriteLine($"Legajo de {l1.Nombre} es menor que el de {l2.Nombre}");
} else {
    WriteLine($"{l1.Nombre} no implementa IComparable");
}
List<Alumno> lista = new List<Alumno>();
lista.Add(l1);
lista.Add(l2);
lista.Add(l3);
lista.Add(l4);
WriteLine("\n> Lista sin ordenar:");

if(lista.Contains( new Alumno("xxx", 123) )) {
    WriteLine("La lista contiene el legajo 123");
} else {
    WriteLine("La lista no contiene el legajo 123");
}

foreach (var a in lista) {
    WriteLine($"- {a}");
}

lista.Sort(); // ❌ ERROR No puede ordenar porque no sabe como comparar

lista.Sort( (a, b) => a.Legajo.CompareTo(b.Legajo) ); 
lista.Sort( (a, b) => a.Nombre.CompareTo(b.Nombre) );
lista.Sort( (a, b) => a.Nombre.Lenght.CompareTo(b.Nombre.Lenght) );

WriteLine("\n> Lista ordenada por legajo:");
foreach (var a in lista) {
    WriteLine($"- {a}");
}

