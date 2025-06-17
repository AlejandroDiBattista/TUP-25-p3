export function Tarjeta({titulo, children}) {
    return (
        <div className="tarjeta">
            {titulo && <h2>{titulo}</h2>}
            {children}
        </div>
    );
}
