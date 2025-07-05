using System;
using System.IO;

class Program
{
    struct Contacto
    {
        public int Id;
        public string Nombre;
        public string Telefono;
        public string Email;
    }

    static Contacto[] contactos = new Contacto[10]; 
    static int contactoCant = 0; 
    static string filePath = "agenda.csv"; 
    static void Main(string[] args)
    {
        Console.WriteLine("Bienvenido a su Agenda de Contactos de confianza");
        Console.Write("Presiona una tecla para continuar...");
        Console.ReadKey();
        CargarContactos();
        for (int salir = 0; salir == 0;)
        {
            Console.Clear();
            Console.WriteLine("------Agenda de Contactos------");
            Console.WriteLine("-------------------------------");
            Console.WriteLine("--opcion 1: Agregar Contacto---");
            Console.WriteLine("--opcion 2: Modificar Contacto-");
            Console.WriteLine("--opcion 3: Borrar Contacto----");
            Console.WriteLine("--opcion 4: Listar Contactos---");
            Console.WriteLine("--opcion 5: Buscar Contacto----");
            Console.WriteLine("--opcion 6: Salir del Programa-");
            Console.WriteLine("-------------------------------");
            int opcion = Validacion();
            if (opcion == 1)
            {
                    if (contactoCant < 10)
            {
                Contacto nuevoContacto = new Contacto();

                Console.WriteLine("Asignando ID al contacto...");
                for (int i = 0; i < contactoCant; i++)
                {
                    if (contactos[i].Id >= nuevoContacto.Id)
                    {
                        nuevoContacto.Id = contactos[i].Id + 1;
                    }
                    else
                    {
                        nuevoContacto.Id = 0;
                    }
                }
                Console.WriteLine("Ingrese el nombre del contacto:");
                nuevoContacto.Nombre = Console.ReadLine();

                Console.WriteLine("Ingrese los números de teléfono:");
                nuevoContacto.Telefono = Console.ReadLine();

                Console.WriteLine("Ingrese los correos electrónicos:");
                nuevoContacto.Email = Console.ReadLine();
                contactos[contactoCant] = nuevoContacto;
                contactoCant++;
            }
            else
            {
                Console.WriteLine("No se pueden agregar más contactos, la agenda está llena.");
            }
                Console.Write("Muchas gracias, presione una tecla para continuar...");
                Console.ReadKey();
         }
            else if (opcion == 2)
            {
            Console.WriteLine("Ingrese el ID del contacto a modificar:");
            int id = int.Parse(Console.ReadLine());
            bool encontrado = false;
            for (int i = 0; i < contactoCant; i++)
            {
                
                if (contactos[i].Id == id)
                {
                    Console.WriteLine("Ingrese el nuevo nombre del contacto:");
                    contactos[i].Nombre = Console.ReadLine();

                    Console.WriteLine("Ingrese el nuevo número de teléfono");
                    contactos[i].Telefono = Console.ReadLine();

                    Console.WriteLine("Ingrese el nuevo correo electrónico");
                    contactos[i].Email = Console.ReadLine();

                    Console.WriteLine("Contacto modificado con éxito.");
                    encontrado = true;
                    break;
                }
            }
            if (!encontrado)
                {
                    Console.WriteLine("Contacto no encontrado.");
                }
            Console.Write("Muchas gracias, presione una tecla para continuar...");
            Console.ReadKey();
            }
            else if (opcion == 3)
            {
                Console.WriteLine("Ingrese el ID del contacto a borrar:");
                int id = int.Parse(Console.ReadLine());

                bool encontrado = false;

                for (int i = 0; i < contactoCant; i++)
                {
                    if (contactos[i].Id == id)
                    {
                        for (int j = i; j < contactoCant - 1; j++)
                        {
                            contactos[j] = contactos[j + 1]; 
                        }
                        contactoCant--;
                        Console.WriteLine("Contacto borrado con éxito.");
                        encontrado = true; 
                        break; 
                    }
                }

                if (!encontrado)
                {
                    Console.WriteLine("Contacto no encontrado.");
                }
                Console.Write("Muchas gracias, presione una tecla para continuar...");
                Console.ReadKey();
            }
            else if (opcion == 4)
            {
            if (contactoCant == 0)
            {
                Console.WriteLine("No hay contactos para mostrar.");
            }else
{
                    Console.Clear();
                    Console.WriteLine("----- Lista de Contactos -----");
                    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", "ID", "Nombre", "Telefono", "Email");
                    Console.WriteLine("-------------------------------------------");
                    for (int i = 0; i < contactoCant; i++)
                    {
                        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-25}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
                    }
                }               
            Console.Write("Muchas gracias, presione una tecla para continuar...");
            Console.ReadKey();
            }
            else if (opcion == 5)
            {
                Console.WriteLine("Ingrese el ID del contacto a buscar:");
                int id = int.Parse(Console.ReadLine());
                bool encontrado = false;
                for (int i = 0; i < contactoCant; i++)
                {
                    if (contactos[i].Id == id)
                    {
                        Console.WriteLine($"ID: {contactos[i].Id}");
                        Console.WriteLine($"Nombre: {contactos[i].Nombre}");
                        Console.WriteLine($"Teléfonos: {string.Join(", ", contactos[i].Telefono)}");
                        Console.WriteLine($"Emails: {string.Join(", ", contactos[i].Email)}");
                        Console.WriteLine("--------------------------");
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    Console.WriteLine("Contacto no encontrado.");
                }
                Console.Write("Muchas gracias, presione una tecla para continuar...");
                Console.ReadKey();
            }
            else if (opcion == 6)

            {
                Console.WriteLine("Saliendo del programa...");
                salir = 1;
            if (salir == 1) break;
            }        
        }
        GuardarContactos();
    }

    static int Validacion()
    {
        int opcion = 0;
        Console.WriteLine("Ingrese una opcion:");
        string input = Console.ReadLine();
        if (int.TryParse(input, out opcion) && opcion >= 1 && opcion <= 6)
        {
            return opcion;
        }
        else
        {
            Console.WriteLine("Opcion invalida. Intente nuevamente.");
            return Validacion();
        }
    }

    static void CargarContactos()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                if (data.Length == 4)
                {
                    contactos[contactoCant].Id = int.Parse(data[0]);
                    contactos[contactoCant].Nombre = data[1];
                    contactos[contactoCant].Telefono = data[2];
                    contactos[contactoCant].Email = data[3];
                    contactoCant++;
                }
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            for (int i = 0; i < contactoCant; i++)
            {
                string telefonos = string.Join(";", contactos[i].Telefono);
                string emails = string.Join(";", contactos[i].Email);
                sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{telefonos},{emails}");
            }
        }
    }
}