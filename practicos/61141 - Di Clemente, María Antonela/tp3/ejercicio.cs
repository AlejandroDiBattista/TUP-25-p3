using System;
using System.Collections.Generic;

class ListaOrdenada<T> where T : IComparable<T>
{
    private List<T> elementos = new List<T>();

    public ListaOrdenada() { }

    public ListaOrdenada(IEnumerable<T> coleccion)
    {
        foreach (var item in coleccion)
        {
            Agregar(item);
        }
    }

    public int Cantidad => elementos.Count;

    public T this[int indice] => elementos[indice];

    public void Agregar(T valor)
    {
        if (Contiene(valor)) return;

        int i = 0;
        while (i < elementos.Count && elementos[i].CompareTo(valor) < 0)
        {
            i++;
        }
        elementos.Insert(i, valor);
    }

    public bool Contiene(T valor)
    {
        return elementos.Contains(valor);
    }

    public void Eliminar(T valor)
    {
        elementos.Remove(valor);
    }

    public ListaOrdenada<T> Filtrar(Func<T, bool> condicion)
    {
        var nueva = new ListaOrdenada<T>();
        foreach (var item in elementos)
        {
            if (condicion(item))
            {
                nueva.Agregar(item);
            }
        }
        return nueva;
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
        return this.Nombre.CompareTo(otro.Nombre);
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

class Program
{
    public static void Assert<T>(T real, T esperado, string mensaje)
    {
        if (!Equals(esperado, real))
            throw new Exception($"[ASSERT FALLÓ] {mensaje} → Esperado: {esperado}, Real: {real}");
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
        Assert(lista.Cantidad, 3, "Cantidad tras agregar repetido");

        lista.Agregar(2);
        Assert(lista.Cantidad, 4, "Cantidad tras agregar 2");
        Assert(lista[0], 1, "1er tras agregar 2");
        Assert(lista[1], 2, "2do tras agregar 2");
        Assert(lista[2], 3, "3ro tras agregar 2");

        lista.Eliminar(2);
        Assert(lista.Cantidad, 3, "Cantidad tras eliminar 2");
        Assert(lista[0], 1, "1er tras eliminar 2");
        Assert(lista[1], 3, "2do tras eliminar 2");
        lista.Eliminar(100);
        Assert(lista.Cantidad, 3, "Eliminar inexistente");

        // Pruebas con strings
        var nombres = new ListaOrdenada<string>(new string[] { "Juan", "Pedro", "Ana" });
        Assert(nombres.Cantidad, 3, "Cantidad nombres");

        Assert(nombres[0], "Ana", "1er nombre");
        Assert(nombres[1], "Juan", "2do nombre");
        Assert(nombres[2], "Pedro", "3er nombre");

        Assert(nombres.Filtrar(x => x.StartsWith("A")).Cantidad, 1, "Empiezan con A");
        Assert(nombres.Filtrar(x => x.Length > 3).Cantidad, 2, "Nombres > 3 letras");

        Assert(nombres.Contiene("Ana"), true, "Contiene Ana");
        Assert(nombres.Contiene("Domingo"), false, "No contiene Domingo");

        nombres.Agregar("Pedro");
        Assert(nombres.Cantidad, 3, "No agrega repetido Pedro");

        nombres.Agregar("Carlos");
        Assert(nombres.Cantidad, 4, "Agrega Carlos");

        Assert(nombres[0], "Ana", "1er tras agregar Carlos");
        Assert(nombres[1], "Carlos", "2do tras agregar Carlos");

        nombres.Eliminar("Carlos");
        Assert(nombres.Cantidad, 3, "Cantidad tras eliminar Carlos");
        Assert(nombres[0], "Ana", "1er tras eliminar Carlos");
        Assert(nombres[1], "Juan", "2do tras eliminar Carlos");

        nombres.Eliminar("Domingo");
        Assert(nombres.Cantidad, 3, "Eliminar inexistente Domingo");
        Assert(nombres[0], "Ana", "1er tras eliminar Domingo");
        Assert(nombres[1], "Juan", "2do tras eliminar Domingo");

        // Pruebas con contactos
        var juan = new Contacto("Juan", "123456");
        var pedro = new Contacto("Pedro", "654321");
        var ana = new Contacto("Ana", "789012");
        var otro = new Contacto("Otro", "345678");

        var contactos = new ListaOrdenada<Contacto>(new Contacto[] { juan, pedro, ana });
        Assert(contactos.Cantidad, 3, "Cantidad contactos");

        Assert(contactos[0].Nombre, "Ana", "1er contacto");
        Assert(contactos[1].Nombre, "Juan", "2do contacto");
        Assert(contactos[2].Nombre, "Pedro", "3er contacto");

        Assert(contactos.Filtrar(x => x.Nombre.StartsWith("A")).Cantidad, 1, "Contactos con A");
        Assert(contactos.Filtrar(x => x.Nombre.Contains("a")).Cantidad, 2, "Contienen 'a'");

        Assert(contactos.Contiene(juan), true, "Contiene Juan");
        Assert(contactos.Contiene(otro), false, "No contiene Otro");

        contactos.Agregar(otro);
        Assert(contactos.Cantidad, 4, "Cantidad tras agregar Otro");
        Assert(contactos.Contiene(otro), true, "Contiene Otro");

        Assert(contactos[0].Nombre, "Ana", "1ro tras agregar Otro");
        Assert(contactos[1].Nombre, "Juan", "2do tras agregar Otro");
        Assert(contactos[2].Nombre, "Otro", "3ro tras agregar Otro");
        Assert(contactos[3].Nombre, "Pedro", "4to tras agregar Otro");

        contactos.Eliminar(otro);
        Assert(contactos.Cantidad, 3, "Cantidad tras eliminar Otro");
        Assert(contactos[0].Nombre, "Ana", "1ro tras eliminar Otro");
        Assert(contactos[1].Nombre, "Juan", "2do tras eliminar Otro");
        Assert(contactos[2].Nombre, "Pedro", "3ro tras eliminar Otro");

        contactos.Eliminar(otro);
        Assert(contactos.Cantidad, 3, "Eliminar Otro otra vez");
    }
}
