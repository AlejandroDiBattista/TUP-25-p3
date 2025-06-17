using servidor.Models;

namespace servidor.Data;

public static class DbInitializer {
    public static void Inicializar(TiendaContext context) {
        context.Database.EnsureCreated();
        if (context.Productos.Any()) return;

        var productos = new List<Producto> {
            new Producto { Nombre = "Camiseta Argentina 1986", Descripcion = "Replica oficial del mundial 86", Precio = 15000, Stock = 10, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/argentina-titular-retro-1986-2-jpeg-23a29062b44768f98b17175631424300-1024-1024.jpg" },
            new Producto { Nombre = "Camiseta Brasil 1970", Descripcion = "Histórica camiseta de Pelé", Precio = 14000, Stock = 8, ImagenUrl = "https://www.retrofootball.es/media/catalog/product/cache/9/image/1300x1300/9df78eab33525d08d6e5fb8d27136e95/3/0/3011_brazil_1970_retrofootball.jpg" },
            new Producto { Nombre = "Camiseta Alemania 1990", Descripcion = "Final contra Argentina", Precio = 13500, Stock = 6, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/alemania-titular-retro-1990-1-bffb8fb80eb523351217175586180282-1024-1024.png" },
            new Producto { Nombre = "Camiseta Francia 1998", Descripcion = "Zidane y la copa en casa", Precio = 13000, Stock = 9, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/francia-titular-retro-1998-1-39d690cf4113d3ed7117190299783075-1024-1024.jpg" },
            new Producto { Nombre = "Camiseta Holanda 1988", Descripcion = "La naranja mecánica", Precio = 12500, Stock = 7, ImagenUrl = "https://lamaquinaretro.com.ar/wp-content/uploads/2022/04/G-5.jpg" },
            new Producto { Nombre = "Camiseta Inglaterra 1966", Descripcion = "Su única copa", Precio = 14500, Stock = 4, ImagenUrl = "https://www.vintagefootballclub.com/wp-content/uploads/2018/06/england-1966-moore-1-800x800.jpg" },
            new Producto { Nombre = "Camiseta Italia 1982", Descripcion = "Paolo Rossi y la gloria", Precio = 15000, Stock = 6, ImagenUrl = "https://lamaquinaretro.com.ar/wp-content/uploads/2022/05/4-6.jpg" },
            new Producto { Nombre = "Camiseta Uruguay 1950", Descripcion = "El Maracanazo", Precio = 16000, Stock = 5, ImagenUrl = "https://m.media-amazon.com/images/I/51N3CtZ2yyL._AC_SX569_.jpg" },
            new Producto { Nombre = "Camiseta España 2010", Descripcion = "Tiki-taka campeón", Precio = 15500, Stock = 6, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/espana-suplente-azul-retro-2010-iniesta-mundial-sudafrica-2010-1-fd43143d1a4453000217188592180194-1024-1024.jpg" },
            new Producto { Nombre = "Camiseta Croacia 2018", Descripcion = "Subcampeón moderno", Precio = 12000, Stock = 5, ImagenUrl = "https://www.camisetasstore.com/webp/CAMISETAS-DE-F%C3%9ATBOL-Croacia-2018---primera-550.webp" },
            new Producto { Nombre = "Camiseta Boca Juniors 2001", Descripcion = "Época dorada con Bianchi y Riquelme", Precio = 13500, Stock = 7, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/boca-juniors-titular-retro-2001-10-roman-1-jpeg-bad8696770a792546117188351214809-1024-1024.jpg" },
            new Producto { Nombre = "Camiseta River Plate 1996", Descripcion = "Copa Libertadores con Francescoli", Precio = 14000, Stock = 6, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/004/269/759/products/tit-95-96-retro-1-20664951e73f58da3317074259898294-640-0.jpg" },
            new Producto { Nombre = "Camiseta AC Milan 2007 Suplente", Descripcion = "Campeón de Champions con Kaká", Precio = 15000, Stock = 5, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/ac-milan-suplente-blanca-retro-2006-2007-final-uefa-champions-league-99-ronaldo-1-6f72c7ec37cf44d6c217417473761444-1024-1024.png" },
            new Producto { Nombre = "Camiseta Manchester United 2008", Descripcion = "Cristiano Ronaldo y la Champions League", Precio = 14500, Stock = 8, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/manchester-united-titular-retro-2008-final-uefa-champions-league-ucl-7-ronaldo-2-jpeg-b9f285d7ea979f11e117335992175885-1024-1024.jpg" },
            new Producto { Nombre = "Camiseta Barcelona 2009", Descripcion = "El equipo de Guardiola", Precio = 15500, Stock = 9, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/003/574/products/barcelona-titular-retro-2009-final-ucl-10-messi-1-jpeg-titular-retro-2009-final-ucl-10-messi-2-jpeg-23016c9d9d3eea9d2e17189014839215-1024-1024.jpg" }
        };

        context.Productos.AddRange(productos);
        context.SaveChanges();
    }
}
