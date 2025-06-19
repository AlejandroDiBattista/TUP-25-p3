import './App.css'
import { useState, useEffect, use } from 'react';
import { Plus } from "lucide-react"
import { Editar, Listar } from './components';
import { contactosIniciales } from './data/contactosIniciales';

function useGuardarContactos() {
    let [contactos, setContactos] = useState(contactosIniciales);

    function cargarContactos(){
        let contactosGuardados = localStorage.getItem("contactos");
        if(contactosGuardados) {
            setContactos(JSON.parse(contactosGuardados));
        } else {
            setContactos(contactosIniciales);
        }
    }

    function guardarContactos(){
        localStorage.setItem("contactos", JSON.stringify(contactos));
    }

    useEffect(() => {
        cargarContactos([]);
    }, []);
    
    useEffect(() => {
        guardarContactos();
    }, [contactos]);

    return [contactos, setContactos];
}

function App() {
    let [proximoId,   setProximoId]   = useState(10);     // Para asignar un nuevo ID
    let [modoEdicion, setModoEdicion] = useState(false);  // Permite saber si estamos en modo edición o no

    // Lista de contactos (inicializada con algunos contactos)
    // Contacto que se está editando o agregando
    let [contacto,  setContacto]  = useState({id: 0, nombre: "", email: "", edad: 0});
    let [contactos, setContactos] = useGuardarContactos(); // Hook personalizado para manejar contactos
    

    function guardarContacto(nuevo) {
        if(nuevo.id === 0) { // Si estamos agregando un nuevo contacto
            nuevo.id = proximoId;         // Asignar el ID del nuevo contacto
            setProximoId(proximoId + 1);  // Incrementar el ID para el próximo contacto
            setContactos([...contactos, nuevo]);
        } else { // Si estamos editando un contacto existente
            setContactos(contactos.map(
                c => c.id === nuevo.id 
                    ? {...c, ...nuevo} 
                    : c));
        }

        setContacto({id: 0, nombre: "", email: "", edad: 0}); // Resetear el formulario
        setModoEdicion(false); // Salir del modo edición
    }

    function cancelarEdicion() {
        setContacto({id: 0, nombre: "", email: "", edad: 0}); // Resetear el formulario
        setModoEdicion(false); // Salir del modo edición
    }

    function editarContacto(c) {
        setContacto(c);       // Cargar el contacto a editar
        setModoEdicion(true); // Entrar en modo edición
    }

    function borrarContacto(id) {
        // Copia todos excepto el que se quiere borrar
        setContactos(contactos.filter(
            c => c.id !== id));
        setContacto({id: 0, nombre: "", email: "", edad: 0}); // Resetear el formulario
    }

    // Cuerpo principal del componente App
    

    
    return (
        <div className="app">
            <h1>Agenda de contactos</h1>
            { !modoEdicion && <button onClick={() => setModoEdicion(true)} className="agregar"><Plus size={32} /> Agregar</button>}
            { modoEdicion 
                ? <Editar 
                    contacto={contacto} 
                    alConfirmar={(nuevo) => guardarContacto(nuevo)} 
                    alCancelar ={() => cancelarEdicion()} />
                : <Listar 
                    contactos={contactos}
                    alEditar={c => editarContacto(c)}
                    alBorrar={borrarContacto} />
            }   
        </div>
    )
}


export default App
