# ¿Qué son los componentes en React?

## 1. Concepto

Un **componente** en React es una función (o clase) que retorna una parte de la interfaz de usuario.  
Son **la unidad básica y reutilizable** de cualquier aplicación React.  
Cada componente puede contener su propio código, estado y lógica de presentación.

---

## 2. Reactividad y optimización

Los componentes de React son **reactivos**:  
> Cuando cambian sus datos (props o estado), React los vuelve a renderizar automáticamente para reflejar los cambios en pantalla.

React utiliza un algoritmo eficiente llamado **reconciliación** para comparar el resultado anterior y el nuevo de cada componente, y solo actualiza en el DOM lo estrictamente necesario.  
**Esto permite interfaces rápidas y escalables, sin que el programador tenga que manipular el DOM manualmente**.

Por ejemplo:
- Si cambiás el estado de un componente con `useState`, **solo ese componente y sus hijos** relevantes se renderizan de nuevo.
- Si un componente no recibe cambios en sus props/estado, React evita renderizarlo innecesariamente.

**En resumen:**  
React se encarga de mantener la interfaz sincronizada con los datos y optimiza los cambios para mejorar el rendimiento.

---

## 3. ¿Para qué sirven?

- Dividir la UI en partes pequeñas, reutilizables e independientes.
- Encapsular lógica y presentación.
- Permitir reutilización y mantenimiento eficiente.

---

## 4. ¿Cómo se crea un componente?

La forma más común y moderna es usando **funciones de JavaScript** (Componentes funcionales):

```jsx
function Saludo() {
  return <h1>¡Hola, mundo!</h1>;
}
```
O con arrow functions:
```jsx
const Saludo = () => <h1>¡Hola, mundo!</h1>;
```
**Importante:**  
El nombre de un componente SIEMPRE debe comenzar con mayúscula.

---

## 5. ¿Cómo se usa (renderiza) un componente?

Simplemente escribiéndolo como una etiqueta JSX:

```jsx
function App() {
  return (
    <div>
      <Saludo />
    </div>
  );
}
```

---

## 6. ¿Cómo se pasan **propiedades (props)** a un componente?

Las **props** permiten personalizar un componente desde “afuera”.  
Podés pasarlas como atributos:

```jsx
<Saludo nombre="Alejandro" edad={40} />
```

**Acceso usando desestructuración:**

```jsx
function Saludo({ nombre, edad }) {
  return (
    <h2>
      Hola, {nombre}. Tenés {edad} años.
    </h2>
  );
}
```

**Si no usás desestructuración:**

```jsx
function Saludo(props) {
  return (
    <h2>
      Hola, {props.nombre}. Tenés {props.edad} años.
    </h2>
  );
}
```

---

## 7. ¿Cómo se pasan eventos a un componente?

Pasás una función como prop, igual que un dato común:

```jsx
function Boton({ onClick }) {
  return <button onClick={onClick}>Haz clic</button>;
}

function App() {
  function mostrarAlerta() {
    alert("¡Clickeaste el botón!");
  }

  return <Boton onClick={mostrarAlerta} />;
}
```

---

## 8. ¿Cómo crear componentes que contengan otros componentes? (`children`)

A veces queremos que un componente pueda “envolver” o contener otros componentes o elementos:

```jsx
function Tarjeta({ children }) {
  return <div className="tarjeta">{children}</div>;
}

function App() {
  return (
    <Tarjeta>
      <h2>Contenido flexible</h2>
      <button>Botón dentro de la tarjeta</button>
    </Tarjeta>
  );
}
```
**`children`** es una prop especial que representa todo lo que está entre la etiqueta de apertura y cierre del componente.

---

## 9. ¿Cómo crear componentes en archivos separados y reutilizarlos? (export/import)

**Ejemplo de organización típica:**

- `App.jsx`
- `Saludo.jsx`

### **Archivo: `Saludo.jsx`**

```jsx
// Definición y exportación del componente
export default function Saludo({ nombre }) {
  return <h2>Hola, {nombre}!</h2>;
}
```

### **Archivo: `App.jsx`**

```jsx
import Saludo from './Saludo';

function App() {
  return (
    <div>
      <Saludo nombre="Alejandro" />
    </div>
  );
}

export default App;
```
> Usá siempre **export default** si el archivo define un solo componente principal.

---

## 10. Ejemplo final: todos los conceptos juntos

### **Archivo: `Boton.jsx`**
```jsx
export default function Boton({ texto, onClick }) {
  return <button onClick={onClick}>{texto}</button>;
}
```

### **Archivo: `Tarjeta.jsx`**
```jsx
export default function Tarjeta({ titulo, children }) {
  return (
    <div style={{ border: "1px solid gray", padding: 16 }}>
      <h3>{titulo}</h3>
      <div>{children}</div>
    </div>
  );
}
```

### **Archivo: `App.jsx`**
```jsx
import Tarjeta from './Tarjeta';
import Boton from './Boton';

function App() {
  function handleSaludo() {
    alert("¡Hola desde el evento!");
  }

  return (
    <Tarjeta titulo="Mi primer tarjeta">
      <p>Esto es un componente hijo.</p>
      <Boton texto="Saludar" onClick={handleSaludo} />
    </Tarjeta>
  );
}

export default App;
```

---

# **Resumen de puntos clave**

- Los componentes son funciones que devuelven JSX.
- Son **reactivos**: si cambian sus datos, React los actualiza automáticamente en pantalla.
- React compara los resultados y solo cambia lo necesario para optimizar el rendimiento.
- Se pueden parametrizar usando **props**.
- Se pueden comunicar usando funciones como props (eventos).
- Los componentes pueden contener otros usando **children**.
- Se organizan en archivos y se comparten con **import/export**.

---
