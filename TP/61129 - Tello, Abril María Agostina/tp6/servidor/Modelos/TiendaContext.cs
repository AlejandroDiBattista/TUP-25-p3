using Microsoft.EntityFrameworkCore;

namespace servidor.Modelos
{
    public class TiendaContext : DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItemsCompra { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(
                new Producto { Id = 1, Nombre = "Rubor Líquido Soft Pinch", Descripcion = "Rubor líquido de acabado natural y larga duración.", Precio = 26130, Stock = 15, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/815/078/products/portada12-19-a-las-00-55-27-94be91828ba1cde6f517391678256935-1024-1024.png" },
                new Producto { Id = 2, Nombre = "Bronceador en barra Warm Wishes", Descripcion = "Bronceador en stick de fácil aplicación y acabado cálido natural.", Precio = 33400, Stock = 25, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/346/493/products/tmp_b64_88e2b83a-ede9-4d6c-ac27-c343579e84f6_346493_351567-81bc839ca5018bf6a817359647079875-1024-1024.webp" },
                new Producto { Id = 3, Nombre = "Iluminador Líquido Positive Light", Descripcion = "Iluminador líquido de acabado radiante y natural.", Precio = 32000, Stock = 21, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/346/493/products/tmp_b64_da5de96a-8610-4dcb-bf7d-9feb3ad0fdb4_346493_351567-77c909521977f7354b17359647963596-480-0.webp" },
                new Producto { Id = 4, Nombre = "Corrector Líquido Iluminador Liquid Touch", Descripcion = "Corrector líquido iluminador de cobertura ligera a media y acabado natural.", Precio = 28500, Stock = 30, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/346/493/products/tmp_b64_29a217ac-a90b-49f5-87ed-653c4359f708_346493_351567-914c074f0da0bed98717359641085224-640-0.webp" },
                new Producto { Id = 5, Nombre = "Máscara de Pestañas Voluminizadora Universal Perfect Strokes", Descripcion = "Máscara voluminizadora de acabado intenso y aplicador universal.", Precio = 26000, Stock = 18, ImagenUrl = "https://i0.wp.com/www.hebe-ec.com/wp-content/uploads/2023/05/RIMEL-BLACK.png" },
                new Producto { Id = 6, Nombre = "Iluminador Sedoso Positive Light", Descripcion = "Iluminador en polvo de textura sedosa y acabado radiante.", Precio = 32000, Stock = 20, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/346/493/products/tmp_b64_b09921e4-b3a0-4257-bd2c-62ef075d16d8_346493_351567-9503a791219d2c0f4517359648796455-1024-1024.webp" },
                new Producto { Id = 7, Nombre = "Bruma Perfumada Corporal y Capilar Find Comfort", Descripcion = "Bruma perfumada para cuerpo y cabello con aroma suave y relajante.", Precio = 34000, Stock = 12, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/346/493/products/tmp_b64_f6a0820d-1f1a-4f25-98cd-9d7f6d4fd205_346493_351567-6ae2916a3d4de98da217359643129581-480-0.webp" },
                new Producto { Id = 8, Nombre = "Lip Oil Soft Pinch", Descripcion = "Lip Oil con tinte hidratante y acabado brillante natural.", Precio = 26500, Stock = 22, ImagenUrl = "https://acdn-us.mitiendanube.com/stores/346/493/products/tmp_b64_0614d549-bc1f-477b-80a2-747bf5d982ec_346493_351567-39660061903abe0cb917359646792292-1024-1024.webp" },
                new Producto { Id = 9, Nombre = "Delineador de Labios Mate Kind Words", Descripcion = "Delineador mate de labios con aplicación suave y definición precisa.", Precio = 19000, Stock = 16, ImagenUrl = "https://blushinmakeup.com/cdn/shop/files/3091305d-daed-4915-81fb-f05df04b84f1_1024x1024.png?v=1742435603" },
                new Producto { Id = 10, Nombre = "Bálsamo Labial con Brillo With Gratitude", Descripcion = "Bálsamo labial con color y brillo natural para hidratación duradera.", Precio = 21000, Stock = 14, ImagenUrl = "https://vibeofbeauty.com/wp-content/uploads/2023/03/BarraDeLabiosMate_RareBeauty-2.png" }

            );
        }
    }
}
