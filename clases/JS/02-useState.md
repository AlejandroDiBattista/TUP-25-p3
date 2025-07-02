# Aprendiendo `useState` en React

## ¿Qué es `useState`?

`useState` es un **hook** de React que permite a los componentes funcionales **guardar y modificar valores** (estado) a lo largo del tiempo.

Antes de los hooks, solo los componentes de clase podían tener “estado”. Con `useState`, cualquier función puede recordar información entre renders.

---

## ¿Para qué sirve el estado?

El estado es útil cuando **algo puede cambiar** en tu componente:

- Un valor de un input
- Un contador
- Si un menú está abierto o cerrado
- La lista de elementos mostrados

---

## ¿Cómo se usa `useState`?

```jsx
import { useState } from "react";

const [valor, setValor] = useState(valorInicial);
```

- `valor`: es el **estado actual**.
- `setValor`: es la **función para actualizar el estado**.
- `valorInicial`: es el valor con el que empieza el estado (solo en el primer render).

---

## Inmutabilidad: ¿Por qué NO debo modificar el valor del estado directamente?

Cuando usás `useState`, **NO debés modificar el valor directamente**.

Por ejemplo, si tu estado es una lista de objetos, **nunca hagas**:

```js
// ¡INCORRECTO!
miLista.push(nuevoElemento); // NO HACE RENDER

// ¡INCORRECTO!
miLista[0].nombre = "Nuevo nombre"; // NO HACE RENDER
```

**¿Por qué?**  
React detecta los cambios cuando *llamás* a la función de actualización (`setMiLista`) con un **nuevo objeto o arreglo**.  
Si modificás el estado existente, React no ve el cambio y NO re-renderiza.

---

## ¿Cómo agregar, cambiar y borrar en una lista de objetos?

Supongamos que tenemos:

```js
const [contactos, setContactos] = useState([]);
```

### **Agregar un elemento:**

```js
setContactos([...contactos, nuevoContacto]);
```
> Usamos el spread `...` para crear un **nuevo array** con el elemento agregado.

---

### **Modificar un elemento:**

Por ejemplo, cambiar el nombre de un contacto con id específico:

```js
setContactos(
  contactos.map(c =>
    c.id === idBuscado
      ? { ...c, nombre: "Nuevo nombre" }
      : c
  )
);
```
> El método `.map` crea un **nuevo array** donde solo el objeto deseado es reemplazado por una copia actualizada.

---

### **Eliminar un elemento:**

```js
setContactos(
  contactos.filter(c => c.id !== idABorrar)
);
```
> `.filter` crea un **nuevo array** sin el elemento a borrar.

---

## Inicialización con función flecha

Cuando el **valor inicial** depende de un cálculo costoso o de un acceso externo (por ejemplo, `localStorage`),  
podés pasarle a `useState` una función en vez de un valor:

```js
const [usuarios, setUsuarios] = useState(() => {
  // Esta función SOLO se ejecuta en el primer render
  return cargarUsuariosDesdeStorage();
});
```

**¿Por qué es útil?**  
Si pusieras simplemente `useState(cargarUsuariosDesdeStorage())`,  
la función se ejecutaría en **cada render**, no solo al inicio.  
Con `useState(() => ...)` te asegurás que solo corre una vez.

---

## Actualización de estado basada en el estado previo (función actualizadora)

A veces, querés actualizar el estado **en base al valor anterior**.  
Por ejemplo, sumar a un contador:

```js
setCuenta(cuenta + 1); // Correcto, pero...
```
Pero si varias actualizaciones se hacen rápido, **puede fallar**.  
En su lugar, usá la función actualizadora:

```js
setCuenta(cuentaAnterior => cuentaAnterior + 1);
```

**¿Por qué es mejor?**  
React garantiza que siempre te da el valor más actualizado del estado, aunque haya varias actualizaciones en cola.

---

## Ejemplo 1: Contador básico

```jsx
import { useState } from "react";

function Contador() {
  const [cuenta, setCuenta] = useState(0);

  return (
    <div>
      <p>Cuenta: {cuenta}</p>
      <button onClick={() => setCuenta(cuenta + 1)}>Sumar</button>
      <button onClick={() => setCuenta(cuenta - 1)}>Restar</button>
      <button onClick={() => setCuenta(0)}>Reset</button>
    </div>
  );
}
```
**¿Qué hace?**  
Cada vez que hacés clic, el valor cambia y el componente se actualiza.

---

## Ejemplo 2: Input controlado

```jsx
import { useState } from "react";

function NombreUsuario() {
  const [nombre, setNombre] = useState("");

  return (
    <div>
      <input
        value={nombre}
        onChange={e => setNombre(e.target.value)}
        placeholder="Escribí tu nombre"
      />
      <p>Hola, {nombre || "desconocido"}!</p>
    </div>
  );
}
```
**¿Qué hace?**  
El estado `nombre` siempre tiene el valor del input, y se actualiza a medida que escribís.

---

## Ejemplo 3: Lista de tareas (agregar y quitar)

```jsx
import { useState } from "react";

function ListaTareas() {
  const [tareas, setTareas] = useState([]);
  const [texto, setTexto] = useState("");

  function agregarTarea() {
    if (texto.trim() === "") return;
    setTareas([...tareas, texto]);
    setTexto("");
  }

  function quitarTarea(idx) {
    setTareas(tareas.filter((_, i) => i !== idx));
  }

  return (
    <div>
      <input
        value={texto}
        onChange={e => setTexto(e.target.value)}
        placeholder="Nueva tarea"
      />
      <button onClick={agregarTarea}>Agregar</button>
      <ul>
        {tareas.map((t, i) => (
          <li key={i}>
            {t}
            <button onClick={() => quitarTarea(i)}>Eliminar</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
```
**¿Qué hace?**  
Permite agregar y eliminar tareas de una lista, usando dos estados: uno para el texto y otro para la lista.

---

## Ejemplo completo: ABM de contactos (agregar, modificar y eliminar)

```jsx
import { useState } from "react";

function App() {
  const [contactos, setContactos] = useState([
    { id: 1, nombre: "Juan", telefono: "123" }
  ]);

  function agregar() {
    setContactos(cs => [
      ...cs,
      { id: Date.now(), nombre: "Nuevo", telefono: "000" }
    ]);
  }

  function modificar(id, nombreNuevo) {
    setContactos(cs =>
      cs.map(c =>
        c.id === id ? { ...c, nombre: nombreNuevo } : c
      )
    );
  }

  function eliminar(id) {
    setContactos(cs => cs.filter(c => c.id !== id));
  }

  // Renderizar...
}
```

---

## Notas importantes

- **Cada llamada a `useState` es independiente.**  
  Podés tener tantos estados como necesites.

- **Actualizar estado no es inmediato.**  
  React re-renderiza después de llamar a la función de seteo.

- **No modifiques el estado directamente.**  
  Usá la función de actualización siempre (`setValor`).

---

## Ejercicios propuestos

1. Modificá el contador para que solo permita valores entre 0 y 10.
2. Crea un componente que oculte o muestre un texto usando un botón.
3. Haz una lista de compras que permita tachar los ítems completados.

---
