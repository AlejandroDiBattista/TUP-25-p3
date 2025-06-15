using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarDatosProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Notebook Profesional y Gaming", "https://acdn-us.mitiendanube.com/stores/001/156/703/products/notebook-gamer-asus-rog-strix-g17-g713pv-ws94-17-ryzen-9-7845hx-1tb-ssd-16gb-rtx-4060-copia-4a3f8882e47fbcca8317314309110054-1024-1024.png", "Notebook Gamer ASUS Rog Strix G17", 3465000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Auriculares para Audio Profesional", "https://m.media-amazon.com/images/I/71BR7ivLOAL.jpg", "Audio Technica ATH-M50X", 300000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImagenUrl", "Nombre" },
                values: new object[] { "https://spacegamer.com.ar/img/Public/1058-producto-203bb-8934.jpg", "Logitech G203" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Teclado con switches mecánicos", "https://www.newmaster.com.ar/wp-content/uploads/2021/08/1-1.jpg", "HyperX Alloy Origins", 135000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "https://acdn-us.mitiendanube.com/stores/001/097/819/products/apple-watch-serie-se-44mm1-ef69a751ed99e1572d16758829200917-1024-1024.png", "Apple Watch SE", 620000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "https://http2.mlstatic.com/D_NQ_NP_927502-MLU75081110091_032024-O.webp", "Samgung Galaxy Tab A9", 235000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "https://www.oscarbarbieri.com/pub/media/catalog/product/cache/7baadf0dec41407c7702efdbff940ecb/4/4/44a9f81c092a58e6af72a2b902f8c330.jpg", "Parlante JBL Go", 60000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Disco super rápido", "https://compucordoba.com.ar/img/Public/1078-producto-d-nq-np-699067-mla31583397158-072019-o1-614.jpg", "Disco SSD WD Green 480 GB", 29000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "USB 3.1", "https://www.torca.com.ar/images/00000000000622444173364.jpg", "Pendrive Kingston 64GB", 9000m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Notebook profesional", "https://via.placeholder.com/150", "Notebook Pro", 250000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Auriculares Bluetooth", "https://via.placeholder.com/150", "Auriculares", 30000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImagenUrl", "Nombre" },
                values: new object[] { "https://via.placeholder.com/150", "Mouse Gamer" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Teclado con switches", "https://via.placeholder.com/150", "Teclado Mecánico", 20000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "https://via.placeholder.com/150", "Smartwatch", 50000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "https://via.placeholder.com/150", "Tablet 10\"", 75000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "https://via.placeholder.com/150", "Parlante Bluetooth", 18000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Carga en 30 min", "https://via.placeholder.com/150", "Cargador Rápido", 8000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "USB 3.0", "https://via.placeholder.com/150", "Pendrive 64GB", 7000m });
        }
    }
}
