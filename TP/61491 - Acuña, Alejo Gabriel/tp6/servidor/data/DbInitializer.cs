using servidor.Models;

namespace servidor.Data;

public static class DbInitializer {
    public static void Inicializar(TiendaContext context) {
        if (context.Productos.Any()) return; // Ya hay productos

        var productos = new List<Producto> {
            new Producto { Nombre = "Camiseta Argentina 1986", Descripcion = "Replica oficial del mundial 86", Precio = 15000, Stock = 10, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/be/Camiseta_Argentina_Mundial_1986.svg/512px-Camiseta_Argentina_Mundial_1986.svg.png" },
            new Producto { Nombre = "Camiseta Brasil 1970", Descripcion = "Histórica camiseta de Pelé", Precio = 14000, Stock = 8, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4b/Brazil1970WorldCupShirt.svg/512px-Brazil1970WorldCupShirt.svg.png" },
            new Producto { Nombre = "Camiseta Alemania 1990", Descripcion = "Final contra Argentina", Precio = 13500, Stock = 6, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f0/Germany1990WorldCupShirt.svg/512px-Germany1990WorldCupShirt.svg.png" },
            new Producto { Nombre = "Camiseta Francia 1998", Descripcion = "Zidane y la copa en casa", Precio = 13000, Stock = 9, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/05/France98kit.svg/512px-France98kit.svg.png" },
            new Producto { Nombre = "Camiseta Holanda 1988", Descripcion = "La naranja mecánica", Precio = 12500, Stock = 7, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0d/Netherlands1988Kit.svg/512px-Netherlands1988Kit.svg.png" },
            new Producto { Nombre = "Camiseta Inglaterra 1966", Descripcion = "Su única copa", Precio = 14500, Stock = 4, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/85/England66kit.svg/512px-England66kit.svg.png" },
            new Producto { Nombre = "Camiseta Italia 1982", Descripcion = "Paolo Rossi y la gloria", Precio = 15000, Stock = 6, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/bc/Italy1982kit.svg/512px-Italy1982kit.svg.png" },
            new Producto { Nombre = "Camiseta Uruguay 1950", Descripcion = "El Maracanazo", Precio = 16000, Stock = 5, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/02/Uruguay1950kit.svg/512px-Uruguay1950kit.svg.png" },
            new Producto { Nombre = "Camiseta España 2010", Descripcion = "Tiki-taka campeón", Precio = 15500, Stock = 6, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/Spain2010WorldCupShirt.svg/512px-Spain2010WorldCupShirt.svg.png" },
            new Producto { Nombre = "Camiseta Croacia 2018", Descripcion = "Subcampeón moderno", Precio = 12000, Stock = 5, ImagenUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/60/Croatia2018WorldCupKit.svg/512px-Croatia2018WorldCupKit.svg.png" }
        };

        context.Productos.AddRange(productos);
        context.SaveChanges();
    }
}
