Aprendiendo useEffect en React

¿Qué es useEffect?

En React, useEffect es un hook que permite a los componentes ejecutar “efectos secundarios”.
Un efecto secundario es cualquier acción que ocurre fuera del flujo normal de renderizado, como:
	•	Traer datos de una API
	•	Modificar directamente el DOM
	•	Usar temporizadores (setTimeout, setInterval)
	•	Escuchar eventos globales (teclado, scroll, etc.)

¿Por qué no podemos hacer esto en el render?

Si pusieras un fetch o un setInterval en el cuerpo de tu función, se ejecutaría cada vez que el componente se renderiza, ¡y eso puede ser muchas veces!
useEffect te permite controlar cuándo y cómo se ejecuta ese código, evitando errores, bucles infinitos o fugas de memoria.

⸻

¿Cómo se usa useEffect?
```js
import { useEffect } from "react";

useEffect(() => {
  // Código que querés ejecutar después de que el componente se renderice

  return () => {
    // (Opcional) Código de limpieza cuando el componente se va o antes de repetir el efecto
  };
}, [dependencias]);
```
•	El primer parámetro es una función con el código del efecto.
•	El segundo parámetro (arreglo de dependencias) le dice a React cuándo ejecutar el efecto.

⸻

Ejemplo 1: Mostrar un mensaje solo al montar el componente

```js
import { useEffect } from "react";

function App() {
  useEffect(() => {
    console.log("El componente se ha montado");
  }, []); // Arreglo vacío = solo al montar

  return <h1>Hola mundo</h1>;
}
```

¿Qué pasa aquí?
El mensaje se muestra una sola vez, justo cuando el componente aparece.

⸻

Ejemplo 2: Obtener datos de una API al cargar el componente

```js
import { useEffect, useState } from "react";

function Usuarios() {
  const [usuarios, setUsuarios] = useState([]);

  useEffect(() => {
    const obtenerUsuarios = async () => {
      const res = await fetch("https://jsonplaceholder.typicode.com/users");
      const data = await res.json();
      setUsuarios(data);
    };
    obtenerUsuarios();
  }, []); // Solo la primera vez

  return (
    <ul>
      {usuarios.map(u => <li key={u.id}>{u.name}</li>)}
    </ul>
  );
}
```

¿Por qué es importante useEffect?
Si pusieras el fetch fuera del useEffect, se ejecutaría en cada render y podrías entrar en un bucle.

⸻

Ejemplo 3: Sincronizar un valor con localStorage

```js
import { useState, useEffect } from "react";

function BlocDeNotas() {
  const [texto, setTexto] = useState(
    () => localStorage.getItem("notas") || ""
  );

  useEffect(() => {
    localStorage.setItem("notas", texto);
  }, [texto]); // Cada vez que cambia texto

  return (
    <textarea value={texto} onChange={e => setTexto(e.target.value)} />
  );
}
```

¿Qué hace este useEffect?
Cada vez que cambiás el texto, se guarda automáticamente en localStorage.

⸻

Ejemplo 4: Limpiar un temporizador

```js
import { useEffect, useState } from "react";

function Temporizador() {
  const [segundos, setSegundos] = useState(0);

  useEffect(() => {
    const id = setInterval(() => {
      setSegundos(s => s + 1);
    }, 1000);

    // Limpieza: detener el temporizador cuando el componente se va
    return () => clearInterval(id);
  }, []);

  return <p>Han pasado {segundos} segundos.</p>;
}
```

¿Por qué la limpieza?
Sin el return, el temporizador seguiría funcionando incluso si el componente desaparece.

⸻

Resumen
	•	useEffect permite ejecutar código después de renderizar.
	•	Usá el array de dependencias para controlar cuándo se ejecuta el efecto.
	•	Usá el return para limpiar recursos o suscripciones.
	•	¡Nunca pongas código “con efectos secundarios” directo en el render!

⸻

Ejercicios Propuestos
	1.	Modificá el ejemplo de localStorage para guardar una lista de tareas.
	2.	Creá un efecto que cambie el título de la página (document.title) según el estado de tu app.
	3.	Usá un effect para mostrar una alerta cuando un contador llegue a 10.

⸻
