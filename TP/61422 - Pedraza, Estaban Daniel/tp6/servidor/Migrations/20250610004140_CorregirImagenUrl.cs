using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class CorregirImagenUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "/imagenes/american-luxe-telecaster.png");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "/imagenes/american-telecaster-blanca.png");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagenUrl",
                value: "/imagenes/stratocaster-professional-ii.png");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagenUrl",
                value: "/imagenes/vintage-telecaster.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "imagenes/american-luxe-telecaster.png");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "imagenes/american-telecaster-blanca.png");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagenUrl",
                value: "imagenes/stratocaster-professional-ii.png");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagenUrl",
                value: "imagenes/vintage-telecaster.png");
        }
    }
}
