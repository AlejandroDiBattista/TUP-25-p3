using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 5, "Guitarra acústica y eléctrica en un solo instrumento.", "/imagenes/acoustasonic-tele.png", "Fender Acoustasonic Telecaster", 1800m, 10 },
                    { 6, "Stratocaster de la serie Acoustasonic con gran versatilidad.", "/imagenes/acoustasonic-strato.png", "Fender Acoustasonic Stratocaster", 1500m, 8 },
                    { 7, "Stratocaster de la serie Standard con características clásicas.", "/imagenes/stratocaster-standard.png", "Fender Standard Stratocaster", 1200m, 12 },
                    { 8, "Telecaster con cuerpo verde y modificaciones personalizadas.", "/imagenes/telecaster-verde-modificada.png", "Fender Telecaster Verde Modificada", 1300m, 15 },
                    { 9, "Telecaster inspirada y modificada con el estilo de Mike Campbell.", "/imagenes/mike-campbell-telecaster.png", "Mike Campbell Telecaster", 1400m, 7 },
                    { 10, "Telecaster de la serie Vintage con un sonido al pasado.", "/imagenes/edicion-limitada-vintage-telecaster.png", "Fender Vintage Telecaster", 1600m, 9 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
