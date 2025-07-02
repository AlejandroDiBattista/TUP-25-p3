
using static System.Console;

class Animal {
    public int Patas;
    protected Animal(int patas){ // Cambiado a protected
        Patas = patas;
    }

    public virtual void Saludar(){
        WriteLine("Hola, soy un animal");
    }
}

class Perro: Animal {
    public string Nombre;
    public Perro(string nombre): base(4){
        Nombre = nombre;
    }

    public override void Saludar(){
        WriteLine($"Hola, soy un perro y me llamo {Nombre}");
    }
}

class Gato: Animal {
    public string Nombre;
    public Gato(string nombre): base(4){
        Nombre = nombre;
    }

    public override void Saludar(){
        WriteLine($"Hola, soy un gato y me llamo {Nombre}");
    }
}

void DemoHerencia(){
    Clear();
    var perro = new Perro("Firulais");
    var gato  = new Gato("Miau");
    perro.Saludar();
    gato.Saludar();

    Animal animal = new Perro("Rocco");
    animal.Saludar(); // Llama al método de la clase base
}

DemoHerencia();

class Empleado: Persona {
    public string Cargo { get; set; }
    public float Sueldo { get; set; }

    public Empleado(string nombre, int edad, string cargo, float sueldo): base(nombre, edad){
        Cargo = cargo;
        Sueldo = sueldo;
    }

    public override string ToString(){
        return $"{base.ToString()}, Cargo: {Cargo}, Sueldo: {Sueldo}";
    }

    public virtual float SueldoAnual {
        get {
            return Sueldo * 12;
        }
    }

    public virtual void Mostrar(){
        WriteLine($"Nombre: {Nombre}, Edad: {Edad}, Cargo: {Cargo}, Sueldo Anual: {SueldoAnual}");
    }

}

class Gerente: Empleado {
    public string Departamento { get; set; }
    public List<Empleado> Empleados { get; set; } = new List<Empleado>();
   
    public Gerente(string nombre, int edad, string cargo, float sueldo, string departamento): base(nombre, edad, cargo, sueldo){
        Departamento = departamento;
    }

    public void AgregarEmpleado(Empleado empleado){
        Empleados.Add(empleado);
    }

    public override void Mostrar(){
        base.Mostrar(); // Llama al método de la clase base (Empleado)
        WriteLine($"> Empleados a cargo de {Nombre}:");
        foreach (var empleado in Empleados){
            WriteLine($" - {empleado.Nombre}");
        }
    }

    public override float SueldoAnual {
        get {
            float total = base.SueldoAnual;
            foreach (var empleado in Empleados){
                total += empleado.SueldoAnual * 0.1f; // 10% más por empleado a cargo
            }
            return total;
        }
    }

    public override string ToString(){
        return $"{base.ToString()}, Departamento: {Departamento}";
    }
}

void DemoHerencia2(){
    Clear();
    var empleado = new Empleado("Juan", 30, "Desarrollador", 50000);
    var gerente = new Gerente("Maria", 35, "Gerente de Ventas", 80000, "Ventas");
    empleado.Mostrar();
    gerente.Mostrar();
}

DemoHerencia2();