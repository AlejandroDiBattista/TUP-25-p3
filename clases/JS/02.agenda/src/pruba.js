
let contactos = [
    {id: 1, nombre: "Alejandro", email: "alejandro@example.com", edad: 30},
    {id: 2, nombre: "Juan",      email: "juan@example.com",      edad: 25},
]

// let copia = [...contactos]               // Copiar
// let copia = contactos.map(c => c)        // Cambiar
// let copia = contactos.filter(c => true); // Borrar

let nuevo  = {id: 5, nombre: "Nuevo", email: "nuevo@example.com", edad: 20}
let cambio = {id: 1, nombre: "Este esta cambiado"}

let agregado = [...contactos, nuevo];                                           // Agregar
let borrados = contactos.filter(c => c.id !== cambio.id);                       // Borrar
let cambiado = contactos.map(c => c.id === cambio.id ? {...c, ...cambio} : c);  // Cambiar

console.log("Contactos", contactos)
console.log("Copia", agregado)
console.log("Borrados", borrados)
console.log("Cambiados", cambiado)

let numero = "10"
console.log( Number( numero) + 5)