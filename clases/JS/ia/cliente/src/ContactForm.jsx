import { useState } from 'react';

export default function ContactForm({ onSave, onCancel, initial }) {
  const [form, setForm] = useState(initial || { nombre: '', apellido: '', telefono: '', email: '' });

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = e => {
    e.preventDefault();
    onSave(form);
  };

  return (
    <form className="form" onSubmit={handleSubmit}>
      <input name="nombre" placeholder="Nombre" value={form.nombre} onChange={handleChange} required />
      <input name="apellido" placeholder="Apellido" value={form.apellido} onChange={handleChange} required />
      <input name="telefono" placeholder="Teléfono" value={form.telefono} onChange={handleChange} required />
      <input name="email" placeholder="Email" value={form.email} onChange={handleChange} required type="email" />
      <div className="form-actions">
        <button type="submit">Guardar</button>
        <button type="button" onClick={onCancel}>Cancelar</button>
      </div>
    </form>
  );
}
