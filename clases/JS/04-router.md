# Cómo crear un sitio multipágina con `react-router-dom` en React

## 1. ¿Para qué sirve `react-router-dom`?

React, por sí solo, construye aplicaciones de “página única” (SPA: Single Page Application).  
Sin un router, no podés tener URLs distintas con diferentes componentes, como `/`, `/contacto`, `/productos`, etc.

**`react-router-dom`** es la librería estándar para agregar **ruteo** (navegación por páginas) a tus aplicaciones React.  
Permite que tu app reaccione a cambios en la URL mostrando diferentes componentes, sin recargar la página.

---

## 2. ¿Cómo se instala?

Desde la terminal, en la carpeta de tu proyecto React:

```bash
npm install react-router-dom
```
O con yarn:
```bash
yarn add react-router-dom
```

---

## 3. Conceptos y funciones principales

- **BrowserRouter**: componente que habilita el uso de rutas (debe envolver tu app).
- **Routes**: contenedor de rutas.
- **Route**: define qué componente se muestra en cada URL.
- **Link**: para navegar entre páginas sin recargar el navegador.
- **useNavigate, useParams**: hooks para navegación programática y leer parámetros de la URL.
- **Outlet**: muestra componentes anidados (subrutas).

---

## 4. Ejemplo básico: Dos páginas y navegación

### Paso 1: Estructura mínima

```jsx
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";

function Inicio() {
  return <h2>Inicio</h2>;
}

function Contacto() {
  return <h2>Contacto</h2>;
}

function App() {
  return (
    <BrowserRouter>
      <nav>
        <Link to="/">Inicio</Link>
        {" | "}
        <Link to="/contacto">Contacto</Link>
      </nav>
      <Routes>
        <Route path="/" element={<Inicio />} />
        <Route path="/contacto" element={<Contacto />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
```
- **`<BrowserRouter>`** debe envolver tu aplicación.
- **`<Routes>`** contiene todos los `<Route>`.
- Cada **`<Route>`** mapea una URL (`path`) a un componente (`element`).
- **`<Link to="..." />`** genera un link navegable interno.

---

## 5. Rutas con parámetros

Podés crear rutas dinámicas, por ejemplo para un perfil de usuario:

```jsx
import { BrowserRouter, Routes, Route, Link, useParams } from "react-router-dom";

function Usuario() {
  const { nombre } = useParams();
  return <h2>Perfil de {nombre}</h2>;
}

function App() {
  return (
    <BrowserRouter>
      <nav>
        <Link to="/">Inicio</Link>
        {" | "}
        <Link to="/usuario/Alejandro">Perfil Alejandro</Link>
        {" | "}
        <Link to="/usuario/Martina">Perfil Martina</Link>
      </nav>
      <Routes>
        <Route path="/" element={<h2>Página de inicio</h2>} />
        <Route path="/usuario/:nombre" element={<Usuario />} />
      </Routes>
    </BrowserRouter>
  );
}
```
- **`/usuario/:nombre`** es una ruta “paramétrica”.
- Usás `useParams()` para acceder a esos valores dentro del componente.

---

## 6. Navegación programática con `useNavigate`

Si querés navegar a otra página luego de un evento (por ejemplo, después de un submit):

```jsx
import { useNavigate } from "react-router-dom";

function Formulario() {
  const navigate = useNavigate();

  function handleSubmit(e) {
    e.preventDefault();
    // ... lógica del formulario
    navigate("/gracias");
  }

  return (
    <form onSubmit={handleSubmit}>
      <button type="submit">Enviar</button>
    </form>
  );
}
```
- `useNavigate()` retorna una función para cambiar la ruta por código.

---

## 7. Subrutas (anidación de rutas)

Podés tener rutas anidadas (por ejemplo, dentro de un panel de usuario):

```jsx
import { Outlet, Link, Routes, Route, BrowserRouter } from "react-router-dom";

function Panel() {
  return (
    <div>
      <h2>Panel de usuario</h2>
      <nav>
        <Link to="perfil">Perfil</Link>
        {" | "}
        <Link to="configuracion">Configuración</Link>
      </nav>
      <Outlet />
    </div>
  );
}

function Perfil() {
  return <div>Perfil del usuario</div>;
}

function Configuracion() {
  return <div>Configuración de la cuenta</div>;
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/panel/*" element={<Panel />}>
          <Route path="perfil" element={<Perfil />} />
          <Route path="configuracion" element={<Configuracion />} />
        </Route>
        <Route path="/" element={<h2>Página principal</h2>} />
      </Routes>
    </BrowserRouter>
  );
}
```
- El componente **`<Outlet />`** es el lugar donde se muestran las subrutas.

---

## 8. Página no encontrada (404)

Podés definir un “catch-all” para rutas desconocidas:

```jsx
<Route path="*" element={<h2>Página no encontrada</h2>} />
```

---

## 9. Rutas protegidas y navegación condicional

Supongamos que solo querés mostrar cierta página si el usuario está autenticado:

```jsx
import { Navigate } from "react-router-dom";

function RutasProtegidas({ usuario, children }) {
  if (!usuario) {
    return <Navigate to="/" />;
  }
  return children;
}

// Uso:
<Route
  path="/panel"
  element={
    <RutasProtegidas usuario={usuarioActual}>
      <Panel />
    </RutasProtegidas>
  }
/>
```
- `Navigate` permite redireccionar según condiciones.
- `RutasProtegidas` es un patrón común para rutas privadas.

---

## 10. Resumen de flujo para un sitio multipágina

1. Instalá `react-router-dom`.
2. Usá `<BrowserRouter>` en la raíz.
3. Definí rutas con `<Routes>` y `<Route>`.
4. Usá `<Link>` para navegación interna.
5. Para rutas dinámicas, usá `useParams`.
6. Para navegar por código, usá `useNavigate`.
7. Para subrutas, usá `<Outlet>` y rutas anidadas.
8. Definí un catch-all para 404.
9. Usá `<Navigate>` y wrappers para rutas protegidas.

---

## 11. Estructura de archivos recomendada

```
/src
  /pages
    Inicio.jsx
    Contacto.jsx
    Usuario.jsx
    Panel.jsx
    Perfil.jsx
    Configuracion.jsx
  App.jsx
  index.js
```
Así podés importar tus páginas en `App.jsx` y mantener tu proyecto organizado.

---

## 12. Ejemplo de App.jsx con todo junto

```jsx
import { BrowserRouter, Routes, Route, Link, useParams, Outlet, useNavigate, Navigate } from "react-router-dom";
import Inicio from "./pages/Inicio";
import Contacto from "./pages/Contacto";
import Usuario from "./pages/Usuario";
import Panel from "./pages/Panel";
import Perfil from "./pages/Perfil";
import Configuracion from "./pages/Configuracion";

function RutasProtegidas({ usuario, children }) {
  if (!usuario) {
    return <Navigate to="/" />;
  }
  return children;
}

function App() {
  const usuarioActual = { nombre: "Alejandro" }; // simulación

  return (
    <BrowserRouter>
      <nav>
        <Link to="/">Inicio</Link> |{" "}
        <Link to="/contacto">Contacto</Link> |{" "}
        <Link to="/usuario/Alejandro">Mi perfil</Link> |{" "}
        <Link to="/panel">Panel</Link>
      </nav>
      <Routes>
        <Route path="/" element={<Inicio />} />
        <Route path="/contacto" element={<Contacto />} />
        <Route path="/usuario/:nombre" element={<Usuario />} />
        <Route
          path="/panel/*"
          element={
            <RutasProtegidas usuario={usuarioActual}>
              <Panel />
            </RutasProtegidas>
          }
        >
          <Route path="perfil" element={<Perfil />} />
          <Route path="configuracion" element={<Configuracion />} />
        </Route>
        <Route path="*" element={<h2>Página no encontrada</h2>} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
```

---

# Recursos útiles

- [Documentación oficial de React Router](https://reactrouter.com/en/main)
- [Ejemplos de uso](https://reactrouter.com/en/main/start/tutorial)

---

¿Querés seguir con temas como navegación anidada más compleja, rutas públicas/privadas avanzadas, o integración con lazy loading?
