using servidor.Models;

namespace servidor.Data;

public static class DbSeeder
{
    public static void Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TiendaDbContext>();

        if (!db.Productos.Any())
        {
            db.Productos.AddRange(
                new Producto {
                    Nombre = "Dior Sauvage",
                    Descripcion = "Perfume intenso para hombre",
                    Precio = 580000,
                    Stock = 10,
                    ImagenUrl = "https://acdn-us.mitiendanube.com/stores/002/708/808/products/183722-800-auto1-5a1e28c8a90f14c83516862423635940-640-0.jpg"
                },
                new Producto {
                    Nombre = "YSL Black Opium",
                    Descripcion = "Aromático y elegante",
                    Precio = 720000,
                    Stock = 9,
                    ImagenUrl = "https://www.rp-luxury.com/5054-thickbox_default/yves-saint-laurent-black-opium-eau-de-parfum.jpg"
                },
                new Producto {
                    Nombre = "Paco Rabanne 1 Million",
                    Descripcion = "Estilo y presencia",
                    Precio = 550000,
                    Stock = 14,
                    ImagenUrl = "https://www.farmaciasrp.com.ar/22848-thickbox_default/paco-rabanne-1-million-elixir-parfum-intense-100-ml.jpg"
                },
                new Producto {
                    Nombre = "Versace Eros",
                    Descripcion = "Fresco y seductor",
                    Precio = 560000,
                    Stock = 12,
                    ImagenUrl = "https://i.pinimg.com/736x/c8/6f/b6/c86fb6c97c6eb352dcf8edf10c4bc26f.jpg"
                },
                new Producto {
                    Nombre = "Bleu de Chanel",
                    Descripcion = "El clásico moderno",
                    Precio = 790000,
                    Stock = 6,
                    ImagenUrl = "https://www.shutterstock.com/image-photo/riga-latvia-october-14-2022-600nw-2363729767.jpg"
                },
                new Producto {
                    Nombre = "Jean Paul Gaultier Le Male",
                    Descripcion = "Icónico y masculino",
                    Precio = 520000,
                    Stock = 11,
                    ImagenUrl = "https://www.farmaciasrp.com.ar/33698-thickbox_default/jean-paul-gaultier-le-male-elixir-absolu-parfum-intense-125-ml.jpg"
                },
                new Producto {
                    Nombre = "Carolina Herrera 212 VIP",
                    Descripcion = "Fiesta y elegancia",
                    Precio = 650000,
                    Stock = 8,
                    ImagenUrl = "https://cdn.vesira.com/media/catalog/product/cache/1/image/650x/040ec09b1e35df139433887a97daa66f/c/a/carolina-herrera-212-vip-club-edition-eau-de-toilette-vaporizador-80-ml.jpg"
                },
                new Producto {
                    Nombre = "Dolce & Gabbana Light Blue",
                    Descripcion = "Ligero y veraniego",
                    Precio = 590000,
                    Stock = 7,
                    ImagenUrl = "https://www.farmaciassanchezantoniolli.com.ar/9652-medium_default/dyg-light-blue-men-x125v-edt.jpg"
                },
                new Producto {
                    Nombre = "Armani Acqua di Gio",
                    Descripcion = "Frescura masculina",
                    Precio = 630000,
                    Stock = 13,
                    ImagenUrl = "https://martshop.com.ua/image/cache/catalog/extended-reviews/parfums/img_4010-220x220.jpg"
                },
                new Producto {
                    Nombre = "Lancôme La Vie Est Belle",
                    Descripcion = "Femenino y alegre",
                    Precio = 700000,
                    Stock = 10,
                    ImagenUrl = "https://cdn.notinoimg.com/detail_main_lq/lancome/3614274336252_03-o/la-vie-est-belle-sparkling-edition___250306.jpg"
                }
            );

            db.SaveChanges();
        }
    }
}