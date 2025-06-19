using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor.Migrations
{
    /// <inheritdoc />
    public partial class CambiarProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Precio" },
                values: new object[] { "Botas elegantes de cuero negro", "https://cdn.pixabay.com/photo/2016/11/29/04/17/boots-1867376_960_720.jpg", 25000m });

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
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Tacones clásicos color rojo pasión", "https://cdn.pixabay.com/photo/2015/03/26/09/54/shoes-690123_960_720.jpg", "Zapatos de Tacón Rojo", 18000m, 12 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Zapatillas cómodas para uso diario", "https://cdn.pixabay.com/photo/2016/11/21/12/54/shoes-1840618_960_720.jpg", "Zapatillas Urbanas", 17000m, 20 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Ballerinas elegantes y cómodas", "https://cdn.pixabay.com/photo/2016/03/23/23/17/ballet-1274376_960_720.jpg", "Ballerinas Rosadas", 12000m });

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
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Elegantes y frescos para cualquier ocasión", "https://cdn.pixabay.com/photo/2020/09/01/19/19/shoes-5534840_960_720.jpg", "Zapatos Blancos", 19000m, 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Precio" },
                values: new object[] { "Botas altas de cuero genuino", "https://example.com/botas-cuero.jpg", 21999m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Sandalias con taco de corcho", "https://example.com/sandalias-plataforma.jpg", "Sandalias Plataforma", 14999m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Zapatillas casuales para todos los días", "https://example.com/zapatillas-urbanas.jpg", "Zapatillas Urbanas", 18999m, 20 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Botines elegantes con taco cuadrado", "https://example.com/botines-taco.jpg", "Botines con Taco", 25999m, 12 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Sandalias estilo romano de cuero sintético", "https://example.com/sandalias-romanas.jpg", "Sandalias Romanas", 13499m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Zuecos altos con diseño moderno", "https://example.com/zuecos.jpg", "Zuecos de Plataforma", 16999m, 10 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Balerinas cómodas y versátiles", "https://example.com/balerinas.jpg", "Balerinas Clásicas", 9999m, 25 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Botas estilo tejano con bordado", "https://example.com/botas-texanas.jpg", "Botas Texanas", 23999m, 8 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Sandalias de color nude con tiras finas", "https://example.com/sandalias-nude.jpg", "Sandalias Nude", 11999m, 16 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Zapatillas de suela alta, estilo urbano", "https://example.com/zapatillas-chunky.jpg", "Zapatillas Chunky", 20999m, 14 });
        }
    }
}
