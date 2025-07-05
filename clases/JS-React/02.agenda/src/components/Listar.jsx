import { Mostrar } from './Mostrar';

export function Listar({contactos, alEditar, alBorrar}) {
    return contactos.map(c => 
        <Mostrar 
            key={c.id} 
            contacto={c} 
            alEditar={alEditar}
            alBorrar={alBorrar} 
        />
    );
}
