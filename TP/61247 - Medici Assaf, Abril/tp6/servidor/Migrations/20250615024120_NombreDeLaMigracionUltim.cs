using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeLaMigracionUltim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 1,
                column: "ImagenUrl",
                value: "img.catalogo/baggie_blanco.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 6,
                column: "ImagenUrl",
                value: "img.catalogo/camisa.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                keyValue: 6,
                column: "ImagenUrl",
                value: "img.catalogo/camisa.jpeg");
        }
    }
}
