export default function ContactTable({ contactos, onEdit, onDelete }) {
  return (
    <table>
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Apellido</th>
          <th>Teléfono</th>
          <th>Email</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        {contactos.map(c => (
          <tr key={c.id}>
            <td>{c.nombre}</td>
            <td>{c.apellido}</td>
            <td>{c.telefono}</td>
            <td>{c.email}</td>
            <td>
              <button onClick={() => onEdit(c)}>Editar</button>
              <button onClick={() => onDelete(c.id)} className="eliminar">Eliminar</button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
