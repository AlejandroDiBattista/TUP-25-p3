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

    public void Agregar(T item)
    {
        if (Contiene(item)) return;

        int index = 0;
        while (index < elementos.Count && elementos[index].CompareTo(item) < 0)
        {
            index++;
        }
        elementos.Insert(index, item);
    }

    public void Eliminar(T item)
    {
        for (int i = 0; i < elementos.Count; i++)
        {
            if (elementos[i].Equals(item))
            {
                elementos.RemoveAt(i);
                break;
            }
        }
    }

    public bool Contiene(T item)
    {
        foreach (var elem in elementos)
        {
            if (elem.Equals(item)) return true;
        }
        return false;
    }

    public ListaOrdenada<T> Filtrar(Func<T, bool> criterio)
    {
        var resultado = new ListaOrdenada<T>();
        foreach (var elem in elementos)
        {
            if (criterio(elem))
            {
                resultado.Agregar(elem);
            }
        }
        return resultado;
    }

    public int Cantidad => elementos.Count;

    public T this[int index] => elementos[index];
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
        if (obj is Contacto c)
            return Nombre == c.Nombre && Telefono == c.Telefono;
        return false;
    }

    public override int GetHashCode()
    {
        return Nombre.GetHashCode() ^ Telefono.GetHashCode();
    }
}
