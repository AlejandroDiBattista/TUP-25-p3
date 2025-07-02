import { useState } from 'react';
import { Tarjeta } from './Tarjeta';

export function Editar({contacto, alConfirmar, alCancelar}) {
    // Estados locales para manejar los campos del formulario
    let [nombre, setNombre] = useState(contacto.nombre || "");
    let [email,  setEmail ] = useState(contacto.email  || "");
    let [edad,   setEdad  ] = useState(contacto.edad   || 0);

    function confirmar() {
        // Al confirmar, se crea un nuevo objeto contacto con los valores del formulario
        alConfirmar({...contacto, nombre, email, edad});
    }

    return ( 
        <Tarjeta titulo={contacto.id === 0 ? "Agregando Contacto" : "Editando Contacto"}>
            <form>
                <fieldset>
                    <legend>Nombre</legend>
                    <input type="text"  
                        value={nombre} 
                        placeholder="Ingresar Nombre" 
                        onChange={e => setNombre(e.target.value)}
                    />
                </fieldset>

                <fieldset>
                    <legend>Email</legend>
                    <input type="text"  
                        value={email} 
                        placeholder="Ingresar Email" 
                        onChange={e => setEmail(e.target.value)}      
                    />
                </fieldset>
                <fieldset>
                    <legend>Edad</legend>
                    <input type="number"
                        value={edad}
                        placeholder="Ingresar Edad"
                        onChange={e => setEdad(parseInt(e.target.value) || 0)} />
                </fieldset>   

                <div className="botones"> 
                    <button type="button" onClick={alCancelar}>Cancelar</button>
                    <button type="button" onClick={confirmar}><b>Aceptar</b></button>
                </div>
            </form>
        </Tarjeta>
    );
}
