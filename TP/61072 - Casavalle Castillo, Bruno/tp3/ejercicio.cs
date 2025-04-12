using System;
using System.Collections.Generic;

public class ListaOrdenada<T> where T : IComparable<T>
{
    private List<T> elementos = new List<T>();

    public ListaOrdenada(IEnumerable<T> elementosIniciales)
    {
        foreach (var elemento in elementosIniciales)
        {
            Agregar(elemento);
        }
    }

    public ListaOrdenada() { }

    public void Agregar(T elemento)
    {
        if (!elementos.Contains(elemento))
        {
            elementos.Add(elemento);
            elementos.Sort();
        }
    }

    public bool Contiene(T elemento)
    {
        return elementos.Contains(elemento);
    }

    public void Eliminar(T elemento)
    {
        elementos.Remove(elemento);
    }

    public int Cantidad => elementos.Count;

    public T this[int index] => elementos[index];

    public ListaOrdenada<T> Filtrar(Predicate<T> condicion)
    {
        ListaOrdenada<T> nuevaLista = new ListaOrdenada<T>();
        foreach (var elemento in elementos)
        {
            if (condicion(elemento))
            {
                nuevaLista.Agregar(elemento);
            }
        }
        return nuevaLista;
    }
}

public class Contacto : IComparable<Contacto>
{
    public string Nombre { get; set; }
    public string Telefono { get; set; }

    public Contacto(string nombre, string telefono)
    {
        Nombre = nombre;
        Telefono = telefono;
    }

    public int CompareTo(Contacto otro)
    {
        return Nombre.CompareTo(otro.Nombre);
    }

    public override bool Equals(object obj)
    {
        return obj is Contacto contacto && Nombre == contacto.Nombre;
    }

    public override int GetHashCode()
    {
        return Nombre.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Nombre}: {Telefono}";
    }
}

class Program
{
    // Funcion auxiliar para pruebas
    static void Assert<T>(T real, T esperado, string mensaje)
    {
        if (!Equals(esperado, real)) throw new Exception($"[ASSERT FALLÓ] {mensaje} → Esperado: {esperado}, Real: {real}");
        Console.WriteLine($"[OK] {mensaje}");
    }

    static void Main()
    {
        // Pruebas con enteros
        var lista = new ListaOrdenada<int>();
        lista.Agregar(5);
        lista.Agregar(1);
        lista.Agregar(3);

        Assert(lista[0], 1, "Primer elemento");
        Assert(lista[1], 3, "Segundo elemento");
        Assert(lista[2], 5, "Tercer elemento");
        Assert(lista.Cantidad, 3, "Cantidad de elementos");

        Assert(lista.Filtrar(x => x > 2).Cantidad, 2, "Cantidad de elementos filtrados");
        Assert(lista.Filtrar(x => x > 2)[0], 3, "Primer elemento filtrado");
        Assert(lista.Filtrar(x => x > 2)[1], 5, "Segundo elemento filtrado");

        Assert(lista.Contiene(1), true, "Contiene");
        Assert(lista.Contiene(2), false, "No contiene");

        lista.Agregar(3);
        Assert(lista.Cantidad, 3, "Cantidad tras duplicado");

        lista.Agregar(2);
        Assert(lista.Cantidad, 4, "Cantidad tras agregar 2");
        Assert(lista[0], 1, "Elemento 0 tras 2");
        Assert(lista[1], 2, "Elemento 1 tras 2");
        Assert(lista[2], 3, "Elemento 2 tras 2");

        lista.Eliminar(2);
        Assert(lista.Cantidad, 3, "Cantidad tras eliminar 2");
        Assert(lista[0], 1, "Elemento 0 tras eliminar 2");
        Assert(lista[1], 3, "Elemento 1 tras eliminar 2");

        lista.Eliminar(100);
        Assert(lista.Cantidad, 3, "Cantidad tras eliminar inexistente");

        // Pruebas con strings
        var nombres = new ListaOrdenada<string>(new[] { "Juan", "Pedro", "Ana" });
        Assert(nombres.Cantidad, 3, "Cantidad nombres");

        Assert(nombres[0], "Ana", "Nombre 0");
        Assert(nombres[1], "Juan", "Nombre 1");
        Assert(nombres[2], "Pedro", "Nombre 2");

        Assert(nombres.Filtrar(x => x.StartsWith("A")).Cantidad, 1, "Nombres A");
        Assert(nombres.Filtrar(x => x.Length > 3).Cantidad, 2, "Nombres > 3");

        Assert(nombres.Contiene("Ana"), true, "Contiene Ana");
        Assert(nombres.Contiene("Domingo"), false, "No contiene Domingo");

        nombres.Agregar("Pedro");
        Assert(nombres.Cantidad, 3, "Cantidad tras duplicado Pedro");

        nombres.Agregar("Carlos");
        Assert(nombres.Cantidad, 4, "Cantidad tras agregar Carlos");

        Assert(nombres[0], "Ana", "Nombre 0 tras Carlos");
        Assert(nombres[1], "Carlos", "Nombre 1 tras Carlos");

        nombres.Eliminar("Carlos");
        Assert(nombres.Cantidad, 3, "Cantidad tras eliminar Carlos");
        Assert(nombres[0], "Ana", "Nombre 0 tras eliminar Carlos");

        nombres.Eliminar("Domingo");
        Assert(nombres.Cantidad, 3, "Cantidad tras eliminar inexistente");

        // Pruebas con contactos
        var juan = new Contacto("Juan", "123456");
        var pedro = new Contacto("Pedro", "654321");
        var ana = new Contacto("Ana", "789012");
        var otro = new Contacto("Otro", "345678");

        var contactos = new ListaOrdenada<Contacto>(new[] { juan, pedro, ana });
        Assert(contactos.Cantidad, 3, "Cantidad contactos");
        Assert(contactos[0].Nombre, "Ana", "Contacto 0");
        Assert(contactos[1].Nombre, "Juan", "Contacto 1");

        Assert(contactos.Filtrar(x => x.Nombre.StartsWith("A")).Cantidad, 1, "Contactos A");
        Assert(contactos.Filtrar(x => x.Nombre.Contains("a")).Cantidad, 2, "Contactos con 'a'");

        Assert(contactos.Contiene(juan), true, "Contiene Juan");
        Assert(contactos.Contiene(otro), false, "No contiene Otro");

        contactos.Agregar(otro);
        Assert(contactos.Cantidad, 4, "Cantidad tras agregar Otro");

        Assert(contactos[2].Nombre, "Otro", "Contacto 2 tras Otro");

        contactos.Eliminar(otro);
        Assert(contactos.Cantidad, 3, "Cantidad tras eliminar Otro");
        Assert(contactos[1].Nombre, "Juan", "Contacto 1 tras eliminar Otro");

        contactos.Eliminar(otro);
        Assert(contactos.Cantidad, 3, "Cantidad tras eliminar Otro de nuevo");
    }
}