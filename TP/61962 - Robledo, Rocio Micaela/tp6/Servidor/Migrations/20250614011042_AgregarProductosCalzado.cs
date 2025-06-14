using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Servidor.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProductosCalzado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Botas altas de cuero genuino", "https://example.com/botas-cuero.jpg", "Botas de Cuero", 21999m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Sandalias con taco de corcho", "https://example.com/sandalias-plataforma.jpg", "Sandalias Plataforma", 14999m, 15 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Zapatillas casuales para todos los días", "https://example.com/zapatillas-urbanas.jpg", "Zapatillas Urbanas", 18999m, 20 });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 4, "Botines elegantes con taco cuadrado", "https://example.com/botines-taco.jpg", "Botines con Taco", 25999m, 12 },
                    { 5, "Sandalias estilo romano de cuero sintético", "https://example.com/sandalias-romanas.jpg", "Sandalias Romanas", 13499m, 18 },
                    { 6, "Zuecos altos con diseño moderno", "https://example.com/zuecos.jpg", "Zuecos de Plataforma", 16999m, 10 },
                    { 7, "Balerinas cómodas y versátiles", "https://example.com/balerinas.jpg", "Balerinas Clásicas", 9999m, 25 },
                    { 8, "Botas estilo tejano con bordado", "https://example.com/botas-texanas.jpg", "Botas Texanas", 23999m, 8 },
                    { 9, "Sandalias de color nude con tiras finas", "https://example.com/sandalias-nude.jpg", "Sandalias Nude", 11999m, 16 },
                    { 10, "Zapatillas de suela alta, estilo urbano", "https://example.com/zapatillas-chunky.jpg", "Zapatillas Chunky", 20999m, 14 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Galaxy A32", "https://via.placeholder.com/200", "Celular Samsung", 150000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Core i5, 8GB RAM", "https://via.placeholder.com/200", "Notebook Lenovo", 350000m, 5 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Bluetooth", "https://via.placeholder.com/200", "Auriculares JBL", 25000m, 15 });
        }
    }
}
