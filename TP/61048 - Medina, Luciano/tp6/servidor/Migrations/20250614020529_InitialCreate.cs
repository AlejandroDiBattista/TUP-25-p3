using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    NombreCliente = table.Column<string>(type: "TEXT", nullable: true),
                    ApellidoCliente = table.Column<string>(type: "TEXT", nullable: true),
                    EmailCliente = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: true),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    Precio = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagenUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemsCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsCompra_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsCompra_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, "Clásico e icónico, floral-aldehídico.", "https://ejemplo.com/chanel_no5.jpg", "Chanel N°5", 150.00m, 25 },
                    { 2, "Fresco y amaderado, para hombres.", "https://ejemplo.com/dior_sauvage.jpg", "Dior Sauvage", 120.00m, 30 },
                    { 3, "Oscuro, opulento y especiado.", "https://ejemplo.com/tom_ford_black_orchid.jpg", "Tom Ford Black Orchid", 200.00m, 15 },
                    { 4, "Fresco y mineral, unisex.", "https://ejemplo.com/jo_malone_woodsage.jpg", "Jo Malone Wood Sage & Sea Salt", 90.00m, 40 },
                    { 5, "Floral blanco, empolvado.", "https://ejemplo.com/gucci_bloom.jpg", "Gucci Bloom", 110.00m, 20 },
                    { 6, "Amaderado especiado, para hombres.", "https://ejemplo.com/paco_rabanne_1million.jpg", "Paco Rabanne 1 Million", 95.00m, 35 },
                    { 7, "Gourmand floral, dulce.", "https://ejemplo.com/lancome_lavie.jpg", "Lancôme La Vie Est Belle", 105.00m, 28 },
                    { 8, "Aromático fougère, para hombres.", "https://ejemplo.com/versace_eros.jpg", "Versace Eros", 85.00m, 45 },
                    { 9, "Oriental especiado, café, vainilla.", "https://ejemplo.com/ysl_black_opium.jpg", "YSL Black Opium", 115.00m, 22 },
                    { 10, "Floral oriental, dulce.", "https://ejemplo.com/ch_good_girl.jpg", "Carolina Herrera Good Girl", 130.00m, 18 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsCompra_CompraId",
                table: "ItemsCompra",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsCompra_ProductoId",
                table: "ItemsCompra",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsCompra");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
