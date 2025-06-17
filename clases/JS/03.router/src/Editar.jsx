import { useParams, Link } from "react-router-dom";

export function Editar(){
    const { nombre } = useParams();
    return (
        <div>
            <h1>Editando {nombre} </h1>
            <p>Esta es la página de edición.</p>
            <br />
            <Link to="/listar">Volver a Listar</Link>
        </div>
    );
}