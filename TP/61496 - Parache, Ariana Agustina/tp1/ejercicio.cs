using System;
using System.IO;

class Agenda
{
    public int Id;
    public string? Nombre;
    public string? Telefono;
    public string? Email;

    static Contacto[] contactos = new Contacto[100];
    static int totalContactos = 0;

    static void Main()
    {
        CargarContactos();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("*****AGENDA DE CONTACTOS*****");
            Console.WriteLine("1) Agregar\n2) Modificar\n3) Borrar\n4) Listar\n5) Buscar\n0) Salir");


            string? opcion = Console.ReadLine();
            if (opcion == "0") { Salir(); break; }
            if (opcion == "1") Agregar();
            if (opcion == "2") Modificar();
            if (opcion == "3") Borrar();
            if (opcion == "4") Listar();
            if (opcion == "5") Buscar();
        }
    }

    static void Agregar()
    {
        if (totalContactos >= 100) { Console.WriteLine("Agenda llena."); Console.ReadKey(); return; }
        Console.Write("Nombre: "); string? nombre = Console.ReadLine();
        Console.Write("Teléfono: "); string? telefono = Console.ReadLine();
        Console.Write("Email: "); string? email = Console.ReadLine();
        contactos[totalContactos++] = new Contacto { Id = totalContactos, Nombre = nombre, Telefono = telefono, Email = email };
        Console.WriteLine("Contacto agregado."); Console.ReadKey();
    }

    static void Modificar()
    {
        Console.Write("ID a modificar: ");
        string? idInput = Console.ReadLine();
        int id = 0;


        bool esNumero = true;
        if (idInput != null)
        {
            for (int i = 0; i < idInput.Length; i++)
            {
                if (idInput[i] < '0' || idInput[i] > '9')
                {
                    esNumero = false;
                    break;
                }
            }

            if (esNumero)
            {
                id = idInput[0] - '0';
                if (id < 1 || id > totalContactos) { Console.WriteLine("ID no válido."); Console.ReadKey(); return; }

                Contacto c = contactos[id - 1];
                Console.WriteLine($"Datos actuales: {c.Nombre}, {c.Telefono}, {c.Email}");
                Console.Write("Nuevo Nombre: "); string? nombre = Console.ReadLine();
                Console.Write("Nuevo Teléfono: "); string? telefono = Console.ReadLine();
                Console.Write("Nuevo Email: "); string? email = Console.ReadLine();


                if (!string.IsNullOrEmpty(nombre) && nombre.Length > 0) c.Nombre = nombre;
                if (!string.IsNullOrEmpty(telefono) && telefono.Length > 0) c.Telefono = telefono;
                if (!string.IsNullOrEmpty(email) && email.Length > 0) c.Email = email;
                contactos[id - 1] = c;
                Console.WriteLine("Contacto modificado.");
            }
            else
            {
                Console.WriteLine("ID no válido.");
            }
            Console.ReadKey();
        }
    }

    static void Borrar()
    {
        Console.Write("ID a borrar: ");
        string? idInput = Console.ReadLine();
        int id = 0;

        bool esNumero = int.TryParse(idInput, out id);

        if (esNumero)
        {

            if (id < 1 || id > totalContactos)
            {
                Console.WriteLine("ID no válido.");
                Console.ReadKey();
                return;
            }


            for (int i = id - 1; i < totalContactos - 1; i++)
            {
                contactos[i] = contactos[i + 1];
            }

            totalContactos--;
            Console.WriteLine("Contacto borrado.");
        }
        else
        {
            Console.WriteLine("ID no válido.");
        }

        Console.ReadKey();
    }

    static void Listar()
    {

        Console.WriteLine("ID  NOMBRE    TELÉFONO   EMAIL");
        for (int i = 0; i < totalContactos; i++)
            Console.WriteLine($"{contactos[i].Id,-3} {contactos[i].Nombre,-10} {contactos[i].Telefono,-10} {contactos[i].Email}");
        Console.ReadKey();
    }

    static void Buscar()
    {
        Console.Write("Buscar: ");
        string? term = Console.ReadLine();
        bool encontrado = false;
        Console.WriteLine("ID  NOMBRE    TELÉFONO   EMAIL");

        for (int i = 0; i < totalContactos; i++)
        {
            Contacto c = contactos[i];
            bool nombreCoincide = false, telefonoCoincide = false, emailCoincide = false;

            if (c.Nombre != null && term != null && c.Nombre.Length >= term.Length)
            {
                for (int j = 0; j < c.Nombre.Length - term.Length + 1; j++)
                {
                    if (c.Nombre.Substring(j, term.Length) == term)
                    {
                        nombreCoincide = true;
                        break;
                    }
                }
            }
            if (c.Telefono != null && term != null && c.Telefono.Length >= term.Length)
            {
                for (int j = 0; j < c.Telefono.Length - term.Length + 1; j++)
                {
                    if (c.Telefono.Substring(j, term.Length) == term)
                    {
                        telefonoCoincide = true;
                        break;
                    }
                }
            }
            if (c.Email != null && term != null && c.Email.Length >= term.Length)
            {
                for (int j = 0; j < c.Email.Length - term.Length + 1; j++)
                {
                    if (c.Email.Substring(j, term.Length) == term)
                    {
                        emailCoincide = true;
                        break;
                    }

                }
            }

            if (nombreCoincide || telefonoCoincide || emailCoincide)
            {
                Console.WriteLine($"{c.Id,-3} {c.Nombre,-10} {c.Telefono,-10} {c.Email}");
                encontrado = true;
            }
        }
        if (!encontrado) Console.WriteLine("No se encontraron resultados.");
        Console.ReadKey();
    }

    static void CargarContactos()
    {
        if (File.Exists("agenda.csv"))
        {
            string[] lineas = File.ReadAllLines("agenda.csv");

            for (int i = 1; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split(',');
                if (datos.Length == 4)
                    contactos[totalContactos++] = new Contacto { Id = totalContactos, Nombre = datos[1], Telefono = datos[2], Email = datos[3] };
            }

        }
    }

    static void Salir()
    {
        string header = "ID,NOMBRE,TELEFONO,EMAIL";
        string[] lineas = new string[totalContactos];
        string[] lineasMasCabecera = new string[totalContactos + 1];

        if (lineas.Length > 0 && lineas[0] != header)
        {
            lineasMasCabecera[0] = header;
            for (int i = 0; i < totalContactos; i++)
                lineasMasCabecera[i + 1] = $"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}";
            File.WriteAllLines("agenda.csv", lineasMasCabecera);

        }
        else
        {
            for (int i = 0; i < totalContactos; i++)
                lineas[i] = $"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}";
            File.WriteAllLines("agenda.csv", lineasMasCabecera);
        }

        Console.WriteLine("Saliendo..."); Console.ReadKey();
    }
}

class Contacto
{
    public int Id;
    public string? Nombre;
    public string? Telefono;
    public string? Email;
}