using System;
using System.Collections.Generic;

class ListaOrdenada<T> : IEnumerable<T> where T : IComparable<T>
{
    private List<T> elementos = new List<T>();

    // Constructor por defecto
    public ListaOrdenada() { }

    // Constructor que inicializa la lista con una colección dada
    public ListaOrdenada(IEnumerable<T> coleccion)
    {
        foreach (var item in coleccion)
        {
            Agregar(item);
        }
    }

    // Agrega un elemento a la lista en la posición ordenada
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

    // Elimina un elemento de la lista si existe
    public void Eliminar(T item)
    {
        int index = elementos.IndexOf(item);
        if (index != -1)
        {
            elementos.RemoveAt(index);
        }
    }

    // Verifica si un elemento existe en la lista
    public bool Contiene(T item)
    {
        return elementos.Contains(item); // Simplificado usando Contains
    }

    // Filtra los elementos según un criterio dado
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

    // Devuelve la cantidad de elementos en la lista
    public int Cantidad => elementos.Count;

    // Accede a un elemento por índice, con validación de rango
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= elementos.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Índice fuera de rango.");
            return elementos[index];
        }
    }

    // Devuelve todos los elementos como una lista
    public List<T> ObtenerTodos()
    {
        return new List<T>(elementos);
    }

    // Método para representar la lista ordenada como cadena
    public override string ToString()
    {
        return string.Join(", ", elementos);
    }

    // Implementación de IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
        return elementos.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
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
        if (obj is Contacto c)
            return Nombre == c.Nombre && Telefono == c.Telefono;
        return false;
    }

    public override int GetHashCode()
    {
        return Nombre.GetHashCode() ^ Telefono.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Nombre} - {Telefono}";
    }
}