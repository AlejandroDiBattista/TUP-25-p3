using System;
using System.Collections.Generic;

class ListaOrdenada<T> where T : IComparable<T>
{
    private List<T> elementos = new List<T>();

    public int Cantidad => elementos.Count;

    public T this[int indice] => elementos[indice];

    public ListaOrdenada() { }
    public ListaOrdenada(IEnumerable<T> coleccion)
    {
        foreach (var item in coleccion)
            Agregar(item);
    }

    public bool Contiene(T elemento) => elementos.Contains(elemento);

    public void Agregar(T elemento)
    {
        if (Contiene(elemento)) return; // Ignora duplicados

        int posicion = elementos.BinarySearch(elemento);
        if (posicion < 0) posicion = ~posicion; 
        elementos.Insert(posicion, elemento);
    }

    public void Eliminar(T elemento)
    {
        if (Contiene(elemento))
            elementos.Remove(elemento);
    }

    public ListaOrdenada<T> Filtrar(Func<T, bool> condicion)
    {
        var listaFiltrada = new ListaOrdenada<T>();
        foreach (var item in elementos)
        {
            if (condicion(item))
                listaFiltrada.Agregar(item);
        }
        return listaFiltrada;
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

    public int CompareTo(Contacto otro) => Nombre.CompareTo(otro.Nombre);
}

/// --------------------------------------------------------///
///   Desde aca para abajo no se puede modificar el código  ///
/// --------------------------------------------------------///

/// 
/// PRUEBAS AUTOMATIZADAS
///

// Funcion auxiliar para las pruebas
public static void Assert<T>(T real, T esperado, string mensaje){
    if (!Equals(esperado, real)) throw new Exception($"[ASSERT FALLÓ] {mensaje} → Esperado: {esperado}, Real: {real}");
    Console.WriteLine($"[OK] {mensaje}");
}


/// Pruebas de lista ordenada (con enteros)

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

Assert(lista.Contiene(1), true,  "Contiene");
Assert(lista.Contiene(2), false, "No contiene");

lista.Agregar(3);
Assert(lista.Cantidad, 3, "Cantidad de elementos tras agregar un elemento repetido");

lista.Agregar(2);
Assert(lista.Cantidad, 4, "Cantidad de elementos tras agregar 2");
Assert(lista[0], 1, "Primer elemento tras agregar 2");
Assert(lista[1], 2, "Segundo elemento tras agregar 2");
Assert(lista[2], 3, "Tercer elemento tras agregar 2");

lista.Eliminar(2);
Assert(lista.Cantidad, 3, "Cantidad de elementos tras eliminar elemento existente");
Assert(lista[0], 1, "Primer elemento tras eliminar 2");
Assert(lista[1], 3, "Segundo elemento tras eliminar 2");
lista.Eliminar(100);
Assert(lista.Cantidad, 3, "Cantidad de elementos tras eliminar elemento inexistente");



/// Pruebas de lista ordenada (con cadenas)

var nombres = new ListaOrdenada<string>(new string[] { "Juan", "Pedro", "Ana" });
Assert(nombres.Cantidad, 3, "Cantidad de nombres");

Assert(nombres[0], "Ana", "Primer nombre");
Assert(nombres[1], "Juan", "Segundo nombre");
Assert(nombres[2], "Pedro", "Tercer nombre");

Assert(nombres.Filtrar(x => x.StartsWith("A")).Cantidad, 1, "Cantidad de nombres que empiezan con A");
Assert(nombres.Filtrar(x => x.Length > 3).Cantidad, 2, "Cantidad de nombres con más de 3 letras");

Assert(nombres.Contiene("Ana"), true, "Contiene Ana");
Assert(nombres.Contiene("Domingo"), false, "No contiene Domingo");

nombres.Agregar("Pedro");
Assert(nombres.Cantidad, 3, "Cantidad de nombres tras agregar Pedro nuevamente");

nombres.Agregar("Carlos");
Assert(nombres.Cantidad, 4, "Cantidad de nombres tras agregar Carlos");

Assert(nombres[0], "Ana", "Primer nombre tras agregar Carlos");
Assert(nombres[1], "Carlos", "Segundo nombre tras agregar Carlos");

nombres.Eliminar("Carlos");
Assert(nombres.Cantidad, 3, "Cantidad de nombres tras agregar Carlos");

Assert(nombres[0], "Ana", "Primer nombre tras eliminar Carlos");
Assert(nombres[1], "Juan", "Segundo nombre tras eliminar Carlos");

nombres.Eliminar("Domingo");
Assert(nombres.Cantidad, 3, "Cantidad de nombres tras eliminar un elemento inexistente");

Assert(nombres[0], "Ana", "Primer nombre tras eliminar Domingo");
Assert(nombres[1], "Juan", "Segundo nombre tras eliminar Domingo");


