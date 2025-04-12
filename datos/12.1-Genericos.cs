// Sopongamos que queremos hacer una libreria que nos permita crear una lista de cualquier tipo
// de datos, y que ademas nos permita ordenarlos y buscar elementos en ella.

// Veamos como seria una lista de enteros...
class ListInt {
    int[] Elementos = new int[10]; // 
    int Contador = 0;

    public ListInt(){}

    public void Agregar(int elemento){
        Elementos[Contador++] = elemento;
    }

    public bool Contiene(int elemento){
        for (var i = 0; i < Contador; i++){
            if (elemento.Equals(Elementos[i])) // Observar que usamos `Equals` en lugar de `==`
                return true;
        }
        return false;
    }
}

// Ahora la podemos usar 
WriteLine("\n> Lista de enteros `ListInt`");
var l = new ListInt();
l.Agregar(3);
l.Agregar(2);
if(l.Contiene(2)) 
    WriteLine("- El elemento 2 está en la lista");
else
    WriteLine("- El elemento 2 no está en la lista");


// Ahora queremos hacer lo mismo pero con un tipo de dato diferente, por ejemplo string
class ListString{
    string[] Elementos = new string[10]; // 
    int Contador = 0;

    public ListString(){}

    public void Agregar(string elemento){
        Elementos[Contador++] = elemento;
    }

    public bool Contiene(string elemento){
        for (var i = 0; i < Contador; i++){
            if (elemento.Equals(Elementos[i]))
                return true;
        }
        return false;
    }
}

// Ahora la podemos usar
WriteLine("\n> Lista de strings `ListString`");
var l2 = new ListString();
l2.Agregar("Juan");
l2.Agregar("Pedro");
if(l2.Contiene("Juan")) 
    WriteLine("El elemento Juan está en la lista");
else 
    WriteLine("El elemento Juan no está en la lista");

// Como vemos la estructura de ambas clases es la misma, solo cambia el tipo de dato que se almacena 
// en la lista. Esto es un problema, ya que si queremos hacer una lista de otro tipo de dato,
// tenemos que crear otra clase, y si queremos hacer una lista de un tipo de dato diferente,
// tenemos que crear otra clase, y asi sucesivamente.

// C# implementa una forma de hacer esto, que se llama genericos.

// Los genericos nos permiten crear una clase que puede trabajar con cualquier tipo de dato,
// sin necesidad de crear una clase para cada tipo de dato.

class Lista<T> {
    T[] Elementos = new T[10];
    int Contador = 0;

    public Lista(){}

    public void Agregar(T elemento){
        Elementos[Contador++] = elemento;
    }

    public bool Contiene(T elemento){
        for (var i = 0; i < Contador; i++){
            if (elemento.Equals(Elementos[i])) // Observar que usamos `Equals` en lugar de `==`
                return true;
        }
        return false;
    }
}

// Observen que ahora el tipo concreto no esta definido al momento de crear la lista...
// en su lugar ponemos un tipo generico `T`.  (El nombre puede ser cualquier otro, pero por convención se usa T)
// Esto nos permite crear una lista de cualquier tipo de dato, sin necesidad de crear una clase para cada tipo de dato.

// Ahora la podemos usar
WriteLine("\n> Lista de enteros `Lista<int>`");
var l3 = new Lista<int>(); //ListaInt  // Aca reemplazamos `T` por `int` y el compilador se encarga de crear la clase `ListaInt`
l3.Agregar(3);
l3.Agregar(2);
if(l3.Contiene(2)) 
    WriteLine("El elemento 2 está en la lista");
else
    WriteLine("El elemento 2 no está en la lista");


// O podemos usarla con otro tipo de dato
WriteLine("\n> Lista de strings `Lista<string>`");
var l4 = new Lista<string>();
l4.Agregar("Juan");
l4.Agregar("Pedro");
if(l4.Contiene("Juan")) 
    WriteLine("El elemento Juan está en la lista");
else 
    WriteLine("El elemento Juan no está en la lista");

// Se dice que la clase List<T> es generica y esta 'abierta' es decir que todavia el tipo T no esta definido.

// Cuando se reemplaza T por un tipo concreto, se dice que esta 'cerrada'.

// La clase List<T> es una clase generica que ya viene en el framework de .NET 
// y que nos permite crear listas de cualquier tipo de dato.

// La case Dictionary<TKey, TValue> es otra clase generica que nos permite crear diccionarios
// de cualquier tipo de dato. Un diccionario es una coleccion de pares clave-valor.

// Por ejemplo, si queremos contar la cantidad de veces que aparece cada palabra en un texto,

var contador = new Dictionary<string, int>();
string texto = "hola mundo hola a todos hola a todos los que estan en el mundo";
string[] palabras = texto.Split(" ");

WriteLine("\n> Contando palabras");
foreach(var palabra in palabras){
    if (contador.ContainsKey(palabra)){
        contador[palabra]++ // contarPalabras[palabra] = contarPalabras[palabra] + 1;
    } else {
        contador[palabra] = 1; // o contarPalabras.Add(palabra, 1);
    }
    // contador[palabra] = contador.GetValueOrDefault(palabra, 0) + 1; // Otra forma de hacerlo
}
WriteLine("\n> Contando palabras");
WriteLine($" Hay {contador.Count} palabras diferentes");
foreach(var palabra in contador){
    WriteLine($"- {palabra.Key,-10} {palabra.Value,2}");
}

// En este caso, la clave es el string y el valor es el int.
// La clase Dictionary<TKey, TValue> es una clase generica que nos permite crear diccionarios
// de cualquier tipo de dato.

/// Ahora bien supongamos que queremos hacer una lista de Alumnos 
public class Alumno : object {
    public string Nombre { get; set; }
    public int Legajo { get; set; }

    public Alumno(string nombre, int legajo){
        Nombre = nombre;
        Legajo = legajo;
    }

    public override string ToString(){
        return $"{Nombre} ({Legajo})";
    }
}

// Ahora la podemos usar 
WriteLine("\n> Lista de Alumnos `Lista<Alumno>`");
var j = new Alumno("Juan", 123);
var p = new Alumno("Pedro", 456);

var l5 = new Lista<Alumno>();
l5.Agregar(j);
l5.Agregar(p);
if(l5.Contiene(j)) 
    WriteLine("- El elemento Juan está en la lista");
else 
    WriteLine("- El elemento Juan no está en la lista");

// Y todo funciono bien... pero...
var o = new Alumno("Juan Carlos", 123);

if(l5.Contiene(o)) 
    WriteLine("El elemento Juan Carlos está en la lista");
else 
    WriteLine("El elemento Juan Carlos no está en la lista");

// Como vemos, el metodo Contiene no funciona como esperabamos.
// ya que un alumno se lo identifica por su legajo... si j.Legajo = 123 y o.Legajo = 123  deberia j.Equals(o) 
// y por lo tanto Contiene deberia devolver `true`.

// Para solucionar esto, tenemos que sobreescribir el metodo `Equals` de la clase `Alumno`
// (Uso herencia para no escribir todo de nuevo)

class Alumno2 : Alumno {
    public Alumno2(string nombre, int legajo): base(nombre, legajo){}

    // public override bool Equals(object obj){
    //     if(obj == null) return false;            // Revisa que no le pase null
    //     if(obj is not Alumno2) return false;     // Revisa que sea del mismo tipo
    //     Alumno2 a = (Alumno2)obj;                // Lo convierte a Alumno2
    //     return Legajo == a.Legajo;          // Compara el legajo
    // }
    
    public override bool Equals(object obj){
        if (obj is Alumno2 a){  // Revisa y convierte a Alumno2 usando `patrones`
            return Legajo == a.Legajo;
        }
        return false;
    }
}

WriteLine("\n> Lista de Alumnos `Lista<Alumno>`");
var j1 = new Alumno2("Juan", 123);
var p1 = new Alumno2("Pedro", 456);
var o0 = new Alumno2("Juan Carlos", 123);

var l6 = new Lista<Alumno2>();
l6.Agregar(j1);
l6.Agregar(p1);
if(l6.Contiene(o0)) 
    WriteLine("El elemento Juan está en la lista");
else 
    WriteLine("El elemento Juan no está en la lista");

// Y todo funciono bien... pero...
var o3 = new Alumno2("Juan Carlos", 123);

if(l6.Contiene(o3)) 
    WriteLine("El elemento Juan Carlos está en la lista");
else 
    WriteLine("El elemento Juan Carlos no está en la lista");

// Como vemos, el metodo Contiene ahora funciona como esperabamos.
// El problema central es que todos las clases implementar el metodo Equals, al no hacerlo 
// usaba el Equals de la clase object, que compara las referencias de los objetos.
// En este caso, como los legajos son iguales, devuelve true.
// En nuestro casos dos alumnos son iguales si tienen el mismo legajo.

WriteLine("\n> Lista de Alumnos `Lista<Alumno2>`");
var j2 = new Alumno2("Juan", 123);
var p2 = new Alumno2("Pedro", 456);

var l7 = new Lista<Alumno2>();
l7.Agregar(j2);
l7.Agregar(p2);

var o2 = new Alumno2("Juan Carlos", 123);

if (l7.Contiene(o2)) 
    WriteLine("El elemento Juan Carlos está en la lista");
else 
    WriteLine("El elemento Juan Carlos no está en la lista");

// En resumen, los genericos nos permiten crear clases que pueden trabajar con cualquier tipo de dato,
// sin necesidad de crear una clase para cada tipo de dato.
// Esto nos permite crear clases mas flexibles y reutilizables.

// Los genericos son una herramienta muy poderosa que nos permite crear clases y metodos

// Veamos por ejemplo como hacer una funcion que ordene una lista de cualquier tipo de dato

void Ordenar(int[] lista){
    var cambio = false;
    do {
        cambio = false;
        for (int j = 0; j < lista.Length - 1; j++) { // Usar j en lugar de i
            if (! (lista[j] <= lista[j + 1])) {      // Si no está en el orden correcto 
                // Intercambiamos los elementos         
                var tmp  = lista[j];
                lista[j] = lista[j + 1];
                lista[j + 1] = tmp;

                cambio = true;  // Si hubo un cambio, seguimos ordenando
            }
        }
    } while (cambio);
}

// Para que esto funcion debe comparar los elementos de la lista mediante el '<=', 
// pero no todo los tipos de datos tienen este operador definido. Mejor usemos `.CompareTo` 
// que es un metodo que tienen todos los tipos de datos y que devuelve -1, 0 o 1 dependiendo si el objeto
// es menor, igual o mayor que el otro objeto.

void Ordenar(string[] lista){
    var cambio = false;
    do {
        cambio = false;
        for (int i = 0; i < lista.Length - 1; i++) { // Corregido para usar j en lugar de i
            if (lista[i].CompareTo(lista[i + 1]) > 0) { // Si no está en el orden correcto 
                // Intercambiamos los elementos         
                var tmp  = lista[i];
                lista[i] = lista[i + 1];
                lista[i + 1] = tmp;

                cambio = true;  // Si hubo un cambio, seguimos ordenando
            }
        }
    } while (cambio);
}

// Pero esto no es muy practico, ya que tenemos que hacer una funcion para cada tipo de dato.
// Podemos hacer una funcion generica que reciba un array de cualquier tipo de dato

void Ordenar<T>(T[] lista) where T : IComparable<T> {
    var cambio = false;
    do {
        cambio = false;
        for (int i = 0; i < lista.Length - 1; i++) { // Recorre el array 
            if (lista[i].CompareTo(lista[i + 1]) > 0) { // Si no esta en el orden correcto 
                // Intercambiamos los elementos         
                var tmp  = lista[i];
                lista[i] = lista[i + 1];
                lista[i + 1] = tmp;

                cambio = true;  // Si hubo un cambio, seguimos ordenando
            }
        }
    } while (cambio);
}

// No podemos poner simplemente T porque si no sabemos si el tipo de dato tiene el metodo CompareTo.
// Los tipos que vienen con c# lo tienen pero los tipos que creamos nosotros no.
// Por eso tenemos que poner `where T : IComparable<T>`
// Esto significa que T tiene que ser un tipo que implemente la interfaz IComparable<T>.

// La interfaz IComparable<T> es una interfaz que define el metodo CompareTo
// que nos permite comparar dos objetos de tipo T.

class Persona : IComparable<Persona> {
    // Implementamos la interfaz IComparable<Persona>
    public int DNI { get; set; }
    public string Nombre { get; set; }

    public Persona(int dni, string nombre){
        DNI = dni;
        Nombre = nombre;
    }

    public override bool Equals(object obj) { // Sobrecarga del método Equals
        if (obj is Persona otro) {
            return DNI.Equals(otro.DNI);
        }
        return false;
    }

    public int CompareTo(Persona otra){ // Implementamos el metodo CompareTo definido en IComparable<Persona>        
        return DNI.CompareTo(otra.DNI); // Comparamos por DNI
    }
    // Este es un ejemplo si quisieramos ordenar por `Nombre`
    // public int CompareTo(Persona otra){ // Implementamos el metodo CompareTo definido en IComparable<Persona>
    //     return Nombre.CompareTo(otra.Nombre); // Comparamos por DNI
    // }

    public override string ToString() {
        return $"{Nombre,-10} (DNI: {DNI})";
    }
}

var persona1 = new Persona(456, "Juan");
var persona2 = new Persona(123, "Pedro");
var persona3 = new Persona(789, "Maria");

var listaPersonas = new Persona[] { persona1, persona2, persona3 };
WriteLine("\n> Lista de Personas");
foreach(var p in listaPersonas){
    WriteLine($"- {p}");
}

Ordenar(listaPersonas);

WriteLine("\n> Lista de Personas ordenada");
foreach(var p in listaPersonas){
    WriteLine($"- {p}");
}


// En este caso, la clase Persona implementa la interfaz IComparable<Persona>
// y por lo tanto podemos usar el metodo Ordenar<T>(T[] lista) para ordenar la lista de personas.
// La interfaz IComparable<T> es una interfaz generica que nos permite comparar dos objetos de tipo T.

