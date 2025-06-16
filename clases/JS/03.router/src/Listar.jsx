import { Link } from "react-router-dom";
export function Listar() {
    const nombres = ["Alejandro", "Franco", "María", "Lucía", "Pedro"];
    return (
        <div>
        <h1>Listar</h1>
        <ul>
            {nombres.map((nombre, index) => (
                <li key={index}>
                    <Link to={`/editar/${nombre}`}>{nombre}</Link>
                </li>
            ))}
        </ul>
        </div>
    );
}