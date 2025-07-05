using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)

//tp1
    struct Contacto
    {
       public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
    }

    
    const int maxC = 100;
    static Contacto[] contactos = new Contacto[maxC];
    static int totalC = 0; 
    static string Archivo = "agenda.csv";

    
    
        
        CargarContactosDesdeArchivo();

        while (true)
        {
            MostrarMenu();
            int opcion = PedirOpcion();

            switch (opcion)
            {
                case 1: AgregarContacto(); break;
                case 2: ModificarContacto(); break;
                case 3: BorrarContacto(); break;
                case 4: ListarContactos(); break;
                case 5: BuscarContacto(); break;
                case 0: 
                    GuardarContactosEnArchivo();
                    Console.WriteLine("Saliendo de la aplicación...");
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente nuevamente.");
                    break;
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
            Console.Clear();
        }
    

    static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("=== AGENDA DE CONTACTOS ===");
        Console.WriteLine("1) Agregar contacto");
        Console.WriteLine("2) Modificar contacto");
        Console.WriteLine("3) Borrar contacto");
        Console.WriteLine("4) Listar contactos");
        Console.WriteLine("5) Buscar contacto");
        Console.WriteLine("0) Salir");
    }

    static int PedirOpcion()
    {
        Console.Write("Seleccione una opción: ");
        int opcion;
        while (!int.TryParse(Console.ReadLine(), out opcion))
        {
            Console.Write("Opción inválida. Intente nuevamente: ");
        }
        return opcion;
    }

    static void AgregarContacto()
    {
        if (totalC >= maxC)
        {
            Console.WriteLine("La agenda está llena. No se pueden agregar más contactos.");
            return;
        }

        Console.WriteLine("\n=== Agregar Contacto ===");
        Contacto contacto = new();
        Contacto contactoNuevo = contacto;
        
        contactoNuevo.Id = totalC + 1;

        Console.Write("Nombre   : ");
        contactoNuevo.Nombre = Console.ReadLine();

        Console.Write("Teléfono : ");
        contactoNuevo.Telefono = Console.ReadLine();

        Console.Write("Email    : ");
        contactoNuevo.Email = Console.ReadLine();

        contactos[totalC] = contactoNuevo;
        totalC++;

        Console.WriteLine($"Contacto agregado con ID = {contactoNuevo.Id}");
    }

    static void ModificarContacto()
    {
        Console.WriteLine("\n=== Modificar Contacto ===");
        Console.Write("Ingrese el ID del contacto a modificar: ");
        
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.Write("ID inválido. Intente nuevamente: ");
        }

        int indice = -1;
        for (int i = 0; i < totalC; i++)
        {
            if (contactos[i].Id == id)
            {
                indice = i;
                break;
            }
        }

        if (indice == -1)
        {
            Console.WriteLine("Contacto no encontrado.");
            return;
        }

        Contacto contactoActual = contactos[indice];
        Console.WriteLine($"Datos actuales => Nombre: {contactoActual.Nombre}, Teléfono : {contactoActual.Telefono}, Email: {contactoActual.Email}");
        Console.WriteLine("(Deje el campo en blanco para no modificar)\n");

        Console.Write("Nombre    : ");
        string nuevoNombre = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoNombre))
            contactos[indice].Nombre = nuevoNombre;

        Console.Write("Teléfono  : ");
        string nuevoTelefono = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoTelefono))
            contactos[indice].Telefono = nuevoTelefono;

        Console.Write("Email     : ");
        string nuevoEmail = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoEmail))
            contactos[indice].Email = nuevoEmail;

        Console.WriteLine("\nContacto modificado con éxito.");
    }

    static void BorrarContacto()
    {
        Console.WriteLine("\n=== Borrar Contacto ===");
        Console.Write("Ingrese el ID del contacto a borrar: ");
        
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.Write("ID inválido. Intente nuevamente: ");
        }

        int indice = -1;
        for (int i = 0; i < totalC; i++)
        {
            if (contactos[i].Id == id)
            {
                indice = i;
                break;
            }
        }

        if (indice == -1)
        {
            Console.WriteLine("Contacto no encontrado.");
            return;
        }

       
        for (int i = indice; i < totalC - 1; i++)
        {
            contactos[i] = contactos[i + 1];
        }
        totalC--;

        Console.WriteLine($"Contacto con ID={id} eliminado con éxito.");
    }

    static void ListarContactos()
    {
        Console.WriteLine("\n=== Lista de Contactos ===");
        
        if (totalC == 0)
        {
            Console.WriteLine("No hay contactos en la agenda.");
            return;
        }

        
        Console.WriteLine("{0,-5}{1,-20}{2,-15}{3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");

   
        for (int i = 0; i < totalC; i++)
        {
            Console.WriteLine("{0,-5}{1,-20}{2,-15}{3,-25}", 
                contactos[i].Id, 
                contactos[i].Nombre, 
                contactos[i].Telefono, 
                contactos[i].Email);
        }
    }

    static void BuscarContacto()
    {
        Console.WriteLine("\n=== Buscar Contacto ===");
        Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
        string termino = Console.ReadLine().ToLower();

        Console.WriteLine("\nResultados de la búsqueda:");
        Console.WriteLine("{0,-5}{1,-20}{2,-15}{3,-25}", "ID", "NOMBRE", "TELÉFONO", "EMAIL");

        bool encontrado = false;
        for (int i = 0; i < totalC; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(termino) ||
                contactos[i].Telefono.ToLower().Contains(termino) ||
                contactos[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine("{0,-5}{1,-20}{2,-15}{3,-25}", 
                    contactos[i].Id, 
                    contactos[i].Nombre, 
                    contactos[i].Telefono, 
                    contactos[i].Email);
                encontrado = true;
            }
        }

        if (!encontrado)
        {
            Console.WriteLine("No se encontraron contactos que coincidan con el término de búsqueda.");
        }
    }

    static void CargarContactosDesdeArchivo()
    {
        if (!File.Exists(Archivo))
            return;

        string[] lineas = File.ReadAllLines(Archivo);
        totalC = 0;

        for (int i = 0; i < lineas.Length; i++)
        {
            string[] campos = lineas[i].Split(',');
            
            if (campos.Length == 4)
            {
                contactos[totalC] = new Contacto
                {
                    Id = int.Parse(campos[0]),
                    Nombre = campos[1],
                    Telefono = campos[2],
                    Email = campos[3]
                };
                totalC++;
            }
        }
    }

    static void GuardarContactosEnArchivo()
    {
        string[] lineasCsv = new string[totalC];
        
        for (int i = 0; i < totalC; i++)
        {
            lineasCsv[i] = $"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}";
        }

        File.WriteAllLines(Archivo, lineasCsv);
    }
