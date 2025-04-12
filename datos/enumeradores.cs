using System;
using System.Collections;
using System.Collections.Generic;

class Lista : IEnumerable<string> {
    public List<string> Nombres {get; private set;} = new();

    public Lista(){}

    public void Add(string nombre) => Nombres.Add(nombre);

    public IEnumerator<string> GetEnumerator() => Nombres.GetEnumerator();
    public IEnumerator GetEnumerator() => ((IEnumerable)Nombres).GetEnumerator();
}

var lista = new Lista();
lista.Add("Alice"); // nuevo elemento
lista.Add("Bob");   // nuevo elemento

Console.WriteLine("=== Lista de Nombres ===");
foreach(var x in lista){
    Console.WriteLine(x);
}

Console.WriteLine("=== Lista de Nombres ===");
foreach(var x in lista){
    Console.WriteLine(x);
}