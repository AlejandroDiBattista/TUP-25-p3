using System;
using System.IO;

public struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;

   
    public Contacto(int id, string nombre, string telefono, string email)
    {
        Id = id;
        Nombre = nombre;
        Telefono = telefono;
        Email = email;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Menu menu = new Menu();
        menu.MostrarMenu();
    }
}

public struct Menu
{
    private Contacto[] contactos;
    private int siguienteId;

    // Constructor
    public Menu()
    {
        contactos = new Contacto[5];  
        siguienteId = 1;
        CargarContactosagenda();
    }

    private void CargarContactosagenda()
    {
        if (File.Exists("agenda.csv"))
        {
            string[] lineas = File.ReadAllLines("agenda.csv");
            for (int i = 0; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split(',');
                if (datos.Length == 4)
                {
                    int id = int.Parse(datos[0].Trim());
                    string nombre = datos[1].Trim();
                    string telefono = datos[2].Trim();
                    string email = datos[3].Trim();

                    
                    if (id >= siguienteId)
                        siguienteId = id + 1; 
                    
                    if (i < contactos.Length)
                    {
                        contactos[i] = new Contacto(id, nombre, telefono, email);
                    }
                }
            }
        }
    }

    private void GuardarContactos()
    {
        string[] lineas = new string[contactos.Length];
        int index = 0;
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id != 0) 
            {
                lineas[index] = $"{contactos[i].Id}, {contactos[i].Nombre}, {contactos[i].Telefono}, {contactos[i].Email}";
                index++;
            }
        }
        File.WriteAllLines("agenda.csv", lineas);
    }

    public void MostrarMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("--------------------");
            Console.WriteLine("AGENDA DE CONTACTOS");
            Console.WriteLine("--------------------");
            Console.WriteLine("1- Agregar contacto");
            Console.WriteLine("2- Modificar contacto");
            Console.WriteLine("3- Eliminar contacto");
            Console.WriteLine("4- Listar contactos");
            Console.WriteLine("5- Buscar contacto");
            Console.WriteLine("0- Salir");

            string opcion = Console.ReadLine();

            Console.Clear();

            if (opcion == "1") AgregarContacto();
            else if (opcion == "2") ModificarContacto();
            else if (opcion == "3") EliminarContacto();
            else if (opcion == "4") ListarContactos();
            else if (opcion == "5") BuscarContacto();
            else if (opcion == "0")
            {
                Salir();
                break;
            }
            else
            {
                Console.WriteLine("Opción incorrecta");
                Console.ReadKey();
            }
        }
    }

    private void AgregarContacto()
    {
        bool agendaLlena = true;
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id == 0) 
            {
                agendaLlena = false;
                break;
            }
        }

        if (agendaLlena)  
        {
            Console.WriteLine("Agenda llena. No se pueden agregar más contactos.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.WriteLine("___ AGREGAR CONTACTO ___");
        Console.Write("Ingrese el nombre: ");
        string nombre = Console.ReadLine();
        Console.Write("Ingrese el número de teléfono: ");
        string telefono = Console.ReadLine();
        Console.Write("Ingrese el email: ");
        string email = Console.ReadLine();

        Contacto nuevoContacto = new Contacto(siguienteId++, nombre, telefono, email);

        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id == 0) 
            {
                contactos[i] = nuevoContacto;
                Console.WriteLine($"Contacto guardado con ID: {nuevoContacto.Id}");
                break;
            }
        }

        GuardarContactos();
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void ModificarContacto()
    {
        Console.Clear();
        Console.WriteLine("___ MODIFICAR CONTACTO ___");
        Console.WriteLine("Ingrese el ID del contacto a modificar: ");
        string idModificar = Console.ReadLine();

        bool encontrado = false;
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id == int.Parse(idModificar))
            {
                encontrado = true;
                Console.WriteLine("Modificar nombre (dejar vacío para no cambiar): ");
                string nuevoNombre = Console.ReadLine();
                Console.WriteLine("Modificar teléfono (dejar vacío para no cambiar): ");
                string nuevoTelefono = Console.ReadLine();
                Console.WriteLine("Modificar email (dejar vacío para no cambiar): ");
                string nuevoEmail = Console.ReadLine();

                if (!string.IsNullOrEmpty(nuevoNombre)) contactos[i].Nombre = nuevoNombre;
                if (!string.IsNullOrEmpty(nuevoTelefono)) contactos[i].Telefono = nuevoTelefono;
                if (!string.IsNullOrEmpty(nuevoEmail)) contactos[i].Email = nuevoEmail;

                Console.WriteLine("Contacto modificado correctamente.");
                break;
            }
        }

        if (!encontrado) Console.WriteLine("ID no encontrado.");
        GuardarContactos();
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void EliminarContacto()
    {
        Console.Clear();
        Console.WriteLine("___ELIMINAR CONTACTO ___");
        Console.WriteLine("Ingrese el ID del contacto a eliminar: ");
        string idEliminar = Console.ReadLine();

        bool encontrado = false;
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id == int.Parse(idEliminar))
            {
                encontrado = true;
                contactos[i] = new Contacto(); 
                Console.WriteLine("Contacto eliminado correctamente.");
                break;
            }
        }

        if (!encontrado) Console.WriteLine("ID no encontrado.");
        GuardarContactos();
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void ListarContactos()
    {
        Console.Clear();
        Console.WriteLine("___ LISTAR CONTACTOS ___");
        Console.WriteLine("Listando contactos:");

        Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-20}", "ID", "Nombre", "Teléfono", "Email");

        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id != 0)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-20}", contactos[i].Id, contactos[i].Nombre, contactos[i].Telefono, contactos[i].Email);
            }
        }

        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void BuscarContacto()
    {
        Console.Clear();
        Console.WriteLine("___ BUSCAR CONTACTO ___");
        Console.WriteLine("Ingrese el nombre del contacto: ");
        string busqueda = Console.ReadLine().ToLower();

        bool encontrado = false;
        for (int i = 0; i < contactos.Length; i++)
        {
            if (contactos[i].Id != 0 &&
                (contactos[i].Nombre.ToLower().Contains(busqueda) ||
                 contactos[i].Telefono.Contains(busqueda) ||
                 contactos[i].Email.ToLower().Contains(busqueda)))
            {
                Console.WriteLine($"ID: {contactos[i].Id} | Nombre: {contactos[i].Nombre} | Teléfono: {contactos[i].Telefono} | Email: {contactos[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado) Console.WriteLine("No se encontraron contactos.");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    private void Salir()
    {
        GuardarContactos(); 
        Console.WriteLine("Saliendo...");
    }
}