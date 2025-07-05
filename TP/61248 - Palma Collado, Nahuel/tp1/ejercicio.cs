using System;
using System.IO;

public static class Consola{
    public static void Limpiar(){
        Clear();
    }
    public static void Escribir(string mensaje){
        WriteLine(mensaje);
    }
    public static string Leer(string texto){
        Write(texto);
        return ReadLine();
    }
    public static void EsperarTecla(string mensaje = "Presione una tecla para continuar..."){
        Write(mensaje);
        ReadKey();
        WriteLine();
    }
}
public struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

public class Agenda
{
    const int MaxContactos = 3;
    static Contacto[] contactos = new Contacto[MaxContactos];
    static int cantidadContactos = 0;
    const string ARCHIVO = "agenda.csv";

    public static void MostrarAgenda()
    {
        CargarContactos();
        while (true)
        {
            Consola.Limpiar();
            Consola.Escribir("------ AGENDA DE CONTACTOS ------");
            Consola.Escribir("1) Agregar contacto");
            Consola.Escribir("2) Modificar contacto");
            Consola.Escribir("3) Borrar contacto");
            Consola.Escribir("4) Listar contactos");
            Consola.Escribir("5) Buscar contacto");
            Consola.Escribir("0) Salir");
            string opcion = Consola.Leer("Seleccione una opción: ");

            if (opcion == "1") AgregarContacto();
            else if (opcion == "2") ModificarContacto();
            else if (opcion == "3") BorrarContacto();
            else if (opcion == "4") ListarContactos();
            else if (opcion == "5") BuscarContacto();
            else if (opcion == "0") 
            { 
                Consola.Escribir("Saliendo de la agenda...");
                Consola.Escribir("Guardando contactos en el archivo...");
                GuardarContactos(); break; 
            }
        }
    }

    static void AgregarContacto()
    {
        if (cantidadContactos >= MaxContactos)
        {
            Consola.Escribir("La agenda está llena.");
            return;
        }

        Contacto nuevo;
        nuevo.Id = cantidadContactos + 1;
        nuevo.Nombre = Consola.Leer("Nombre: ");
        nuevo.Telefono = Consola.Leer("Teléfono: ");
        nuevo.Email = Consola.Leer("Email: ");

        contactos[cantidadContactos] = nuevo;
        cantidadContactos++;
        Consola.Escribir("Contacto agregado con ID = " + nuevo.Id);
        Consola.Escribir("-------------------------------");
        Consola.EsperarTecla();
    }

    static void ModificarContacto()
    {
        int id = int.Parse(Consola.Leer("Ingrese el ID del contacto a modificar: "));

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                Consola.Escribir($"Datos actuales => Nombre: {contactos[i].Nombre}, Teléfono: {contactos[i].Telefono}, Email: {contactos[i].Email}");
                string nombre = Consola.Leer("Nombre (Presione Enter para no cambiar): ");
                if (nombre != "") contactos[i].Nombre = nombre;

                string telefono = Consola.Leer("Teléfono (Presione Enter para no cambiar): ");
                if (telefono != "") contactos[i].Telefono = telefono;

                string email = Consola.Leer("Email (Presione Enter para no cambiar): ");
                if (email != "") contactos[i].Email = email;

                Consola.Escribir("Contacto modificado con éxito.");
                Consola.EsperarTecla();
                return;
            }
        }
        Consola.Escribir("ID no encontrado.");
        Consola.Escribir("-------------------------------");
        Consola.EsperarTecla();
    }

    static void BorrarContacto()
    {
        int id = int.Parse(Consola.Leer("Ingrese el ID del contacto a borrar: "));

        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                for (int j = i; j < cantidadContactos - 1; j++)
                    contactos[j] = contactos[j + 1];

                cantidadContactos--;
                Consola.Escribir("Contacto eliminado con éxito.");
                Consola.EsperarTecla();
                return;
            }
        }
        Consola.Escribir("ID no encontrado.");
        Consola.Escribir("-------------------------------");
        Consola.EsperarTecla();
    }

    static void ListarContactos()
    {
        Consola.Escribir("------ Lista de Contactos ------");
        Consola.Escribir("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < cantidadContactos; i++)
        {
            Consola.Escribir($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-20}");
        }
        Consola.Escribir("-------------------------------");
        Consola.EsperarTecla();
    }

    static void BuscarContacto()
    {
        int id = int.Parse(Consola.Leer("Ingrese el ID del contacto a buscar: "));

        Consola.Escribir("Resultados de la búsqueda:");
        Consola.Escribir("ID    NOMBRE               TELÉFONO       EMAIL");
        for (int i = 0; i < cantidadContactos; i++)
        {
            if (contactos[i].Id == id)
            {
                Consola.Escribir($"{contactos[i].Id,-5} {contactos[i].Nombre,-20} {contactos[i].Telefono,-15} {contactos[i].Email,-20}");
                Consola.Escribir("-------------------------------");
                Consola.EsperarTecla();
                return;
            }
        }
        Consola.Escribir("ID no encontrado.");
        Consola.Escribir("-------------------------------");
        Consola.EsperarTecla();
    }

    static void CargarContactos()
    {
        if (File.Exists(ARCHIVO))
        {
            string[] lineas = File.ReadAllLines(ARCHIVO);
            foreach (string linea in lineas)
            {
                string[] partes = linea.Split(',');
                if (partes.Length == 4)
                {
                    Contacto c;
                    c.Id = int.Parse(partes[0]);
                    c.Nombre = partes[1];
                    c.Telefono = partes[2];
                    c.Email = partes[3];
                    contactos[cantidadContactos++] = c;
                }
            }
        }
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(ARCHIVO))
        {
            for (int i = 0; i < cantidadContactos; i++)
            {
                sw.WriteLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
            }
        }
    }
}

Clear();
Agenda.MostrarAgenda();