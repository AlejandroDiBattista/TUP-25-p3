using System;
using System.IO;

int cantidadMaximaContactos = 100;
Contacto[] contactos = new Contacto[cantidadMaximaContactos];
int cantidadContactos = 0;
int proxID = 1;

CargarDesdeArchivo();

while (true)
{
    Console.Clear();
    Console.WriteLine("----- AGENDA DE CONTACTOS -----");

    Console.WriteLine("1. Agregar contacto");
    Console.WriteLine("2. Modificar contacto");
    Console.WriteLine("3. Borrar contacto");
    Console.WriteLine("4. Listar contactos");
    Console.WriteLine("5. Buscar contacto");
    Console.WriteLine("0. Salir");
    Console.Write("Seleccione una opción: ");
    string opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            Console.Clear();
            Console.WriteLine("----- AGREGAR CONTACTO -----"); 
            AgregarContacto(); break;
        case "2":
            Console.Clear();
            Console.WriteLine("----- MODIFICAR CONTACTO -----"); 
            ModificarContacto(); break;
        case "3":
            Console.Clear();
            Console.WriteLine("----- BORRAR CONTACTO -----"); 
            BorrarContacto(); break;
        case "4":
            Console.Clear();
            Console.WriteLine("----- LISTA DE CONTACTOS -----"); 
            ListarContactos(); break;
        case "5":
            Console.Clear();
            Console.WriteLine("----- BUSCAR CONTACTO -----"); 
            BuscarContacto(); break;
        case "0": GuardarArchivo(); return;
        default:
            Console.WriteLine("Opción no válida. Presione una tecla para continuar.");
            Console.ReadKey();
            break;
    }
}

struct Contacto
{
    public int ID;
    public string Nombre;
    public string Telefono;
    public string Email;
}

string LeerTexto(string mensaje)
{
    string input;
    do
    {
        Console.Write(mensaje);
        input = Console.ReadLine().Trim();
    } while (string.IsNullOrEmpty(input) || int.TryParse(input, out _));
    return input;
}

string LeerNumero(string mensaje)
{
    string input;
    do
    {
        Console.Write(mensaje);
        input = Console.ReadLine().Trim();
    } while (string.IsNullOrEmpty(input) || !long.TryParse(input, out _));
    return input;
}

void AgregarContacto()
{
    if (cantidadContactos >= cantidadMaximaContactos)
    {
        Console.WriteLine("No se pueden agregar más contactos.");
        Console.ReadKey();
        return;
    }

    Contacto nuevoContacto = new Contacto();
    nuevoContacto.ID = proxID++;
    nuevoContacto.Nombre = LeerTexto("Ingrese el nombre del contacto: ");
    nuevoContacto.Telefono = LeerNumero("Ingrese el teléfono del contacto: ");
    nuevoContacto.Email = LeerTexto("Ingrese el email del contacto: ");

    contactos[cantidadContactos++] = nuevoContacto;
    Console.WriteLine("Contacto agregado exitosamente. Presione una tecla para continuar.");
    Console.ReadKey();
}

void ModificarContacto()
{
    if (cantidadContactos == 0)
    {
        Console.WriteLine("No hay contactos para modificar.");
        Console.ReadKey();
        return;
    }

    ListarContactos();
    int id = int.Parse(LeerNumero("Ingrese el ID del contacto a modificar: "));
    int indice = -1;

    for (int i = 0; i < cantidadContactos; i++)
    {
        if (contactos[i].ID == id)
        {
            indice = i;
            break;
        }
    }

    if (indice == -1)
    {
        Console.WriteLine("Contacto no encontrado. Presione una tecla para continuar.");
        Console.ReadKey();
        return;
    }

    contactos[indice].Nombre = LeerTexto("Ingrese el nuevo nombre del contacto: ");
    contactos[indice].Telefono = LeerNumero("Ingrese el nuevo teléfono del contacto: ");
    contactos[indice].Email = LeerTexto("Ingrese el nuevo email del contacto: ");

    Console.WriteLine("Contacto modificado exitosamente. Presione una tecla para continuar.");
    Console.ReadKey();
}

void BorrarContacto()
{
    if (cantidadContactos == 0)
    {
        Console.WriteLine("No hay contactos para borrar.");
        Console.ReadKey();
        return;
    }

    ListarContactos();
    int id = int.Parse(LeerNumero("Ingrese el ID del contacto a borrar: "));
    int indice = -1;

    for (int i = 0; i < cantidadContactos; i++)
    {
        if (contactos[i].ID == id)
        {
            indice = i;
            break;
        }
    }

    if (indice == -1)
    {
        Console.WriteLine("Contacto no encontrado. Presione una tecla para continuar.");
        Console.ReadKey();
        return;
    }

    for (int i = indice; i < cantidadContactos - 1; i++)
    {
        contactos[i] = contactos[i + 1];
    }
    cantidadContactos--;

    Console.WriteLine("Contacto eliminado exitosamente. Presione una tecla para continuar.");
    Console.ReadKey();
}

void ListarContactos()
{
    Console.WriteLine("Lista de contactos:");
    for (int i = 0; i < cantidadContactos; i++)
    {
        Console.WriteLine($"ID: {contactos[i].ID}, Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
    }
    Console.WriteLine("Presione una tecla para continuar.");
    Console.ReadKey();
}

void BuscarContacto()
{
    string termino = LeerTexto("Ingrese un término de búsqueda: ");
    bool encontrado = false;

    for (int i = 0; i < cantidadContactos; i++)
    {
        if (contactos[i].Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"ID: {contactos[i].ID}, Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
            encontrado = true;
        }
    }

    if (!encontrado)
    {
        Console.WriteLine("No se encontraron contactos.");
    }

    Console.WriteLine("Presione una tecla para continuar.");
    Console.ReadKey();
}

void CargarDesdeArchivo()
{
    proxID = 1; 

    if (!File.Exists("agenda.csv")) return;

    string[] lineas = File.ReadAllLines("agenda.csv");
    for (int i = 0; i < lineas.Length; i++)
    {
        string[] datos = lineas[i].Split(',');
        if (datos.Length == 4)
        {
            contactos[cantidadContactos++] = new Contacto
            {
                ID = int.Parse(datos[0]),
                Nombre = datos[1],
                Telefono = datos[2],
                Email = datos[3]
            };

            int idActual = int.Parse(datos[0]);
            if (idActual >= proxID)
            {
                proxID = idActual + 1;
            }
        }
    }
}

void GuardarArchivo()
{
    using (StreamWriter writer = new StreamWriter("agenda.csv"))
    {
        Console.WriteLine(" ID,Nombre,Telefono,Email");
        for (int i = 0; i < cantidadContactos; i++)
        {
            writer.WriteLine($"{contactos[i].ID},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
        }
    }
    Console.WriteLine("Contactos guardados en agenda.csv. Presione una tecla para continuar.");
    Console.ReadKey();
}