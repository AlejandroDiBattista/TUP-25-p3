using System;
using System.IO;

class Persona
{
    static Agenda[] Cuentascontacto = new Agenda[5];     static int Indice = 0; 

    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public string Apellido { get; private set; }
    public string Email { get; private set; }
    public int Telefono { get; private set; }

    static int proximoId = 100;

    static int GenerarId()
    {
        return proximoId++;
    }

    public Persona(string nombre, string apellido, string email, int telefono)
    {
        Id = GenerarId();
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Telefono = telefono;
    }

    static void Main(string[] args)
    {
        bool salir = false;

        // Crear una nueva agenda
        Agenda agenda = new Agenda();
        
        while (!salir)
        {
            Console.Clear();
            Console.WriteLine("****1. Agregar contacto");
            Console.WriteLine("****2. Modificar contacto");
            Console.WriteLine("****3. Borrar contacto");
            Console.WriteLine("****4. Listar contacto");
            Console.WriteLine("****5. Buscar contacto");
            Console.WriteLine("****6. Salir");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    AgregarContacto(agenda);
                    break;
                case "2":
                    ModificarContacto(agenda);
                    break;
                case "3":
                    BorrarContacto(agenda);
                    break;
                case "4":
                    ListarContacto(agenda);
                    break;
                case "5":
                    BuscarContacto(agenda);
                    break;
                case "6":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción Incorrecta o Inexistente");
                    break;
            }
        }
    }

    static void AgregarContacto(Agenda agenda)
    {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Apellido: ");
        string apellido = Console.ReadLine();
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Write("Teléfono: ");
        int telefono = int.Parse(Console.ReadLine());

        Persona nuevaPersona = new Persona(nombre, apellido, email, telefono);
        agenda.Agregar(nuevaPersona);
        Console.WriteLine("Contacto agregado.");
    }

    static void ModificarContacto(Agenda agenda)
    {
        Console.Write("Ingrese el id ");
        int id = int.Parse(Console.ReadLine());
        agenda.Modificar(id);
    }

    static void BorrarContacto(Agenda agenda)
    {
        Console.Write("Ingrese el Id ");
        int id = int.Parse(Console.ReadLine());
        agenda.Borrar(id);
    }

    static void ListarContacto(Agenda agenda)
    {
        agenda.Mostrar();
    }

    static void BuscarContacto(Agenda agenda)
    {
        Console.Write("Ingrese id ");
        string busqueda = Console.ReadLine().ToLower();
        agenda.Buscar(busqueda);
    }
}

class Agenda
{
    int cantidad;
    Persona[] personas;

    public Agenda(int maximo = 3)
    {
        cantidad = 0;
        personas = new Persona[maximo];
    }

    public void Agregar(Persona persona)
    {
        if (cantidad < personas.Length)
        {
            personas[cantidad++] = persona;
        }
        else
        {
            Console.WriteLine("Agenda llena, no se pueden agregar más contactos.");
        }
    }

    public void Modificar(int id)
    {
        for (var i = 0; i < cantidad; i++)
        {
            if (personas[i].Id == id)
            {
                Console.WriteLine($"Contacto encontrado: {personas[i].Nombre} {personas[i].Apellido}");
                Console.Write("Nuevo nombre: ");
               // personas[i].Nombre = Console.ReadLine();
                Console.Write("Nuevo apellido: ");
                //personas[i].Apellido = Console.ReadLine();
                Console.Write("Nuevo email: ");
                //personas[i].Email = Console.ReadLine();
                Console.Write("Nuevo teléfono: ");
                //personas[i].Telefono = int.Parse(Console.ReadLine());
                Console.WriteLine("Contacto modificado.");
                return;
            }
        }
        Console.WriteLine("No se encontró el contacto con ese ID.");
    }

    public void Borrar(int id)
    {
        for (var i = 0; i < cantidad; i++)
        {
            if (personas[i].Id == id)
            {
                for (var j = i; j < cantidad - 1; j++)
                {
                    personas[j] = personas[j + 1];
                }
                cantidad--;
                Console.WriteLine("Contacto eliminado.");
                return;
            }
        }
        Console.WriteLine("No se encontró el contacto con ese ID.");
    }

    public void Mostrar()
    {
        Console.WriteLine($"\nLista de personas ({cantidad} de {personas.Length})");
        for (var i = 0; i < cantidad; i++)
        {
            Console.WriteLine($"ID: {personas[i].Id} - {personas[i].Nombre} {personas[i].Apellido}, Teléfono: {personas[i].Telefono}, Email: {personas[i].Email}");
        }
    }

    public void Buscar(string busqueda)
    {
        bool encontrado = false;
        foreach (var persona in personas)
        {
            if (persona != null && (persona.Nombre.ToLower().Contains(busqueda) || persona.Apellido.ToLower().Contains(busqueda) || persona.Email.ToLower().Contains(busqueda)))
            {
                Console.WriteLine($"ID: {persona.Id} - {persona.Nombre} {persona.Apellido}, Teléfono: {persona.Telefono}, Email: {persona.Email}");
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron contactos con el término de búsqueda.");
        }
    }
}