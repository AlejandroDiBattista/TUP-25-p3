class Producto{
    constructor(nombre, precio, cantidad = 0) {
        this.nombre = nombre;
        this.precio = precio;
        this.cantidad = cantidad;
    }

    vender(cantidad){
        if (cantidad <= 0) return;
        if (this.cantidad < cantidad) return

        this.cantidad -= cantidad;
    }

    comprar(cantidad) {
        if (cantidad <= 0) return;
        this.cantidad += cantidad;
    }

    toString() {
        return `${this.nombre} $${this.precio} (x ${this.cantidad})`;
    }
}

let cocaCola = new Producto("Coca Cola", 1500, 10);
let pepsiCola = new Producto("Pepsi Cola", 1400, 5);
let catalogo = [cocaCola, pepsiCola];

function listar(catalogo){
    console.log("\n=== Listado de productos ===");
    for(let producto of catalogo) {
        console.log(producto.toString());
    }
}
listar(catalogo);
cocaCola.vender(2);
listar(catalogo);
pepsiCola.vender(50);
listar(catalogo);