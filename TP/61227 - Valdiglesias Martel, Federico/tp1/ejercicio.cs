using System;     
using System.IO;    


int opcion;
int indice = 0;
int idIncremento = 1;



Contacto auxiliarContacto;
Contacto eliminado;

void OpcionesMenu()
{
  Console.WriteLine("========== Agenda de contacto ==========");
  Console.WriteLine("1) Agregar contacto");
  Console.WriteLine("2) Modificar contacto");
  Console.WriteLine("3) Borrar contacto");
  Console.WriteLine("4) Listar contactos");
  Console.WriteLine("5) Buscar contacto");
  Console.WriteLine("0) Salir");
}

void OpcionAgregar()
{
  Console.WriteLine("======= Agregar contacto =========");
  Console.Write("Nombre: ");
  string nombre = Console.ReadLine();
  Console.Write("Telefono: ");
  string telefono = Console.ReadLine();
  Console.Write("Email: ");
  string email = Console.ReadLine();
  Contacto nuevo = new Contacto(idIncremento, nombre, telefono, email);
  string contactoTexto = $"{nuevo.Nombre},{nuevo.Telefono},{nuevo.Email}";
  File.AppendAllText("agenda.csv", contactoTexto + Environment.NewLine);
  Console.Write("Presione cualquier tecla para continuar...");
  Console.ReadKey();
}

void OpcionModificar()
{
  Console.WriteLine("======= Modificar contacto =========");

  Console.Write("Ingrese el ID del contacto a modificar: ");
  int id = int.Parse(Console.ReadLine());

  Contacto[] contactos = listarContacto("agenda.csv");

  for (int i = 1; i < contactos.Length; i++)
  {
    if (contactos[i].Id == id)
    {
      auxiliarContacto = contactos[i];

      Console.WriteLine($"Datos actuales => Id:{auxiliarContacto.Id}  Nombre: {auxiliarContacto.Nombre}, Telefono: {auxiliarContacto.Telefono}, Email: {auxiliarContacto.Email}");

      break;
    }
  }

  Console.WriteLine("Deje el campo en blanco para no modificar");
  Console.Write("Nombre: ");
  string nombre = Console.ReadLine();
  Console.Write("Telefono: ");
  string telefono = Console.ReadLine();
  Console.Write("Email: ");
  string email = Console.ReadLine();
  auxiliarContacto.Modificar(nombre, telefono, email);

  guardarActualizacion(auxiliarContacto);

  Console.WriteLine("Contacto modificado con éxito");
  Console.Write("Presione cualquier tecla para continuar...");
  Console.ReadKey();

}

public void OpcionListarContacto()
{
  Console.WriteLine("===== Lista de contactos =====");
  Console.WriteLine(" ID      Nombre        Telefono          Email   ");
  Contacto[] contactos = listarContacto("agenda.csv");
  for (int i = 0; i < contactos.Length; i++)
  {
    Console.WriteLine(contactos[i]);
  }
  Console.Write("Presione una tecla para continuar...");
  Console.ReadKey();


}

void OpcionBuscar()
{
  Console.WriteLine("======= Buscar contacto =========");
  Console.Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
  string palabraClave = Console.ReadLine();
  Console.WriteLine("");
  buscarContacto("agenda.csv", palabraClave);
  Console.Write("Presione cualquier tecla para continuar...");
  Console.ReadKey();
}

void OpcionBorrar()
{
  Console.WriteLine("======= Borrar contacto =========");
  Console.WriteLine("");
  Console.WriteLine("Ingrese el ID del contacto a borrar: ");
  int id = int.Parse(Console.ReadLine());
  BorrarContacto(id);
  Console.Write("Presione cualquier tecla para continuar...");
  Console.ReadKey();
}


do
{
  Console.Clear();
  OpcionesMenu();
  Console.Write("Seleccione una opción: ");
  opcion = int.Parse(Console.ReadLine());

  switch (opcion)
  {
    case 1:
      OpcionAgregar();
      break;
    case 2:
      OpcionModificar();
      break;
    case 3:
      OpcionBorrar();
      break;
    case 4:
      OpcionListarContacto();
      break;
    case 5:
      OpcionBuscar();
      break;
    case 0:
      Console.WriteLine("Saliendo de la aplicación....");
      break;
    default:
      Console.WriteLine("La opcion ingresada es incorrecta. Intente nuevamente");
      break;

  }

} while (opcion != 0);


public struct Contacto
{

  public int Id { get; set; }
  public string Nombre { get; set; }
  public string Telefono { get; set; }
  public string Email { get; set; }

  public Contacto(int id)
  {
    Id = id;
  }

  public Contacto(int id, string nombre, string telefono, string email)
  {
    Id = id;
    Nombre = nombre;
    Telefono = telefono;
    Email = email;
  }

  public override string ToString()
  {
    return $" {Id}      {Nombre}      {Telefono}        {Email}";
  }


  public void Modificar(string nombre, string telefono, string email)
  {
    if (nombre != "")
    {
      Nombre = nombre;
    }
    else if (telefono != "")
    {
      Telefono = telefono;
    }
    else if (email != "")
    {
      Email = email;
    }


  }



}

public Contacto[] listarContacto(string ruta)
{
  int idContactos = 1;
  string datos = File.ReadAllText(ruta);

  string[] lineas = datos.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

  Contacto[] datosContactos = new Contacto[lineas.Length - 1];


  for (int i = 1; i < lineas.Length; i++)
  {

    string[] datosSeparados = lineas[i].Split(",");

    Contacto nuevo = new Contacto(i);
    nuevo.Nombre = datosSeparados[0].Trim();
    nuevo.Telefono = datosSeparados[1].Trim();
    nuevo.Email = datosSeparados[2].Trim();
    datosContactos[i - 1] = nuevo;
  }

  return datosContactos;

}

void guardarActualizacion(Contacto contactoActualizado)
{
  Contacto[] listaContactos = listarContacto("agenda.csv");


  for (int i = 1; i < listaContactos.Length; i++)
  {

    if (listaContactos[i].Id == contactoActualizado.Id)
    {
      listaContactos[i].Nombre = contactoActualizado.Nombre;
      listaContactos[i].Telefono = contactoActualizado.Telefono;
      listaContactos[i].Email = contactoActualizado.Email;
      break;
    }
  }

  // 3. Preparar líneas para el CSV
  string[] contactosFormatoCsv = new string[listaContactos.Length + 1]; // +1 para el encabezado

  // Agregar encabezado
  contactosFormatoCsv[0] = "nombre,telefono,email";

  // Agregar contactos
  for (int i = 0; i < listaContactos.Length; i++)
  {
    contactosFormatoCsv[i + 1] = $"{listaContactos[i].Nombre},{listaContactos[i].Telefono},{listaContactos[i].Email}";
  }

  // 4. Guardar en el archivo
  File.WriteAllLines("agenda.csv", contactosFormatoCsv);

}

void buscarContacto(string ruta, string palabraClave)
{

  Contacto[] datosAgenda = listarContacto(ruta);

  for (int i = 0; i < datosAgenda.Length; i++)
  {
    if (datosAgenda[i].Nombre == palabraClave || datosAgenda[i].Telefono == palabraClave || datosAgenda[i].Email == palabraClave)
    {
      Console.WriteLine("Resultado de la busqueda...");
      Console.WriteLine(" ID      Nombre        Telefono          Email   ");
      Console.WriteLine(datosAgenda[i]);
    }
  }
}

void BorrarContacto(int id)
{

  Contacto[] listaContacto = listarContacto("agenda.csv");

  Contacto[] listaActualizada = new Contacto[listaContacto.Length];

  for (int i = 0; i < listaContacto.Length; i++)
  {
    if (listaContacto[i].Id != id)
    {
      listaActualizada[i] = listaContacto[i];
    }
    else
    {
      eliminado = listaContacto[i];
    }
  }

  string[] contactosFormatoCsv = new string[listaActualizada.Length + 1]; // +1 para el encabezado

  // Agregar encabezado
  contactosFormatoCsv[0] = "nombre,telefono,email";

  // Agregar contactos
  for (int i = 0; i < listaActualizada.Length; i++)
  {
    contactosFormatoCsv[i + 1] = $"{listaActualizada[i].Nombre},{listaActualizada[i].Telefono},{listaActualizada[i].Email}";
  }

  // 4. Guardar en el archivo
  File.WriteAllLines("agenda.csv", contactosFormatoCsv);


  Console.WriteLine($"Contacto con ID = {eliminado.Id}, eliminado con éxito");

}
