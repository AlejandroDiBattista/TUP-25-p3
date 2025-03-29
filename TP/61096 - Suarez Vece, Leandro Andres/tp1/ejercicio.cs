using System;
using System.IO;


public struct Contacto
{
    public int id;
    public string nombre;
    public string telefono;
    public string email;

    public Contacto(int id, string nombre, string telefono, string email)
    {
        this.id = id;
        this.nombre = nombre;
        this.telefono = telefono;
        this.email = email;
    }

};

public Contacto[] contactos = new Contacto[100];

public int option = 0;
public string file = "agenda.csv";

Console.Clear();
Console.WriteLine("===== Cargando datos =====");
Console.WriteLine("===== AGENDA DE CONTACTOS =====");
ReadFile(file, contactos);

do
{
    try
    {
        Console.WriteLine("\n1) Agregar contacto \n2) Modificar contacto \n3) Borrar contacto \n4) Listar contactos \n5) Buscar contacto \n0) Salir\n");
        option = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine(option);

        if (option < 0 || option > 5)
        {
            Console.Clear();

            Console.WriteLine("Tecla equivocada, saliendo del programa");
            option = 0;
        }
        if (option == 1)
        {
            Console.Clear();
            var id = ObtenerIndiceVacio(contactos);

            if (id < 0)
                Console.WriteLine("Memoria llena");

            Console.WriteLine("Rellene los datos con, NOMBRE, TELÉFONO, EMAIL");

            contactos[id] = rellenar(id);
        }
        if (option == 2)
        {
            Console.Clear();
            Console.WriteLine("Ingrese el Id del elemento a modificar");

            var id = int.Parse(Console.ReadLine());
            contactos[id] = rellenar(id);

        }
        if (option == 3)
        {
            Console.Clear();
            Console.WriteLine("Ingrese el Id del elemento a eliminar");

            var id = int.Parse(Console.ReadLine());
            contactos[id] = DeleteData(id);

        }
        if (option == 4)
        {
            Console.Clear();
            HeadTable();
            ViewContact(contactos);
        }
        if (option == 5)
        {
            Console.Clear();
            Console.WriteLine("Ingrese un término de búsqueda (nombre, teléfono o email)\n");

            var buscar = Console.ReadLine().ToLower();
            Search(contactos, buscar);
        }
    }
    catch (System.Exception)
    {
        Console.WriteLine("Tecla equivocada, saliendo del programa");
        option = 0;

    }

} while (option != 0);

Console.WriteLine("Guardando cambios...");
SaveFile(file, contactos);

Console.WriteLine("Presionar una tecla para Finalizar...");
Console.ReadKey();


// ########################## funciones manejar arrays ##########################

public void ViewContact(Contacto[] contact)
{

    for (int i = 1; i < contact.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(contact[i].nombre))

            break;

        Console.WriteLine($"{contact[i].id}  - {contact[i].nombre.ToLower()}   - {contact[i].telefono}  - {contact[i].email}");

    }
}

public void Search(Contacto[] contact, string data)
{
    for (int i = 0; i < contact.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(contact[i].nombre))
            continue;

        if (contact[i].nombre.ToLower().Contains(data)
            || contact[i].telefono.ToLower().Contains(data)
            || contact[i].email.ToLower().Contains(data))
        {
            HeadTable();
            Console.WriteLine($"{contact[i].id}  - {contact[i].nombre}   - {contact[i].telefono}   - {contact[i].email}");
            return;
        }
    }
    Console.WriteLine("nombre no encontrado");
}


public Contacto DeleteData(int id)
{
    return new Contacto { id = 0, nombre = "", email = "", telefono = "" };
}

public void HeadTable()
{
    Console.WriteLine("ID    NOMBRE     TELÉFONO       EMAIL ");
}

public Contacto rellenar(int id)
{
    var nombre = Console.ReadLine();
    var tel = Console.ReadLine();
    var email = Console.ReadLine();

    return new Contacto { id = id, nombre = nombre, telefono = tel, email = email };
}

public int ObtenerIndiceVacio(Contacto[] data)
{
    for (int i = 0; i < data.Length; i++)
    {
        if (string.IsNullOrWhiteSpace(data[i].nombre))
            return i;
    }
    return -1;

}

//############################ leer archivos #######################

public void ReadFile(string file, Contacto[] contactos)
{
    int id = 0;
    try
    {
        using (StreamReader lector = new StreamReader(file))
        {
            string line;
            while ((line = lector.ReadLine()) != null)
            {
                if (line == "")
                    break;

                string[] col = line.Split(',');

                var aux = new Contacto(id, col[0], col[1], col[2]);

                contactos[id] = aux;
                id++;
            }
        }
    }
    catch (System.Exception)
    {
        Console.Write("Error al abrir archivo");
        throw;
    }
}

public void SaveFile(string file, Contacto[] contactos)
{

    try
    {
        using (StreamWriter write = new StreamWriter(file))
        {
            for (int i = 0; i < contactos.Length; i++)
            {
                write.WriteLine($"{contactos[i].nombre},{contactos[i].telefono},{contactos[i].email}");
            }
        }
    }
    catch (System.Exception)
    {
        Console.Write("Error al abrir archivo");
        throw;
    }
}