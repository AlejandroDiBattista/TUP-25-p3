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
                    { 1, "Smartphone 6.5''", "/img/celularx.jpg", "Celular X", 150000m, 10 },
                    { 2, "Bluetooth, cancelación de ruido", "/img/auricularespro.jpg", "Auriculares Pro", 35000m, 15 },
                    { 3, "Intel i7, 16GB RAM", "/img/notebookultra.jpg", "Notebook Ultra", 420000m, 5 },
                    { 4, "RGB, 12000 DPI", "/img/mousegamer.jpg", "Mouse Gamer", 12000m, 20 },
                    { 5, "Switch Blue, retroiluminado", "/img/tecladomecanico.jpg", "Teclado Mecánico", 18000m, 12 },
                    { 6, "Full HD, IPS", "/img/monitor24.jpg", "Monitor 24''", 65000m, 8 },
                    { 7, "USB-C, 30W", "/img/cargador.jpg", "Cargador Rápido", 7000m, 25 },
                    { 8, "Tipo C, 1m", "/img/cableusb.jpg", "Cable USB", 2500m, 30 },
                    { 9, "Bebida cola", "/img/gaseosa.jpg", "Gaseosa 1.5L", 1200m, 50 },
                    { 10, "Cableados, 3.5mm", "/img/auricularesinear.jpg", "Auriculares In-ear", 4000m, 18 }
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
