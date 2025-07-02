using static System.Console;


class Persona {
    public string Nombre {get;set;}
    public int Edad {get;set;}

    public void CumplirAño(){
        Edad++;
    }

    public virtual void Saludar(){ // virtual permite redefinir el método en la subclase
        WriteLine($"Hola, soy {Nombre} y tengo {Edad} años");
    }
}

var ale = new Persona{Nombre = "Alejandro"};
var cla = new Persona{Nombre = "Claudia"};

Clear();
WriteLine("\nSaludando a las personas");
ale.Saludar();
cla.Saludar();


// Herencia 'Alumno' es una 'Persona' (Alumno hereda de Persona)

// 'Alumno' es una clase derivada o subclase de 'Persona'
// 'Persona' es una clase base o superclase de 'Alumno'
// 'Alumno' hereda todos los miembros de 'Persona'
// 'Alumno' puede agregar nuevos miembros o redefinir los existentes
// 'Alumno' puede redefinir el método 'Saludar' de 'Persona'

class Alumno : Persona{
    public string Curso {get;set;} // Propiedad específica de Alumno

    public override void Saludar(){ // override redefine el método de la clase base
        base.Saludar(); // Llama al método de la clase base
        WriteLine($"Estoy cursando {Curso}");
    }

    public void Rendir(){ // Método específico de Alumno
        WriteLine($"Estoy rindiendo el exámen de {Curso}");
    }
}

var ana = new Alumno{Nombre="Analia", Curso="C#"};
var car = new Alumno{Nombre="Carlos", Curso="Java"};

WriteLine("\nSaludando a los alumnos");
ana.Saludar();
car.Saludar();
ana.Rendir();

Persona persona = ana; // 'persona' es una referencia a un objeto de tipo Alumno
persona.Saludar();
ana.Saludar();


// Todo 'hijo' puede ser tratado como 'padre' pero al revés no es posible

Persona a = new Alumno{Nombre="Analia", Curso="C#"}; // Valido
// Alumno b = new Persona{Nombre="Analia", Curso="C#"}; // ❌ Invalido

// Polimorfismo es la capacidad de un objeto de tomar muchas formas
// En este caso, el objeto 'ana' es de tipo Alumno pero se comporta como Persona
// Esto es posible porque Alumno hereda de Persona
// El método Saludar() de Alumno es llamado en lugar del de Persona

WriteLine("\nSaludando a todos");
// Persona[] es un array de Persona, pero puede contener Alumno
Persona[] persona = new[]{ ana, car, ale, cla };
foreach (var item in persona){
    item.Saludar();
}

//---

// 'objeto' es la clase base de todas las clases en C#

class Punto : object { // No es necesario especificar object, ya que todas las clases heredan de object (pero se puede)
    public int X {get;set;}
    public int Y {get;set;}

    public Punto(int x, int y){
        X = x;
        Y = y;
    }

    public override string ToString(){
        return $"({X}, {Y})";
    }
}

// --------------------------------------------------------------------
//
// object es la clase base de todas las clases en C#
//
object entero = 10;
object string = "Hola";
object punto = new Punto(10, 20);
object alumno = new Alumno{Nombre="Analia", Curso="C#"};

// Puedo asignar cualquier tipo a una variable de tipo `object`
// Pero no puedo acceder a sus miembros sin hacer un cast
// var x = entero.X; // ❌ Invalido

// var x = ((Punto)punto).X; // Valido
// var x = ((Alumno)alumno).Curso; // Valido
// var x = ((Persona)alumno).Curso; // ❌ Invalido
// var x = ((Alumno)alumno).Nombre; // Valido

// 'as' es un operador de cast (o conversion) seguro
// 'is' es un operador para verificar el tipo de un objeto

Punto p1 = p as Punto; // Cast seguro (si no se puede convertir, devuelve null)
Punto p2 = (Punto)p;   // Cast inseguro (o forzado)

Punto p3 = d as Punto; // Cast seguro (si no se puede convertir, devuelve null)
if(p is Punto){
    p3 = (Punto)p; // Cast inseguro (o forzado)
} else {
    WriteLine("No se puede convertir");
}

