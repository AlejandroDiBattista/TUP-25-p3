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
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre" },
                values: new object[] { "Teléfono de última generación.", "/images/telefono.jpg", "Iphone 16 Pro Max" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Auriculares JBL");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Mouse Gamer Logitech");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ImagenUrl", "Nombre" },
                values: new object[] { "/images/camara.jpg", "Camara Full HD" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "Nombre",
                value: "Router Wi-Fi ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre" },
                values: new object[] { "Teléfono de última generación con cámara de 108MP y pantalla AMOLED.", "/images/smartphone.jpg", "Smartphone X10" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Auriculares Bluetooth");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Mouse Gamer Inalámbrico");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ImagenUrl", "Nombre" },
                values: new object[] { "/images/webcam.jpg", "Webcam Full HD" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "Nombre",
                value: "Router Wi-Fi 6");
        }
    }
}
