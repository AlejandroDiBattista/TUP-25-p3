using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class ActualizaNombres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 1,
                columns: new[] { "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[] { "Pantalon de algodon con estampado.", "wwwroot/img.catalogo/baggie_blanco.jpg", "Baggie Crema", 35200m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 2,
                columns: new[] { "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[] { "Pantalon de algodon con estampado.", "wwwroot/img.catalogo/baggie_negro.jpeg", "Baggie Negro", 35200m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 3,
                columns: new[] { "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[] { "Bermuda de jean", "wwwroot/img.catalogo/bermuda.jpeg", "Bermuda", 25000m });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "ProductoId", "CantidadDisponible", "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[,]
                {
                    { 4, 15, "Buzo de algodon bordado.", "wwwroot/img.catalogo/buzo_marron.jpg", "Buzo marron", 40000m },
                    { 5, 15, "Buzo de algodon bordado", "wwwroot/img.catalogo/buzo_negro.jpeg", "Buzo negro", 40000m },
                    { 6, 15, "Camisa mangas cortas", "wwwroot/img.catalogo/camisa.jpeg", "Camisa negra", 37000m },
                    { 8, 15, "Gorra gris gastado", "wwwroot/img.catalogo/gorra.jpeg", "Gorra", 15000m },
                    { 9, 15, "Pantalon de jean con bordado", "wwwroot/img.catalogo/jean_logo.jpeg", "Jean Ahumado", 40000m },
                    { 10, 15, "Pantalon cargo camuflado gris", "wwwroot/img.catalogo/oantalon_camuflado.jpeg", "Pantalon cargo", 41500m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 1,
                columns: new[] { "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[] { "RGB, alta precisión", "img/mouse.jpg", "Mouse Gamer", 5500m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 2,
                columns: new[] { "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[] { "Switch blue, retroiluminado", "img/teclado.jpg", "Teclado Mecánico", 10000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "ProductoId",
                keyValue: 3,
                columns: new[] { "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[] { "Bluetooth, cancelación de ruido", "img/auriculares.jpg", "Auriculares", 7800m });
        }
    }
}
