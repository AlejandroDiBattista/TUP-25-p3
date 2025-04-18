using System;
using System.Collections.Generic;


class ListaOrdenada<T> where T : IComparable<T> {
    private T[] items;
    private int cantidad;
    private const int CAPACIDAD_INICIAL = 4;

    public ListaOrdenada() {
        items = new T[0]; // Empezar con tamaño 0, Array.Resize lo manejará
        cantidad = 0;
    }

    public ListaOrdenada(IEnumerable<T> itemsIniciales) : this() {
        foreach (var item in itemsIniciales) {
            Agregar(item);
        }
    }

    public int Cantidad => cantidad;

    public T this[int index] {
        get {
            if (index < 0 || index >= cantidad) {
                throw new IndexOutOfRangeException("Índice fuera de rango.");
            }
            return items[index];
        }
    }

    // Método privado para buscar el índice usando búsqueda binaria.
    // Devuelve el índice si se encuentra.
    // Devuelve el complemento bitwise (~) del índice donde debería insertarse si no se encuentra.
    private int BuscarIndice(T item) {
        int bajo = 0;
        int alto = cantidad - 1;

        while (bajo <= alto) {
            int medio = bajo + (alto - bajo) / 2;
            int comparacion = items[medio].CompareTo(item);

            if (comparacion == 0) {
                return medio; // Encontrado
            }

            if (comparacion < 0) {
                bajo = medio + 1; // Buscar en la mitad derecha
            } else {
                alto = medio - 1; // Buscar en la mitad izquierda
            }
        }

        return ~bajo; // No encontrado, devuelve dónde debería estar
    }

    // Método privado para asegurar la capacidad del array interno.
    private void AsegurarCapacidad() {
        if (cantidad == items.Length) {
            int nuevaCapacidad = items.Length == 0 ? CAPACIDAD_INICIAL : items.Length * 2;
            Array.Resize(ref items, nuevaCapacidad);
        }
    }

    public void Agregar(T item) {
        int indice = BuscarIndice(item);

        if (indice >= 0) return; // El elemento ya existe, no agregar duplicados

        int indiceInsercion = ~indice; // Calcular índice de inserción real

        // Asegurar capacidad antes de insertar
        AsegurarCapacidad();

        // Desplazar elementos a la derecha para hacer espacio usando Array.Copy
        if (indiceInsercion < cantidad) {
            Array.Copy(items, indiceInsercion, items, indiceInsercion + 1, cantidad - indiceInsercion);
        }

        // Insertar el nuevo elemento
        items[indiceInsercion] = item;
        cantidad++;
    }

    public void Eliminar(T item) {
        int indiceEliminar = BuscarIndice(item);

        if (indiceEliminar >= 0) { // Solo eliminar si se encuentra el elemento
            cantidad--;
            // Desplazar elementos a la izquierda para llenar el espacio usando Array.Copy
            if (indiceEliminar < cantidad) {
                Array.Copy(items, indiceEliminar + 1, items, indiceEliminar, cantidad - indiceEliminar);
            }
            items[cantidad] = default(T); // Limpiar la última posición (opcional)
        }
    }

    public bool Contiene(T item) {
        return BuscarIndice(item) >= 0;
    }

    public ListaOrdenada<T> Filtrar(Func<T, bool> predicado) {
        ListaOrdenada<T> resultado = new ListaOrdenada<T>();
        // Filtrar no se beneficia directamente de la búsqueda binaria,
        // ya que debe evaluar el predicado para cada elemento.
        for (int i = 0; i < cantidad; i++) {
            if (predicado(items[i])) {
                // Usamos el Agregar optimizado del resultado.
                resultado.Agregar(items[i]);
            }
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
        return Nombre.GetHashCode();
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