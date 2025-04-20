using System;
using System.Collections.Generic;

class ListaOrdenada<T> where T : IComparable<T> {
    // Clase interna para los nodos de la lista enlazada
    private class Nodo {
        public T Valor { get; set; }
        public Nodo Siguiente { get; set; }

        public Nodo(T valor) {
            Valor = valor;
            Siguiente = null;
        }
    }

    private Nodo primerNodo;
    private int cantidad;

    // Constructor por defecto
    public ListaOrdenada() {
        primerNodo = null;
        cantidad = 0;
    }

    // Constructor que inicializa con una colección de elementos
    public ListaOrdenada(IEnumerable<T> elementos) : this() { // Llama al constructor por defecto
        foreach (var elemento in elementos) {
            Agregar(elemento);
        }
    }

    // Propiedad para obtener la cantidad de elementos
    public int Cantidad => cantidad;

    // Indexador para acceder a los elementos por índice
    public T this[int index] {
        get {
            if (index < 0 || index >= cantidad) {
                throw new IndexOutOfRangeException("El índice está fuera del rango de la lista.");
            }
            Nodo actual = primerNodo!; // Sabemos que no es null si index es válido y cantidad > 0
            for (int i = 0; i < index; i++) {
                actual = actual.Siguiente!; // Sabemos que no será null por la validación del índice
            }
            return actual.Valor;
        }
    }

    // Agrega un elemento manteniendo el orden y sin duplicados
    public void Agregar(T elemento) {
        Nodo nuevoNodo = new Nodo(elemento);
        Nodo actual    = primerNodo;
        Nodo anterior  = null;

        // Buscar la posición correcta para insertar o si ya existe
        while (actual != null && elemento.CompareTo(actual.Valor) > 0) {
            anterior = actual;
            actual   = actual.Siguiente;
        }

        // Verificar si el elemento ya existe
        if (actual != null && elemento.CompareTo(actual.Valor) == 0) {
            return; // Elemento duplicado, no hacer nada
        }

        // Insertar el nuevo nodo
        if (anterior == null) {
            // Insertar al principio
            nuevoNodo.Siguiente = primerNodo;
            primerNodo = nuevoNodo;
        } else {
            // Insertar en medio o al final
            nuevoNodo.Siguiente = actual;
            anterior.Siguiente  = nuevoNodo;
        }
        cantidad++;
    }

    // Elimina un elemento de la lista
    public void Eliminar(T elemento) {
        Nodo actual   = primerNodo;
        Nodo anterior = null;

        // Buscar el elemento a eliminar
        while (actual != null && elemento.CompareTo(actual.Valor) > 0) {
            anterior = actual;
            actual   = actual.Siguiente;
        }

        // Verificar si el elemento se encontró
        if (actual != null && elemento.CompareTo(actual.Valor) == 0) {
            // Eliminar el nodo
            if (anterior == null) {
                // Eliminar el primer nodo
                primerNodo = actual.Siguiente;
            } else {
                // Eliminar un nodo intermedio o el último
                anterior.Siguiente = actual.Siguiente;
            }
            cantidad--;
        }
        // Si no se encontró (actual es null o el valor no coincide), no hacer nada.
    }

    // Verifica si un elemento está contenido en la lista
    public bool Contiene(T elemento) {
        Nodo actual = primerNodo;

        // Buscar el elemento aprovechando el orden
        while (actual != null && elemento.CompareTo(actual.Valor) > 0) {
            actual = actual.Siguiente;
        }

        // Verificar si el elemento actual es el buscado
        return actual != null && elemento.CompareTo(actual.Valor) == 0;
    }

    // Devuelve una nueva ListaOrdenada con los elementos que cumplen el predicado
    public ListaOrdenada<T> Filtrar(Func<T, bool> predicado) {
        var resultado = new ListaOrdenada<T>();
        Nodo actual = primerNodo;
        Nodo ultimoResultado = null; // Para optimizar la inserción en la lista resultado

        while (actual != null) {
            if (predicado(actual.Valor)) {
                // Crear nodo directamente en la lista resultado para eficiencia
                Nodo nuevoNodoResultado = new Nodo(actual.Valor);
                if (resultado.primerNodo == null) {
                    resultado.primerNodo = nuevoNodoResultado;
                } else {
                    ultimoResultado!.Siguiente = nuevoNodoResultado; // Sabemos que ultimoResultado no es null aquí
                }
                ultimoResultado = nuevoNodoResultado;
                resultado.cantidad++;
            }
            actual = actual.Siguiente;
        }
        return resultado;
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
        if (otro == null) return 1;
        return string.Compare(this.Nombre, otro.Nombre, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.CompareOptions.IgnoreCase | System.Globalization.CompareOptions.IgnoreNonSpace);
    }

    public override bool Equals(object obj) {
      return obj is Contacto otro && CompareTo(otro) == 0;
    }

    public override int GetHashCode() {
        return HashCode.Combine(Nombre);
    }

    public override string ToString() {
        return $"{Nombre} ({Telefono})";
    }
}

#region 
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


/// Pruebas de lista ordenada (con contactos) 

var juan  = new Contacto("Juan",  "123456");
var pedro = new Contacto("Pedro", "654321");
var ana   = new Contacto("Ana",   "789012");
var otro  = new Contacto("Otro",  "345678");

var contactos = new ListaOrdenada<Contacto>(new Contacto[] { juan, pedro, ana });
Assert(contactos.Cantidad, 3, "Cantidad de contactos");
Assert(contactos[0].Nombre, "Ana", "Primer contacto");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto");
Assert(contactos[2].Nombre, "Pedro", "Tercer contacto");

Assert(contactos.Filtrar(x => x.Nombre.StartsWith("A")).Cantidad, 1, "Cantidad de contactos que empiezan con A");
Assert(contactos.Filtrar(x => x.Nombre.Contains("a")).Cantidad, 2, "Cantidad de contactos que contienen a");

Assert(contactos.Contiene(juan), true, "Contiene Juan");
Assert(contactos.Contiene(otro), false, "No contiene Otro");

contactos.Agregar(otro);
Assert(contactos.Cantidad, 4, "Cantidad de contactos tras agregar Otro");
Assert(contactos.Contiene(otro), true, "Contiene Otro");

Assert(contactos[0].Nombre, "Ana", "Primer contacto tras agregar Otro");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto tras agregar Otro");
Assert(contactos[2].Nombre, "Otro", "Tercer contacto tras agregar Otro");
Assert(contactos[3].Nombre, "Pedro", "Cuarto contacto tras agregar Otro");

contactos.Eliminar(otro);
Assert(contactos.Cantidad, 3, "Cantidad de contactos tras eliminar Otro");
Assert(contactos[0].Nombre, "Ana", "Primer contacto tras eliminar Otro");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto tras eliminar Otro");
Assert(contactos[2].Nombre, "Pedro", "Tercer contacto tras eliminar Otro");

contactos.Eliminar(otro);
Assert(contactos.Cantidad, 3, "Cantidad de contactos tras eliminar un elemento inexistente");
Assert(contactos[0].Nombre, "Ana", "Primer contacto tras eliminar Otro");
Assert(contactos[1].Nombre, "Juan", "Segundo contacto tras eliminar Otro");
Assert(contactos[2].Nombre, "Pedro", "Tercer contacto tras eliminar Otro");
#endregion