using TiendaOnline.Modelos;
namespace TiendaOnline.Datos
{
    public static class DbInitializer
    {
        public static void Inicializar(TiendaDbContext context)
        {
          
            
            if (context.Productos.Any())
            {
                return;
            }

            var productos = new List<Producto>
            {
                new Producto
                {
                Nombre = "Aloe Vera",
                Descripcion = "Planta suculenta ideal para interiores, fácil de cuidar.",
                Precio = 2500,
                Stock = 30,
                ImagenUrl = "https://images.unsplash.com/photo-1632380211596-b96123618ca8?q=80&w=464&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
                },

                new Producto
                {
                Nombre = "Ficus Benjamina",
                Descripcion = "Clásico de interiores. Árbol pequeño. Muy resistente.",
                Precio = 1800,
                Stock = 50,
                ImagenUrl = "https://dcdn-us.mitiendanube.com/stores/002/094/351/products/nurserylive-plants-ficus-benjamina-weeping-fig-plant_600x6001-e01d2e6814109bc61816509049660685-640-0.jpg"
                },

                new Producto
                {
                Nombre = "Pala de mano chica",
                Descripcion = "Herramienta de jardinería para trasplantes pequeños.",
                Precio = 1200,
                Stock = 40,
                ImagenUrl = "https://media.istockphoto.com/id/952363796/es/foto/tiro-corto-de-mujer-jardiner%C3%ADa-pala-en-mano-aislado-en-blanco.jpg?s=170667a&w=0&k=20&c=WagYPiVqNyDtpf_tHwfzVK6Tlll5r4KxYb4g86vLLXw="
                },

                new Producto
                {
                Nombre = "Fertilizante orgánico líquido 500ml",
                Descripcion = "Ideal para fortalecer raíces y hojas de plantas verdes.",
                Precio = 3200,
                Stock = 25,
                ImagenUrl = "https://http2.mlstatic.com/D_NQ_NP_917589-MLA71654824104_092023-O.webp"
                },

                new Producto
                {
                Nombre = "Suculenta Echeveria",
                Descripcion = "Planta ornamental de hojas gruesas en forma de roseta.",
                Precio = 2200,
                Stock = 35,
                ImagenUrl = "https://images.unsplash.com/photo-1604188167973-ff43bd3eb9d4?q=80&w=435&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
                },

                new Producto
                {
                Nombre = "Cactus San Pedro",
                Descripcion = "Alto y resistente, perfecto para exteriores. Rústico y es fácil de cuidar.",
                Precio = 4500,
                Stock = 18,
                ImagenUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRiuyJU8_hi6xzVowEwj6JKOkEvLX0cY3WwWQ&s"
                },

                new Producto {
                Nombre = "Cactus pequeño en maceta",
                Descripcion = "Cactus mini decorativo, requiere poco riego.",
                Precio = 1900,
                Stock = 40,
                ImagenUrl = "https://img.freepik.com/fotos-premium/muchos-cactus-pequenos-suculentas-macetas_165293-630.jpg"
                },

                new Producto {
                Nombre = "Tierra abonada x5kg",
                Descripcion = "Mezcla de tierra negra con compost orgánico.",
                Precio = 2800,
                Stock = 20,
                ImagenUrl = "https://http2.mlstatic.com/D_668810-MLA76373980268_052024-C.jpg"
                },

                new Producto
                {
                Nombre = "Helecho Boston",
                Descripcion = "Perfecto para colgar. Hojas largas que caen en forma de cascada.",
                Precio = 6800,
                Stock = 10,
                ImagenUrl = "https://images.prismic.io/begreen/67a9b649-9ef3-4959-bf4a-f34a442862ad_1_1_helecho.jpg?auto=compress,format&rect=0,0,1600,1600&w=680&h=680"
                },

                new Producto
                {
                Nombre = "Palo de agua (Dracaena fragrans)",
                Descripcion = "Planta de interior con tallo largo y hojas verdes brillantes.",
                Precio = 4700,
                Stock = 12,
                ImagenUrl = "https://naugreen.com/cdn/shop/files/1_45a1d321-8396-4300-836a-35d0ef0171ac_1800x1800.jpg?v=1710715695"
            }
            };

            context.Productos.AddRange(productos);
            context.SaveChanges();
        }
    }
}


