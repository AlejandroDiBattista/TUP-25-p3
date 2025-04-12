### 1  
¿Qué ocurre cuando se pasa un tipo **primitivo** (por ejemplo `int`) como parámetro a un método **sin** usar `ref` ni `out`?

a) El valor original siempre se modifica  
b) Se pasa por referencia automáticamente  
c) Se pasa una copia del valor original

---

### 2  
¿Qué palabra clave se utiliza en C# para **pasar un argumento por referencia**?

a) `ref`  
b) `out`  
c) `in`

---

### 3  
¿Cuál es la diferencia principal entre `ref` y `out`?

a) `ref` requiere que la variable esté inicializada antes de pasarla, `out` no  
b) `ref` copia el valor, `out` pasa por referencia  
c) No hay diferencia, son sinónimos

---

### 4  
¿Qué ocurre si usás `ref` con una variable que no fue inicializada?

a) El programa compila y usa un valor por defecto  
b) El compilador da un error  
c) El valor se inicializa automáticamente en el método

---

### 5  
¿Qué sucede con los tipos de referencia como clases si los pasás a un método **sin** usar `ref`?

a) Se pasa una copia del objeto completo  
b) Se pasa una copia de la referencia al objeto  
c) El método no puede modificar el objeto original

---

### 6  
¿Cuál de estas sentencias **modifica el valor original** de la variable `x`?

```csharp
void Modificar(int x) {
    x = 100;
}
```

a) Siempre modifica  
b) Nunca modifica  
c) Solo si `x` es una clase

---

### 7  
¿Qué palabra clave garantiza que el valor pasado no será modificado dentro del método?

a) `readonly`  
b) `in`  
c) `const`

---

### 8  
¿Qué se requiere para usar la palabra clave `out` en un método?

a) Que la variable esté inicializada antes de llamarlo  
b) Que el método le asigne obligatoriamente un valor  
c) Que se pase un tipo de valor, no de referencia

---

### 9  
¿Qué salida muestra este código?

```csharp
void Cambiar(ref int x) {
    x = 50;
}

int a = 10;
Cambiar(ref a);
Console.WriteLine(a);
```

a) 10  
b) 0  
c) 50

---

### 10  
¿Cuál de los siguientes usos de `ref` es válido?

a) `void MiMetodo(ref int x = 5)`  
b) `void MiMetodo(ref int x)`  
c) `void MiMetodo(int ref x)`
