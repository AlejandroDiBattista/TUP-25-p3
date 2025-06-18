using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "vaso1.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "manta.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagenUrl",
                value: "pipeta.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagenUrl",
                value: "matraz.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagenUrl",
                value: "balanza.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagenUrl",
                value: "agitador.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagenUrl",
                value: "tubos.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagenUrl",
                value: "probeta.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImagenUrl",
                value: "cronometro.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImagenUrl",
                value: "termometro.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagenUrl",
                value: "img/vaso1.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagenUrl",
                value: "img/manta.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagenUrl",
                value: "img/pipeta.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagenUrl",
                value: "img/matraz.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagenUrl",
                value: "img/balanza.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagenUrl",
                value: "img/agitador.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagenUrl",
                value: "img/tubos.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagenUrl",
                value: "img/probeta.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImagenUrl",
                value: "img/cronometro.jpg");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImagenUrl",
                value: "img/termometro.jpg");
        }
    }
}
