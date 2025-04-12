// En C#, es posible definir operadores. En el caso de la igualdad, el operador `==`
// se puede sobrecargar para que compare los valores de los objetos.

class Persona {
    public int Dni { get; set; }
    public string Nombre { get; set; }

    public Persona(int dni, string nombre) {
        Dni = dni;
        Nombre = nombre;
    }

    public override bool Equals(object obj) { // Sobrecarga del método Equals
        if (obj is Persona otra) {
            return this.Dni == otra.Dni;
        }
        return false;
    }

    public static bool operator ==(Persona p1, Persona p2) {
        if (p1 is null && p2 is null) return true;  // null == null
        if (p1 is null || p2 is null) return false; // null es diferente de cualquier valor
        return p1.Equals(p2);
    }

    public static bool operator !=(Persona p1, Persona p2) { // Debe definirse junto con el operador ==
        return !(p1 == p2);
    }
}

// Tres observaciones importantes:
// 1. Los operadores se definen como métodos estáticos y reciben dos parámetros.
// 2. Es necesario definir el operador `!=` para que funcione correctamente el operador `==`.
// 3. Es obligatorio definir el método `Equals` para que el operador `==` funcione correctamente.

var a = new Persona(12345678, "Juan");
var b = new Persona(12345678, "Juan Carlos");

WriteLine(a.Equals(b)); // true
WriteLine(a == b); // true
WriteLine(a != b); // false

// Esto funciona, pero genera una advertencia (`warning`) indicando que falta implementar `GetHashCode`.
// Esto es un requisito técnico en C# para garantizar la consistencia en estructuras como diccionarios y conjuntos.
// Un `hash code` es un número entero que representa el objeto y se utiliza para optimizar la búsqueda de objetos en colecciones.

// El `hash code` se calcula a partir de los valores de los atributos del objeto.
/*
    public override int GetHashCode() {
        return Dni.GetHashCode();
    }
*/

// En este caso, el `hash code` se calcula a partir del DNI, pero se puede calcular a partir de otros atributos.

// Por ejemplo, si tenemos una clase `Contacto` con `Nombre` y `Apellido`, podemos calcular el `hash code` de la siguiente manera:
// tambien tiene Edad y Telefono pero estos campos no constituyen a la identidad de contacto, son atributos adicionales.

class Contacto {
    public string Nombre   { get; set; }    // El contacto se identifica por su nombre y apellido
    public string Apellido { get; set; }

    public int Edad { get; set; }           // Edad y telefono son atributos adicionales, no constituyen la identidad
    public string Telefono { get; set; }

    public Contacto(string nombre, string apellido, int edad, string telefono) {
        Nombre = nombre;
        Apellido = apellido;
        Edad = edad;
        Telefono = telefono;
    }

    public override bool Equals(object obj) {
        if (obj is Contacto otra) {
            return Nombre.Equals(otra.Nombre) && Apellido.Equals(otra.Apellido);
        }
        return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Nombre, Apellido);
    }

    public static bool operator ==(Contacto p1, Contacto p2) {
        if (p1 is null && p2 is null) return true;  // null == null
        if (p1 is null || p2 is null) return false; // null es diferente de cualquier valor
        return p1.Equals(p2);
    }

    public static bool operator !=(Contacto p1, Contacto p2) { // Debe definirse junto con el operador ==
        return !(p1 == p2);
    }
}

// Ahora podemos usar la clase `Contacto` y comparar objetos de esta clase.
var c1 = new Contacto("Juan", "Pérez", 30, "123456789");
var c2 = new Contacto("Juan", "Pérez", 25, "987654321");
var c3 = new Contacto("Juan", "Gómez", 40, "123456789");

WriteLine(c1 == c2); // true
WriteLine(c1 == c3); // false

// Ahora, si queremos buscar un contacto en una lista de contactos, podemos usar el método `Contains` de la lista.
List<Contacto> agenda = new List<Contacto>();
agenda.Add(c1);
agenda.Add(c2);
agenda.Add(c3);

if(agenda.Contains(new Contacto("Juan", "Pérez", 0, ""))) {
    WriteLine("El contacto ya existe en la agenda.");
} else {
    WriteLine("El contacto no existe en la agenda.");
}
// En este caso, la búsqueda de un contacto en la agenda se basa en el nombre y apellido, ignorando la edad y el teléfono.  

// Bono extra: IEquatable<T>
// La interfaz `IEquatable<T>` permite definir la comparación de igualdad de una manera 
// más eficiente y segura.
// Al implementar esta interfaz, se puede evitar la necesidad de hacer un `cast` al tipo correcto

class Punto : IEquatable<Punto> {
    public int X { get; set; }
    public int Y { get; set; }
    public Punto(int x, int y) {
        X = x;
        Y = y;
    }

    public bool Equals(Punto otro) { // Implementación de IEquatable<Punto>
        if (otro is null) return false;
        return X == otro.X && Y == otro.Y;
    }
    
    public override bool Equals(object obj) { // Reescritura del Equals de object
        if (obj is Punto otro) {
            return Equals(otro);
        }
        return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y); // Los int son su propio HashCode
    }
}