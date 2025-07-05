using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class SeedProductosConImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Color: Ultramarine", "img/iPhone16.jpg", "iPhone 16", 950000m, 20 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Color: Pink", "img/iPhone15.jpg", "iPhone 15", 720000m, 20 });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 3, "Color: Gold", "img/iPhone14Pro.jpg", "iPhone 14 Pro", 620000m, 20 },
                    { 4, "Color: Graphite", "img/iPhone13Pro.jpg", "iPhone 13 Pro", 520000m, 20 },
                    { 5, "Color: Red", "img/iPhone12.jpg", "iPhone 12", 320000m, 20 },
                    { 6, "Color: White", "img/iPhone11.jpg", "iPhone 11", 220000m, 20 },
                    { 7, "Color: Blue", "img/iPhoneXR.jpg", "iPhone XR", 120000m, 20 },
                    { 8, "Color: Red", "img/FundaSilicona.jpg", "Funda Silicona iPhone 12", 10000m, 20 },
                    { 9, "Color: Gray", "img/iPadAir5.jpg", "iPad Air 5", 720000m, 20 },
                    { 10, "Cargador 20w más cable Lightning", "img/CargadorConCable.jpg", "Cargador Original Apple", 20000m, 20 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);

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
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Color Blue", "url1", "iPhone 13 Pro", 650000m, 15 });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[] { "Color Black", "url2", "iPhone 12", 420000m, 5 });
        }
    }
}
