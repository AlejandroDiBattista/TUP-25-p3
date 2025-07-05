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
class Program
{
    struct Contact
    {
        public int Id;
        public string Name;
        public string Phone;
        public string Email;
    }

    static Contact[] contacts = new Contact[100];
    static int contactCount = 0;
    static string filePath = "agenda.csv";

    static void Main()
    {
        LoadContacts();

        for (int exit = 0; exit == 0;)
        {
            Console.Clear();
            Console.WriteLine("Agenda de Contactos");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("elegir una opcion: ");
            string option = Console.ReadLine();

            if (option == "1")
            {
                AddContact();
            }
            else if (option == "2")
            {
                ModifyContact();
            }
            else if (option == "3")
            {
                DeleteContact();
            }
            else if (option == "4")
            {
                ListContacts();
            }
            else if (option == "5")
            {
                SearchContact();
            }
            else if (option == "6")
            {
                SaveContacts();
                exit = 1; 
            }
            else
            {
                Console.WriteLine("opcion invalida, presione una tecla");
                Console.ReadKey();
            }
        }
    }

    static void LoadContacts()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] data = line.Split(',');
                if (data.Length == 4)
                {
                    contacts[contactCount].Id = int.Parse(data[0]);
                    contacts[contactCount].Name = data[1];
                    contacts[contactCount].Phone = data[2];
                    contacts[contactCount].Email = data[3];
                    contactCount++;
                }
            }
        }
    }

    static void SaveContacts()
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < contactCount; i++)
            {
                writer.WriteLine($"{contacts[i].Id},{contacts[i].Name},{contacts[i].Phone},{contacts[i].Email}");
            }
        }
    }

    static void AddContact()
    {
        if (contactCount >= contacts.Length)
        {
            Console.WriteLine("lista de contactos llena, presione una tecla");
            Console.ReadKey();
            return;
        }

        Console.Write("Ingresa el nombre: ");
        string name = Console.ReadLine();

        Console.Write("Ingresa el teléfono: ");
        string phone = Console.ReadLine();

        Console.Write("Ingresa el correo electrónico: ");
        string email = Console.ReadLine();

        contacts[contactCount].Id = contactCount + 1;
        contacts[contactCount].Name = name;
        contacts[contactCount].Phone = phone;
        contacts[contactCount].Email = email;

        contactCount++;

        Console.WriteLine("contacto agregado, presione una tecla");
        Console.ReadKey();
    }

    static void ModifyContact()
    {
        Console.Write("ingresar ID de contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        bool found = false;

        for (int i = 0; i < contactCount; i++)
        {
            if (contacts[i].Id == id)
            {
                found = true;
                Console.WriteLine($"modificando el contacto {id}: {contacts[i].Name}");

                Console.Write("ingresar nuevo nombre (o presionar enter para mantener el mismo): ");
                string name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name)) contacts[i].Name = name;

                Console.Write("ingresar nuevo telefono (o presionar enter para mantener el mismo): ");
                string phone = Console.ReadLine();
                if (!string.IsNullOrEmpty(phone)) contacts[i].Phone = phone;

                Console.Write("ingresar nuevo correo electronico (o presionar enter para mantener el mismo): ");
                string email = Console.ReadLine();
                if (!string.IsNullOrEmpty(email)) contacts[i].Email = email;

                Console.WriteLine("contacto actualizado, presione una tecla");
                Console.ReadKey();
                break;
            }
        }

        if (!found)
        {
            Console.WriteLine("contacto no encontrado, presione una tecla");
            Console.ReadKey();
        }
    }

    static void DeleteContact()
    {
        Console.Write("ingresar ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());

        bool found = false;

        for (int i = 0; i < contactCount; i++)
        {
            if (contacts[i].Id == id)
            {
                found = true;
                for (int j = i; j < contactCount - 1; j++)
                {
                    contacts[j] = contacts[j + 1];
                }
                contactCount--;
                Console.WriteLine("contacto borrado, presione una tecla");
                Console.ReadKey();
                break;
            }
        }

        if (!found)
        {
            Console.WriteLine("contacto no encontrado, presione una tecla");
            Console.ReadKey();
        }
    }

    static void ListContacts()
    {
        Console.WriteLine("ID\tNombre\tTeléfono\tCorreo");
        Console.WriteLine("-------------------------------------");

        for (int i = 0; i < contactCount; i++)
        {
            Console.WriteLine($"{contacts[i].Id}\t{contacts[i].Name}\t{contacts[i].Phone}\t{contacts[i].Email}");
        }

        Console.WriteLine("presione una tecla");
        Console.ReadKey();
    }

    static void SearchContact()
    {
        Console.Write("ingresar termino de busqueda: ");
        string searchTerm = Console.ReadLine().ToLower();

        Console.WriteLine("ID\tNombre\tTeléfono\tCorreo");
        Console.WriteLine("-------------------------------------");

        bool found = false;

        for (int i = 0; i < contactCount; i++)
        {
            if (contacts[i].Name.ToLower().Contains(searchTerm) || 
                contacts[i].Phone.ToLower().Contains(searchTerm) || 
                contacts[i].Email.ToLower().Contains(searchTerm))
            {
                found = true;
                Console.WriteLine($"{contacts[i].Id}\t{contacts[i].Name}\t{contacts[i].Phone}\t{contacts[i].Email}");
            }
        }

        if (!found)
        {
            Console.WriteLine("no se encuentran contactos, presione una tecla");
        }
        else
        {
            Console.WriteLine("presione una tecla");
        }
        
        Console.ReadKey();
    }
}