using System;       
using System.ComponentModel.Design;
using System.Data.Common;
using System.IO;    
using System.Runtime;
using static System.Console;

/* Notas propias:
        - Estudiar archivos .csv 
        - Repasar struct y class
        - Para crear archivos .cs en VSC: 1) Ctrl + ñ
                                          2) cd OneDrive/Desktop
                                          3) mkdir console
                                          4) cd console
                                          5) dotnet new console                      
*/

namespace TP1
{
    public struct Datos
    {
        public string nombre;
        public int telefono;
        public string email;
        public int id;
    }

    public class Constructores
    {
        //Variables globales.
        Datos[] dat = new Datos[100];           //Se hace un vector/array al struct para la acumulacion de datos
        int opc;                                //Opcion del switch 
        int cantCont = 0;                       //Cantidad de contactos almacenados
        int contID = 1;                         //Contador de ID
        string archivo = "agenda.csv";

        public Constructores()
        {
            CargarDesdeArchivo();
        }

        public void Menu()
        {
            do{
                Clear();
                WriteLine("===== AGENDA DE CONTACTOS =====");
                WriteLine("1) Agregar contacto");
                WriteLine("2) Modificar contacto");    
                WriteLine("3) Borrar contacto");
                WriteLine("4) Listar contactos"); 
                WriteLine("5) Buscar contacto");
                WriteLine("0) Salir");
                Write("Seleccione una opción: "); 
                
                if(int.TryParse(ReadLine(), out opc)) //Convierte en int al OPC, lo ingresado en la consola se guarda en opc. Si es numero se selecciona la opcion, si es una letra vuelve al menu principal.
                {
                    switch(opc){
                        case 0:
                            WriteLine("Presione una tecla para cerrar.");
                            ReadKey();
                            Clear();
                            GuardarEnArchivo();
                            return;

                        case 1:
                            Agregar();
                            break;

                        case 2:
                            Modificar();
                            break;

                        case 3:
                            Borrar();
                            break;

                        case 4:
                            Listar();
                            break;

                        case 5:
                            Busqueda();
                            break;
                        
                        default:
                            ForegroundColor = ConsoleColor.Red;
                            WriteLine("ERROR: Opcion no valida.");
                            ResetColor();
                            break;
                    }
                }
                else
                {
                    WriteLine("Entrada no válida, ingrese un número.");
                }

            }while(opc != 6);
        }

        public void Agregar()
        {   
            WriteLine("=== Agregar Contacto ===");
            if(cantCont >= dat.Length)          //En el hipotetico caso que supere el tamaño del vector.
            {                                      
                WriteLine("La agenda está llena, elimine algun contacto.");
                return;
            }

            Datos contNew;
            contNew.id = contID++;

            Write("Nombre   : ");
            contNew.nombre = ReadLine();
            Write("Telefono : ");
            contNew.telefono = Convert.ToInt32(ReadLine());
            Write("Email    : ");
            contNew.email = ReadLine();

            dat[cantCont] = contNew; 
            cantCont++;

            WriteLine($"Contacto agregado con ID = {contNew.id}");
            WriteLine("Presione una tecla para continuar...");
            ReadKey();
            Clear();
        }

        public void Listar()
        {
            WriteLine("=== Lista de Contactos ===");
            WriteLine("ID\t\tNOMBRE\t\tTELEFONO\t\tEMAIL");
            for(int i = 0; i < cantCont; i++)
            {
                WriteLine($"{dat[i].id}\t\t{dat[i].nombre}\t\t{dat[i].telefono}\t\t\t{dat[i].email}");
            }
            WriteLine("\nPresione una tecla para continuar...");    
            ReadKey();
            Clear();
        }

        public void Busqueda(){
            WriteLine("=== Buscar Contacto ===");
            Write("Ingrese un término de búsqueda (nombre, teléfono o email): ");
            string busc = ReadLine().ToLower();    //Pasamos todo a minuscula para evitar problemas con mayusculas
            bool encontrado = false;

            WriteLine("\nID\t\tNOMBRE\t\tTELEFONO\t\tEMAIL");

            for(int i = 0; i < cantCont; i++){
                if(dat[i].nombre.ToLower().Contains(busc) || dat[i].email.ToLower().Contains(busc) || dat[i].telefono.ToString().Contains(busc))    //Ayuda de CHATGPT, buscar bien para que sirve Contains. Originalmente se puso: if(dat[i].nombre.ToLower() || dat[i].email.ToLower() || dat[i].telefono)
                {
                    encontrado = true;
                    WriteLine($"{dat[i].id}\t\t{dat[i].nombre}\t\t{dat[i].telefono}\t\t\t{dat[i].email}");
                }
            }

            if(!encontrado)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("No se encontraron coincidencias.");
                ResetColor();
            }
            WriteLine("\nPresione una tecla para continuar...");
            ReadKey();
            Clear();
        }

        public void Modificar()
        {
            WriteLine("=== Modificar Contacto ===");
            Write("Ingrese el ID del contacto a modificar (Deje el campo en blanco para no modificar): ");
            int id = Convert.ToInt32(ReadLine());
            
                for(int i = 0; i < cantCont; i++)
                {
                    if(dat[i].id == id)
                    {
                        Write("Nuevo Nombre    : ");
                        string nombre = ReadLine();
                        dat[i].nombre = string.IsNullOrWhiteSpace(nombre) ? dat[i].nombre : nombre;         //El operador ternario (? :) para evitar modificar los valores si el usuario deja el campo vacío. Sacado con Chatgpt, version original: if(!string.IsNullOrWhiteSpace(nombre)) dat[i].nombre = nombre;

                        Write("Nuevo Teléfono  : ");
                        string telefono = ReadLine();
                        dat[i].telefono = int.TryParse(telefono, out int tel) ? tel : dat[i].telefono;

                        Write("Nuevo Email     : ");
                        string email = ReadLine();
                        dat[i].email = string.IsNullOrWhiteSpace(email) ? dat[i].email : email;

                        ForegroundColor = ConsoleColor.Green;
                        WriteLine("Contacto modificado con éxito.");
                        ResetColor();
                        break;
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine("ERROR: ID inválido.");
                        ResetColor();
                    }
                }
            
            

            WriteLine("Presione cualquier tecla para continuar...");
            ReadKey();
            Clear();
        }

        public void Borrar()
        {
            
            WriteLine("=== Borrar Contacto ===");
            Write("Ingrese el ID del contacto a borrar: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                int index = -1;                                 //Inicializado en -1 para no causar problemas si el ID existe
                for(int i = 0; i < cantCont; i++)
                {
                    if(id == dat[i].id)
                    {
                        index = i;
                        break;
                    }
                }
                if(index != -1)
                {
                    for(int i = 0; i < cantCont - 1; i++)
                    {
                        dat[i] = dat[i + 1];                    //Sacado del metodo burbuja para mover los elementos a la izquierda
                    }
                    cantCont--;                                 //Reduccion de cantidad de contactos guardados
                    WriteLine($"La ID {id} fue eliminada.");
                } 
                else WriteLine("Contacto inexistente.");
            }
            else
            {
                WriteLine("ID inválido.");
            }

            WriteLine("Presione cualquier tecla para continuar...");
            ReadKey();
            Clear();
        }
        private void CargarDesdeArchivo()
        {
            if (File.Exists(archivo))
            {
                string[] lineas = File.ReadAllLines(archivo);
                foreach (string linea in lineas)
                {
                    string[] datos = linea.Split(',');
                    if (datos.Length == 4 && int.TryParse(datos[1], out int telefono) && int.TryParse(datos[3], out int id))
                    {
                        dat[cantCont++] = new Datos { nombre = datos[0], telefono = telefono, email = datos[2], id = id };
                        contID = Math.Max(contID, id + 1);
                    }
                }
            }
        }

        private void GuardarEnArchivo()
        {
            string[] lineas = new string[cantCont];
            for (int i = 0; i < cantCont; i++)
            {
                lineas[i] = $"{dat[i].nombre}, {dat[i].telefono}, {dat[i].email}, {dat[i].id}";
            }
            File.WriteAllLines(archivo, lineas);
        }
    }

    
    internal class Principal
    {
        static void Main(string[] args)
        {
            Constructores cons = new Constructores();
            cons.Menu();
        }
    }
}