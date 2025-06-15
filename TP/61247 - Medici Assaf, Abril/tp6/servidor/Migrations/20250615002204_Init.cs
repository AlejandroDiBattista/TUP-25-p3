using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 1,
                column: "ImagenUrl",
                value: "img.catalogo/baggie_blanco.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 2,
                column: "ImagenUrl",
                value: "img.catalogo/baggie_negro.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 3,
                column: "ImagenUrl",
                value: "img.catalogo/bermuda.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 4,
                column: "ImagenUrl",
                value: "img.catalogo/buzo_marron.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 5,
                column: "ImagenUrl",
                value: "img.catalogo/buzo_negro.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 6,
                column: "ImagenUrl",
                value: "img.catalogo/camisa.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 8,
                column: "ImagenUrl",
                value: "img.catalogo/gorra.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 9,
                column: "ImagenUrl",
                value: "img.catalogo/jean_logo.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 10,
                column: "ImagenUrl",
                value: "img.catalogo/pantalon_camuflado.jpeg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 1,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/baggie_blanco.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 2,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/baggie_negro.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 3,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/bermuda.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 4,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/buzo_marron.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 5,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/buzo_negro.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 6,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/camisa.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 8,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/gorra.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 9,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/jean_logo.jpeg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 10,
                column: "ImagenUrl",
                value: "wwwroot/img.catalogo/oantalon_camuflado.jpeg");
        }
    }
}
