using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servidor.Migrations
{
    /// <inheritdoc />
    public partial class CargarProductosConImagenesLocales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/botaslargas.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/sandaliasconplataforma.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl" },
                values: new object[] { "Zapatos de vestir con taco", "http://localhost:5164/img/zapatosclasicos.webp" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/zapatillasurbanas.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/botinesdetaco.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/sandaliascruzadas.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/zapatosdecharol.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/botastexanas.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/suecosdeverano.webp");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImagenUrl",
                value: "http://localhost:5164/img/zapatosnude.webp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/21/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/25/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "ImagenUrl" },
                values: new object[] { "Zapatos de vestir con taco bajo", "https://picsum.photos/id/36/300/200" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/47/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/54/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/65/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/72/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/83/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/94/300/200");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImagenUrl",
                value: "https://picsum.photos/id/105/300/200");
        }
    }
}
