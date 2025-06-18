 # 1. Motivación y conceptos básicos

- **¿Por qué necesitamos enrutamiento en una SPA?**  
  - Evitar recargas completas de página.  
  - Mantener la experiencia fluida de una aplicación de una sola página.  
- **Terminología clave:**  
  - **Ruta (Route):** Asocia una URL con un componente.  
  - **Router:** Contenedor que habilita el enrutamiento.  
  - **Navegación:** Enlaces que cambian la URL sin refrescar la página.

---

## 2. Instalación y configuración inicial

1. **Instalar la librería**  
   ```bash
   npm install react-router-dom@6
   ```
2. **Envolver tu aplicación en un Router**  
   ```jsx
   // index.jsx
   import { createRoot } from 'react-dom/client';
   import { BrowserRouter } from 'react-router-dom';
   import App from './App';

   createRoot(document.getElementById('root')).render(
     <BrowserRouter>
       <App />
     </BrowserRouter>
   );
   ```
3. **Explicación:**  
   - `BrowserRouter` escucha cambios en la URL y renderiza rutas correspondientes.

---

## 3. Declaración de rutas y componentes de navegación

1. **Definir rutas en el componente principal**  
   ```jsx
   // App.jsx
   import { Routes, Route, Link } from 'react-router-dom';
   import Home from './pages/Home';
   import About from './pages/About';
   import NotFound from './pages/NotFound';

   function App() {
     return (
       <nav>
         <Link to="/">Inicio</Link> |{' '}
         <Link to="/about">Acerca</Link>
       </nav>
       <Routes>
         <Route path="/" element={<Home />} />
         <Route path="/about" element={<About />} />
         <Route path="*" element={<NotFound />} />
       </Routes>
     );
   }

   export default App;
   ```
2. **Diferencia `Link` vs. `<a>` tradicional:**  
   - `Link` previene recarga total y actualiza el estado de React.
3. **Actividad guiada:**  
   - Crear dos componentes sencillos (`Home`, `About`) que muestren un título y un párrafo.

---

## 4. Navegación programática y parámetros de ruta

1. **Parámetros dinámicos**  
   ```jsx
   // pages/Product.jsx
   import { useParams } from 'react-router-dom';

   function Product() {
     const { id } = useParams();
     return <h2>Producto #{id}</h2>;
   }

   // En App.jsx
   <Route path="/product/:id" element={<Product />} />
   ```
2. **Navegar desde código**  
   ```jsx
   import { useNavigate } from 'react-router-dom';

   function LoginButton() {
     const navigate = useNavigate();
     const handleLogin = () => {
       // ... lógica de autenticación
       navigate('/dashboard');
     };
     return <button onClick={handleLogin}>Entrar</button>;
   }
   ```
3. **Ejercicio práctico:**  
   - Crear lista de productos con enlaces a `/product/1`, `/product/2`, etc., y mostrar detalles dinámicos.

---

## 5. Rutas anidadas (Nested Routes)

1. **Estructura de carpetas**  
   ```
   src/
     pages/
       Dashboard.jsx
       Dashboard/
         Profile.jsx
         Settings.jsx
   ```
2. **Definir rutas anidadas**  
   ```jsx
   // App.jsx
   <Routes>
     <Route path="/dashboard" element={<Dashboard />}>
       <Route path="profile" element={<Profile />} />
       <Route path="settings" element={<Settings />} />
     </Route>
   </Routes>
   ```
3. **Usar `<Outlet>` para renderizar hijos**  
   ```jsx
   // pages/Dashboard.jsx
   import { Outlet, Link } from 'react-router-dom';

   function Dashboard() {
     return (
       <>
         <h1>Dashboard</h1>
         <nav>
           <Link to="profile">Perfil</Link> |{' '}
           <Link to="settings">Configuración</Link>
         </nav>
         <Outlet />
       </>
     );
   }
   ```
4. **Actividad:**  
   - Ampliar el ejercicio de productos para tener una sección de “Administración” con rutas anidadas.

---

## 6. Buenas prácticas y recomendaciones

- **Mantén los componentes de página en `src/pages`** y los componentes de UI reutilizables en `src/components`.  
- **Evita rutas profundamente anidadas** (más de 3 niveles), podrías fragmentar la UI en Layouts independientes.  
- **Manejo de rutas protegidas:** crear un componente `<PrivateRoute>` que compruebe permisos antes de renderizar su `element`.  
- **404 y redirecciones:** usar `path="*"` para capturar rutas desconocidas y `Navigate` para redirigir:
  ```jsx
  import { Navigate } from 'react-router-dom';
  <Route path="/old-path" element={<Navigate to="/new-path" replace />} />
  ```

