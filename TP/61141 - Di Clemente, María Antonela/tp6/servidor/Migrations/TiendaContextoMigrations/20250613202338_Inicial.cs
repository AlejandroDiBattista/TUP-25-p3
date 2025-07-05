using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations.TiendaContextoMigrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
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
                    ImagenUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Categoria = table.Column<string>(type: "TEXT", nullable: true)
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
                columns: new[] { "Id", "Categoria", "Descripcion", "ImagenUrl", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, "", "Pantalla 6.6'' - 128GB - 4GB RAM", "img/a.jpg", "Samsung A06", 280000m, 15 },
                    { 2, "", "Smartphone de Apple con chip A17 Pro y cámara avanzada", "img/iphone.jpg", "iPhone 15 Pro Max", 1950000m, 30 },
                    { 3, "", "Notebook liviana con procesador Apple Silicon M3", "img/macbook-air.jpg", "Apple MacBook Air M3", 2100000m, 15 },
                    { 4, "", "Consola de videojuegos de Sony con diseño más delgado", "img/PlayStation5.jpg", "PlayStation 5 Slim", 950000m, 25 },
                    { 5, "", "Consola de Microsoft con 1 TB de almacenamiento y juegos 4K", "img/Microsoft-Xbox-Series-X.jpg", "Xbox Series X", 870000m, 12 },
                    { 6, "", "Intel i5 - 8GB RAM - 256GB SSD", "img/notebook-hp-.jpg", "Notebook HP 14", 385000m, 5 },
                    { 7, "", "Ergonómica, reclinable, color negro y rojo", "img/sillaGamer.jpg", "Silla Gamer", 115000m, 7 },
                    { 8, "", "Reloj inteligente con sensores de salud y chip S9.", "img/aple-watch.jpg", "Apple Watch Series 9", 420000m, 50 },
                    { 9, "", "Full HD - HDMI - VGA", "img/monitor.jpg", "Monitor LED 24", 95000m, 10 },
                    { 10, "", "Auriculares con cancelación de ruido y sonido espacial", "img/AirPods-si.jpg", "AirPods Pro 2 (USB-C)", 127000m, 18 }
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
