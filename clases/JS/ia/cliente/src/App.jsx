import { useEffect, useState } from 'react'
import './App.css'
import { getContacts, createContact, updateContact, deleteContact } from './contactService'
import ContactTable from './ContactTable'
import ContactForm from './ContactForm'

function App() {
  const [contactos, setContactos] = useState([]);
  const [editando, setEditando] = useState(null);
  const [mostrandoForm, setMostrandoForm] = useState(false);
  const [cargando, setCargando] = useState(true);

  const cargarContactos = async () => {
    setCargando(true);
    try {
      const data = await getContacts();
      setContactos(data);
    } catch (e) {
      alert(e.message);
    }
    setCargando(false);
  };

  useEffect(() => {
    cargarContactos();
  }, []);

  const crearContacto = async contacto => {
    try {
      await createContact(contacto);
      setMostrandoForm(false);
      cargarContactos();
    } catch (e) {
      alert(e.message);
    }
  };

  const actualizarContacto = async contacto => {
    try {
      await updateContact(contacto.id, contacto);
      setEditando(null);
      setMostrandoForm(false);
      cargarContactos();
    } catch (e) {
      alert(e.message);
    }
  };

  const eliminarContacto = async id => {
    if (!window.confirm('¿Eliminar contacto?')) return;
    try {
      await deleteContact(id);
      cargarContactos();
    } catch (e) {
      alert(e.message);
    }
  };

  return (
    <div className="container">
      <h1>Agenda de Contactos</h1>
      {cargando ? <p>Cargando...</p> : (
        <>
          <button className="nuevo" onClick={() => { setMostrandoForm(true); setEditando(null); }}>Nuevo contacto</button>
          <ContactTable contactos={contactos} onEdit={c => { setEditando(c); setMostrandoForm(true); }} onDelete={eliminarContacto} />
        </>
      )}
      {mostrandoForm && (
        <div className="modal">
          <ContactForm
            initial={editando}
            onSave={editando ? (data) => actualizarContacto({ ...editando, ...data }) : crearContacto}
            onCancel={() => { setMostrandoForm(false); setEditando(null); }}
          />
        </div>
      )}
    </div>
  );
}

export default App
