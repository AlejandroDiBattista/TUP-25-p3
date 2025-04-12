using System;       
struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}
    const int maxCont = 100;
    static Contacto[] agenda = new Contacto[maxCont];
    static int totalContactos = 0;
    static int id = 1;


int opcion = 0;

        while (true)
        {
            Console.WriteLine("\nMenú de Opciones:");
            Console.WriteLine("1. Agregar Contacto");
            Console.WriteLine("2. Modificar Contacto");
            Console.WriteLine("3. Borrar Contacto");
            Console.WriteLine("4. Listar Contactos");
            Console.WriteLine("5. Buscar Contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Elige una opción: ");
            
            opcion = int.Parse(Console.ReadLine());

            if (opcion == 6)
            {
                Console.WriteLine("¡Chauuuuuuuuuuuuuuu!");
                break; 
            }

            switch (opcion)
            {
                case 1: AgregarContacto();
                   
                    Console.WriteLine("Opción: Agregar Contacto");
                    break;
                case 2: ModificarContacto();
                    
                    Console.WriteLine("Opción: Modificar Contacto");
                    break;
                case 3: BorrarContacto();
                  
                    Console.WriteLine("Opción: Borrar Contacto");
                    break;
                case 4: ListarContactos();
                  
                    Console.WriteLine("Opción: Listar Contactos");
                    break;
                case 5: BuscarContacto();
                   
                    Console.WriteLine("Opción: Buscar Contacto");
                    break;
                default:
                    Console.WriteLine("Opción no válida. Intenta de nuevo.");
                    break;
            }
            Console.WriteLine("\nPresiona una tecla para continuar...");
            Console.ReadKey();
        }
    
static void AgregarContacto()
{
    if (totalContactos >= agenda.Length)
    {
        Console.WriteLine("La agenda está llena.");
        return;
    }

    Console.Write("Nombre: ");
    string nombre = Console.ReadLine();

    Console.Write("Teléfono: ");
    string telefono = Console.ReadLine();

    Console.Write("Email: ");
    string email = Console.ReadLine();

    agenda[totalContactos] = new Contacto
    {
        Id = id,
        Nombre = nombre,
        Telefono = telefono,
        Email = email
    };

    Console.WriteLine("Contacto agregado correctamente.");

    totalContactos++;
    id++;
}
static void ModificarContacto()
{
    Console.Write("Ingresa el ID del contacto a modificar: ");
    int id = int.Parse(Console.ReadLine()); 

    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Id == id) 
        {
            Console.Write("Nuevo nombre: ");
            string nombre = Console.ReadLine();
            if (nombre != "") agenda[i].Nombre = nombre;

            Console.Write("Nuevo teléfono: ");
            string telefono = Console.ReadLine();
           if (telefono != "") agenda[i].Telefono = telefono;

            Console.Write("Nuevo email: ");
            string email = Console.ReadLine();
           if (email != "") agenda[i].Email = email;

            Console.WriteLine("Contacto modificado.");
            return; 
        }
    }

    Console.WriteLine("Contacto no encontrado.");
}
static void BorrarContacto()
{
    Console.Write("Ingresa el ID del contacto a borrar: ");
    int id = int.Parse(Console.ReadLine());

    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Id == id)
        {
            Console.WriteLine($"Contacto encontrado: {agenda[i].Nombre} - {agenda[i].Telefono} - {agenda[i].Email}");
            for (int j = i; j < totalContactos - 1; j++)
            {
                agenda[j] = agenda[j + 1]; 
            }
            totalContactos--; 
            Console.WriteLine("Contacto borrado.");
            return; 
        }
    }
    Console.WriteLine("No se encontró un contacto con ese ID.");
}
static void ListarContactos()
{
    if (totalContactos == 0)
    {
        Console.WriteLine("No hay contactos en la agenda.");
        return;
    }
    Console.WriteLine("\nLista de Contactos:");
    Console.WriteLine("ID | Nombre | Teléfono | Email");

    for (int i = 0; i < totalContactos; i++)
    {
        Console.WriteLine($"{agenda[i].Id} | {agenda[i].Nombre} | {agenda[i].Telefono} | {agenda[i].Email}");
    }
}
static void BuscarContacto()
{
    Console.Write("Ingresa el ID del contacto a buscar: ");
    int idBusqueda = int.Parse(Console.ReadLine());

    bool encontrado = false;

    Console.WriteLine("\nResultados de la búsqueda:");
    Console.WriteLine("ID | Nombre | Teléfono | Email");

    for (int i = 0; i < totalContactos; i++)
    {
        if (agenda[i].Id == idBusqueda)
        {
            Console.WriteLine($"{agenda[i].Id} | {agenda[i].Nombre} | {agenda[i].Telefono} | {agenda[i].Email}");
            encontrado = true;
        }
    }
    if (!encontrado)
    {
        Console.WriteLine("No se encontró un contacto con ese ID.");
    }
}
