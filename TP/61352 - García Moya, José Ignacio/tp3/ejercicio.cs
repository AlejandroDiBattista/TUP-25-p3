using System;
using System.Collections.Generic;

class ListaOrdenada<T> where T : IComparable<T> {
    private List<T> elementos = new List<T>();

    public ListaOrdenada() {}

    public ListaOrdenada(IEnumerable<T> coleccion) {
        foreach (var item in coleccion) {
            Agregar(item);
        }
    }

    public int Cantidad => elementos.Count;

    public void Agregar(T elemento) {
        if (elementos.Contains(elemento)) return;

        int index = elementos.BinarySearch(elemento);
        if (index < 0) index = ~index;
        elementos.Insert(index, elemento);
    }

    public bool Contiene(T elemento) {
        return elementos.Contains(elemento);
    }

    public void Eliminar(T elemento) {
        elementos.Remove(elemento);
    }

    public ListaOrdenada<T> Filtrar(Predicate<T> condicion) {
        var nuevaLista = new ListaOrdenada<T>();
        foreach (var item in elementos) {
            if (condicion(item)) {
                nuevaLista.Agregar(item);
            }
        }
        return nuevaLista;
    }

    public T this[int indice] {
        get => elementos[indice];
    }
}

class Contacto : IComparable<Contacto> {
    public string Nombre { get; set; }
    public string Telefono { get; set; }

    public Contacto(string nombre, string telefono) {
        Nombre = nombre;
        Telefono = telefono;
    }

    public int CompareTo(Contacto otro) {
        return Nombre.CompareTo(otro.Nombre);
    }

    public override bool Equals(object obj) {
        if (obj is Contacto otro) {
            return Nombre == otro.Nombre && Telefono == otro.Telefono;
        }
        return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Nombre, Telefono);
    }
}
