using System;
using System.Collections.Generic;
using System.Linq;

class ListaOrdenada<T> where T : IComparable<T>
{
    private readonly List<T> elementos = new List<T>();

    // Constructor por defecto
    public ListaOrdenada() { }

    // Constructor que acepta una colección inicial
    public ListaOrdenada(IEnumerable<T> coleccion)
    {
        foreach (var item in coleccion)
        {
            Agregar(item); // Asegura que los elementos se agreguen ordenados y sin duplicados
        }
    }

    // Método para agregar un elemento, manteniendo la lista ordenada
    public void Agregar(T item)
    {
        if (elementos.Contains(item)) return; // Ignorar duplicados
        int index = elementos.BinarySearch(item); // Buscar posición de inserción
        if (index < 0) index = ~index; // Convertir índice negativo a posición válida
        elementos.Insert(index, item); // Insertar en la posición correcta
    }

    // Método para eliminar un elemento
    public void Eliminar(T item)
    {
        elementos.Remove(item); // Elimina el elemento si existe
    }

    // Método para verificar si contiene un elemento
    public bool Contiene(T item)
    {
        return elementos.Contains(item);
    }

    // Método para filtrar elementos según un criterio
    public ListaOrdenada<T> Filtrar(Func<T, bool> criterio)
    {
        return new ListaOrdenada<T>(elementos.Where(criterio));
    }

    // Propiedad para obtener la cantidad de elementos
    public int Cantidad => elementos.Count;

    // Indexador para acceder a elementos por índice
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= elementos.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Índice fuera de rango");
            return elementos[index];
        }
    }
}

class Contacto : IComparable<Contacto>
{
    public string Nombre { get; set; }
    public string Telefono { get; set; }

    // Constructor de Contacto
    public Contacto(string nombre, string telefono)
    {
        Nombre = nombre;
        Telefono = telefono;
    }

    // Implementación de la comparación para ordenar por nombre
    public int CompareTo(Contacto otro)
    {
        return Nombre.CompareTo(otro.Nombre);
    }

    // Sobrescritura de Equals para comparar contactos por nombre y teléfono
    public override bool Equals(object obj)
    {
        if (obj is Contacto c)
            return Nombre == c.Nombre && Telefono == c.Telefono;
        return false;
    }

    // Sobrescritura de GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(Nombre, Telefono);
    }

    // Representación en texto (opcional)
    public override string ToString()
    {
        return $"{Nombre} - {Telefono}";
    }
}

