using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor.Migrations
{
    /// <inheritdoc />
    public partial class ProductosCalzadoIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Botas elegantes para el invierno", "https://i.imgur.com/NzYFQbj.jpg", "Botas de cuero", 14999m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Sandalias altas y cómodas para verano", "https://i.imgur.com/OnD3Quy.jpg", "Sandalias con plataforma", 8999m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Zapatos de vestir con taco bajo", "https://i.imgur.com/xVC0fWk.jpg", "Zapatos clásicos", 11999m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Calzado cómodo y moderno para todos los días", "https://i.imgur.com/yN6DR6V.jpg", "Zapatillas urbanas", 10499m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Botines de moda con cierre lateral", "https://i.imgur.com/tD9XHWE.jpg", "Botines con taco", 13999m, 8 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Livianas, ideales para el verano", "https://i.imgur.com/ZQIRSt7.jpg", "Sandalias cruzadas", 7999m, 18 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Brillantes y formales, para eventos especiales", "https://i.imgur.com/hOsUOAh.jpg", "Zapatos de charol", 12999m, 6 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Estilo vaquero en cuero sintético", "https://i.imgur.com/tJbEvOi.jpg", "Botas texanas", 14999m, 7 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Frescos, livianos y fáciles de calzar", "https://i.imgur.com/NHcvSGQ.jpg", "Zuecos de verano", 6499m, 25 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Elegancia para toda ocasión", "https://i.imgur.com/Drz98yD.jpg", "Zapatos nude", 11499m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Botas elegantes de cuero negro", "https://cdn.pixabay.com/photo/2016/11/29/04/17/boots-1867376_960_720.jpg", "Botas de Cuero", 25000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Sandalias brillantes ideales para verano", "https://cdn.pixabay.com/photo/2016/03/23/22/20/women-1274056_960_720.jpg", "Sandalias Doradas", 15000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Tacones clásicos color rojo pasión", "https://cdn.pixabay.com/photo/2015/03/26/09/54/shoes-690123_960_720.jpg", "Zapatos de Tacón Rojo", 18000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Zapatillas cómodas para uso diario", "https://cdn.pixabay.com/photo/2016/11/21/12/54/shoes-1840618_960_720.jpg", "Zapatillas Urbanas", 17000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Ballerinas elegantes y cómodas", "https://cdn.pixabay.com/photo/2016/03/23/23/17/ballet-1274376_960_720.jpg", "Ballerinas Rosadas", 12000m, 18 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Botines con diseño moderno y suave", "https://cdn.pixabay.com/photo/2021/03/10/10/14/shoes-6081546_960_720.jpg", "Botines Beige", 22000m, 11 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Altura y confort para días de calor", "https://cdn.pixabay.com/photo/2021/03/30/17/04/shoes-6138400_960_720.jpg", "Sandalias de Cuña", 14000m, 13 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Perfectos para eventos elegantes", "https://cdn.pixabay.com/photo/2017/08/06/12/32/shoes-2590240_960_720.jpg", "Zapatos de Fiesta", 20000m, 9 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Para un look fuerte y sofisticado", "https://cdn.pixabay.com/photo/2017/08/06/17/08/fashion-2590545_960_720.jpg", "Botas Largas", 27000m, 7 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Elegantes y frescos para cualquier ocasión", "https://cdn.pixabay.com/photo/2020/09/01/19/19/shoes-5534840_960_720.jpg", "Zapatos Blancos", 19000m });
        }
    }
}
