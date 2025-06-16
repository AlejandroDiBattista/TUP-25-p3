using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operaciones",
                columns: table => new
                {
                    OperacionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: true),
                    Apellido = table.Column<string>(type: "TEXT", nullable: true),
                    CorreoElectronico = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operaciones", x => x.OperacionId);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", nullable: true),
                    Detalle = table.Column<string>(type: "TEXT", nullable: true),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    CantidadDisponible = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagenUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoId);
                });

            migrationBuilder.CreateTable(
                name: "ItemsOperaciones",
                columns: table => new
                {
                    ItemOperacionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false),
                    OperacionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsOperaciones", x => x.ItemOperacionId);
                    table.ForeignKey(
                        name: "FK_ItemsOperaciones_Operaciones_OperacionId",
                        column: x => x.OperacionId,
                        principalTable: "Operaciones",
                        principalColumn: "OperacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsOperaciones_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "ProductoId", "CantidadDisponible", "Detalle", "ImagenUrl", "Titulo", "Valor" },
                values: new object[,]
                {
                    { 1, 12, "RGB, alta precisión", "img/mouse.jpg", "Mouse Gamer", 5500m },
                    { 2, 8, "Switch blue, retroiluminado", "img/teclado.jpg", "Teclado Mecánico", 10000m },
                    { 3, 15, "Bluetooth, cancelación de ruido", "img/auriculares.jpg", "Auriculares", 7800m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsOperaciones_OperacionId",
                table: "ItemsOperaciones",
                column: "OperacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsOperaciones_ProductoId",
                table: "ItemsOperaciones",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsOperaciones");

            migrationBuilder.DropTable(
                name: "Operaciones");

            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
