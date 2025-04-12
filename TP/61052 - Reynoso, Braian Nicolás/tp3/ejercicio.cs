using System;
using System.Collections.Generic;
using System.Linq;


class ListaOrdenada<T> where T : IComparable<T>
{
    private List<T> elementos = new List<T>();

    public ListaOrdenada() { }

    public ListaOrdenada(IEnumerable<T> elementosIniciales)
    {
        foreach (var elemento in elementosIniciales)
        {
            Agregar(elemento);
        }
    }

    public void Agregar(T elemento)
    {
        if (Contiene(elemento)) return;

        int index = elementos.BinarySearch(elemento);
        if (index < 0) index = ~index;
        elementos.Insert(index, elemento);
    }

    public void Eliminar(T elemento)
    {
        elementos.Remove(elemento);
    }

    public bool Contiene(T elemento)
    {
        return elementos.Contains(elemento);
    }

    public int Cantidad => elementos.Count;

    public T this[int indice] => elementos[indice];

    public ListaOrdenada<T> Filtrar(Func<T, bool> condicion)
    {
        var resultado = new ListaOrdenada<T>();
        foreach (var e in elementos)
        {
            if (condicion(e)) resultado.Agregar(e);
        }
        return resultado;
    }
}

class Contacto : IComparable<Contacto>
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
        return string.Compare(this.Nombre, otro.Nombre, StringComparison.Ordinal);
    }

    public override bool Equals(object obj)
    {
        if (obj is Contacto otro)
        {
            return this.Nombre == otro.Nombre && this.Telefono == otro.Telefono;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Nombre, Telefono);
    }
}

// PRUEBAS AUTOMATIZADAS //

public static class Pruebas
{
    public static void Assert<T>(T real, T esperado, string mensaje)
    {
        if (!Equals(esperado, real)) throw new Exception($"[ASSERT FALLÓ] {mensaje} → Esperado: {esperado}, Real: {real}");
        Console.WriteLine($"[OK] {mensaje}");
    }

    public static void Ejecutar()
    {
        // Pruebas de lista ordenada (con enteros)
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
        Assert(lista.Cantidad, 3, "Cantidad tras agregar repetido");

        lista.Agregar(2);
        Assert(lista.Cantidad, 4, "Cantidad tras agregar 2");
        Assert(lista[0], 1, "Orden 1");
        Assert(lista[1], 2, "Orden 2");
        Assert(lista[2], 3, "Orden 3");

        lista.Eliminar(2);
        Assert(lista.Cantidad, 3, "Cantidad tras eliminar 2");
        Assert(lista[0], 1, "Orden tras eliminar 2");
        Assert(lista[1], 3, "Orden tras eliminar 2");

        lista.Eliminar(100);
        Assert(lista.Cantidad, 3, "Cantidad tras eliminar inexistente");

        // Pruebas de lista ordenada (con strings)
        var nombres = new ListaOrdenada<string>(new string[] { "Juan", "Pedro", "Ana" });
        Assert(nombres.Cantidad, 3, "Cantidad de nombres");

        Assert(nombres[0], "Ana", "Primer nombre");
        Assert(nombres[1], "Juan", "Segundo nombre");
        Assert(nombres[2], "Pedro", "Tercer nombre");

        Assert(nombres.Filtrar(x => x.StartsWith("A")).Cantidad, 1, "Nombres que empiezan con A");
        Assert(nombres.Filtrar(x => x.Length > 3).Cantidad, 2, "Nombres con más de 3 letras");

        Assert(nombres.Contiene("Ana"), true, "Contiene Ana");
        Assert(nombres.Contiene("Domingo"), false, "No contiene Domingo");

        nombres.Agregar("Pedro");
        Assert(nombres.Cantidad, 3, "Cantidad tras agregar Pedro repetido");

        nombres.Agregar("Carlos");
        Assert(nombres.Cantidad, 4, "Cantidad tras agregar Carlos");

        Assert(nombres[0], "Ana", "Orden tras agregar Carlos");
        Assert(nombres[1], "Carlos", "Orden tras agregar Carlos");

        nombres.Eliminar("Carlos");
        Assert(nombres.Cantidad, 3, "Cantidad tras eliminar Carlos");

        Assert(nombres[0], "Ana", "Orden tras eliminar Carlos");
        Assert(nombres[1], "Juan", "Orden tras eliminar Carlos");

        nombres.Eliminar("Domingo");
        Assert(nombres.Cantidad, 3, "Cantidad tras eliminar inexistente");

        // Pruebas de lista ordenada (con contactos)
        var juan = new Contacto("Juan", "123456");
        var pedro = new Contacto("Pedro", "654321");
        var ana = new Contacto("Ana", "789012");
        var otro = new Contacto("Otro", "345678");

        var contactos = new ListaOrdenada<Contacto>(new Contacto[] { juan, pedro, ana });
        Assert(contactos.Cantidad, 3, "Cantidad de contactos");
        Assert(contactos[0].Nombre, "Ana", "Primer contacto");
        Assert(contactos[1].Nombre, "Juan", "Segundo contacto");
        Assert(contactos[2].Nombre, "Pedro", "Tercer contacto");

        Assert(contactos.Filtrar(x => x.Nombre.StartsWith("A")).Cantidad, 1, "Contactos que empiezan con A");
        Assert(contactos.Filtrar(x => x.Nombre.Contains("a")).Cantidad, 2, "Contactos que contienen 'a'");

        Assert(contactos.Contiene(juan), true, "Contiene Juan");
        Assert(contactos.Contiene(otro), false, "No contiene Otro");

        contactos.Agregar(otro);
        Assert(contactos.Cantidad, 4, "Cantidad tras agregar Otro");
        Assert(contactos.Contiene(otro), true, "Contiene Otro");

        Assert(contactos[0].Nombre, "Ana", "Orden tras agregar Otro");
        Assert(contactos[1].Nombre, "Juan", "Orden tras agregar Otro");
        Assert(contactos[2].Nombre, "Otro", "Orden tras agregar Otro");
        Assert(contactos[3].Nombre, "Pedro", "Orden tras agregar Otro");

        contactos.Eliminar(otro);
        Assert(contactos.Cantidad, 3, "Cantidad tras eliminar Otro");
        Assert(contactos[0].Nombre, "Ana", "Orden tras eliminar Otro");
        Assert(contactos[1].Nombre, "Juan", "Orden tras eliminar Otro");
        Assert(contactos[2].Nombre, "Pedro", "Orden tras eliminar Otro");

        contactos.Eliminar(otro);
        Assert(contactos.Cantidad, 3, "Cantidad tras eliminar inexistente");
    }
}

        Pruebas.Ejecutar();
        Console.WriteLine("¡Todas las pruebas pasaron correctamente!");
    

