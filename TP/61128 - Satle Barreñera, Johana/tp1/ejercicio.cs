using System;  
using System.IO;  

struct Contacto  
{  
    public int Id;  
    public string Nombre;  
    public string Telefono;  
    public string Email;  
}  

static class AgendaScript  
{  
    static Contacto[] contactos = new Contacto[100];  
    static int cantidad = 0;  
    static int siguienteId = 1;  

    static AgendaScript()  
    {  
        if (File.Exists("agenda.csv"))  
        {  
            string[] lineas = File.ReadAllLines("agenda.csv");  
            for (int i = 0; i < lineas.Length; i++)  
            {  
                string[] datos = lineas[i].Split(',');  
                if (datos.Length == 4)  
                {  
                    if (int.TryParse(datos[0], out int id))  
                    {  
                        contactos[i].Id = id;  
                        contactos[i].Nombre = datos[1];  
                        contactos[i].Telefono = datos[2];  
                        contactos[i].Email = datos[3];  
                        cantidad++;  
                        if (contactos[i].Id >= siguienteId)  
                            siguienteId = contactos[i].Id + 1;  
                    }  
                    else  
                    {  
                        Console.WriteLine($"Formato incorrecto en la línea {i + 1}: ID no es un número.");  
                    }  
                }  
                else  
                {  
                    Console.WriteLine($"Formato incorrecto en la línea {i + 1}: se esperaba 4 valores.");  
                }  
            }  
        }  
    }  

    public static void Ejecutar()  
    {  
        int opcion = 0;  
        while (opcion != 6)  
        {  
            Console.WriteLine("\n1. Agregar");  
            Console.WriteLine("2. Modificar");  
            Console.WriteLine("3. Borrar");  
            Console.WriteLine("4. Listar");  
            Console.WriteLine("5. Buscar");  
            Console.WriteLine("6. Salir");  
            Console.Write("Opción: ");  

            if (int.TryParse(Console.ReadLine(), out opcion))  
            {  
                if (opcion == 1) Agregar();  
                if (opcion == 2) Modificar();  
                if (opcion == 3) Borrar();  
                if (opcion == 4) Listar();  
                if (opcion == 5) Buscar();  
            }  
            else  
            {  
                Console.WriteLine("Opción inválida, por favor ingresa un número entre 1 y 6.");  
            }  
        }  

        using (StreamWriter sw = new StreamWriter("agenda.csv"))  
        {  
            for (int i = 0; i < cantidad; i++)  
            {  
                sw.WriteLine(contactos[i].Id + "," + contactos[i].Nombre + "," + contactos[i].Telefono + "," + contactos[i].Email);  
            }  
        }  
        Console.WriteLine("Agenda guardada.");  
    }  

    static void Agregar()  
    {  
        if (cantidad < 100)  
        {  
            contactos[cantidad].Id = siguienteId;  
            siguienteId++;  
            Console.Write("Nombre: ");  
            contactos[cantidad].Nombre = Console.ReadLine();  
            Console.Write("Teléfono: ");  
            contactos[cantidad].Telefono = Console.ReadLine();  
            Console.Write("Email: ");  
            contactos[cantidad].Email = Console.ReadLine();  
            cantidad++;  
            Console.WriteLine("Agregado.");  
        }  
        else  
        {  
            Console.WriteLine("Agenda completa. No se puede agregar más contactos.");  
        }  
    }  

    static void Modificar()  
    {  
        Console.Write("ID a modificar: ");  
        if (int.TryParse(Console.ReadLine(), out int id))  
        {  
            for (int i = 0; i < cantidad; i++)  
            {  
                if (contactos[i].Id == id)  
                {  
                    Console.Write("NUEVO nombre: ");  
                    string n = Console.ReadLine();  
                    if (!string.IsNullOrWhiteSpace(n)) contactos[i].Nombre = n;  
                    Console.Write("Nuevo teléfono: ");  
                    n = Console.ReadLine();  
                    if (!string.IsNullOrWhiteSpace(n)) contactos[i].Telefono = n;  
                    Console.Write("Nuevo email: ");  
                    n = Console.ReadLine();  
                    if (!string.IsNullOrWhiteSpace(n)) contactos[i].Email = n;  
                    Console.WriteLine("Modificado.");  
                    return;  
                }  
            }  
            Console.WriteLine("ID no encontrado.");  
        }  
        else  
        {  
            Console.WriteLine("ID inválido.");  
        }  
    }  

    static void Borrar()  
    {  
        Console.Write("ID  borrar: ");  
        if (int.TryParse(Console.ReadLine(), out int id))  
        {  
            for (int i = 0; i < cantidad; i++)  
            {  
                if (contactos[i].Id == id)  
                {  
                    for (int j = i; j < cantidad - 1; j++)  
                        contactos[j] = contactos[j + 1];  
                    cantidad--;  
                    Console.WriteLine("Borrado.");  
                    return;  
                }  
            }  
            Console.WriteLine("ID no encontrado.");  
        }  
        else  
        {  
            Console.WriteLine("ID inválido.");  
        }  
    }  

    static void Listar()  
    {  
        for (int i = 0; i < cantidad; i++)  
        {  
            Console.WriteLine(contactos[i].Id + " - " + contactos[i].Nombre + " - " + contactos[i].Telefono + " - " + contactos[i].Email);  
        }  
    }  

    static void Buscar()  
    {  
        Console.Write("Buscar: ");  
        string busqueda = Console.ReadLine().ToLower();  
        bool encontrado = false;  
        for (int i = 0; i < cantidad; i++)  
        {  
            if (contactos[i].Nombre.ToLower().Contains(busqueda) ||  
                contactos[i].Telefono.ToLower().Contains(busqueda) ||  
                contactos[i].Email.ToLower().Contains(busqueda))  
            {  
                Console.WriteLine(contactos[i].Id + " - " + contactos[i].Nombre + " - " + contactos[i].Telefono + " - " + contactos[i].Email);  
                encontrado = true;  
            }  
        }  
        if (!encontrado)  
        {  
            Console.WriteLine("No se encontraron resultados.");  
        }  
    }  
}  

AgendaScript.Ejecutar();  