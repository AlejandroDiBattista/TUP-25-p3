import { Trash2, Pencil } from "lucide-react";
import { Tarjeta } from './Tarjeta';

export function Mostrar({contacto, alBorrar, alEditar}) {
    return (
        <Tarjeta titulo="Contacto">
            <p>ID: <b>{contacto.id}</b></p>
            <p>Nombre: <b>{contacto.nombre}</b></p>
            <p>Email: <b>{contacto.email}</b></p>
            <p>Edad: <b>{contacto.edad}</b></p>
            <div className="botones">
                <button onClick={() => alEditar(contacto)}><Pencil /> Editar</button>
                <button onClick={() => alBorrar(contacto.id)}><Trash2 /> Borrar</button>
            </div>
        </Tarjeta>
    );
}
