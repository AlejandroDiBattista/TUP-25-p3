using System;
using System.Collections.Generic;
using System.Linq;

public class ListaOrdenada<T> where T : IComparable<T>
{
    private List<T> elementos;

    public ListaOrdenada()
    {
        elementos = new List<T>();
    }

    public bool Contiene(T elemento)
    {
        return elementos.Contains(elemento);
    }

    public void Agregar(T elemento)
    {
        if (Contiene(elemento))
            return;

        int posicion = 0;
        while (posicion < elementos.Count && elementos[posicion].CompareTo(elemento) < 0)
        {
            posicion++;
        }
        elementos.Insert(posicion, elemento);
    }

    public void Eliminar(T elemento)
    {
        elementos.Remove(elemento);
    }

    public int Cantidad
    {
        get { return elementos.Count; }
    }

    public T this[int indice]
    {
        get
        {
            if (indice < 0 || indice >= elementos.Count)
                throw new IndexOutOfRangeException("Índice fuera de rango");
            return elementos[indice];
        }
    }

    public ListaOrdenada<T> Filtrar(Func<T, bool> condicion)
    {
        ListaOrdenada<T> listaFiltrada = new ListaOrdenada<T>();
        foreach (T elemento in elementos)
        {
            if (condicion(elemento))
            {
                listaFiltrada.Agregar(elemento);
            }
        }
        return listaFiltrada;
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

    public int CompareTo(Contacto? other)
    {
        if (other == null) return 1;
        return string.Compare(this.Nombre, other.Nombre, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return $"{Nombre} - {Telefono}";
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        
        Contacto other = (Contacto)obj;
        return string.Equals(Nombre, other.Nombre, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Nombre?.ToLower().GetHashCode() ?? 0;
    }
}
