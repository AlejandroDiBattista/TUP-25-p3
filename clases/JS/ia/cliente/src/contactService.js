const API_URL = 'http://localhost:5198/contacts';

export async function getContacts() {
    const res = await fetch(API_URL);
    if (!res.ok) throw new Error('Error al obtener contactos');
    return res.json();
}

export async function getContact(id) {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error('Contacto no encontrado');
    return res.json();
}

export async function createContact(contact) {
    const res = await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(contact)
    });
    if (!res.ok) throw new Error('Error al crear contacto');
    return res.json();
}

export async function updateContact(id, contact) {
    const res = await fetch(`${API_URL}/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(contact)
    });
    if (!res.ok) throw new Error('Error al actualizar contacto');
}

export async function deleteContact(id) {
    const res = await fetch(`${API_URL}/${id}`, {
        method: 'DELETE'
    });
    if (!res.ok) throw new Error('Error al eliminar contacto');
}
