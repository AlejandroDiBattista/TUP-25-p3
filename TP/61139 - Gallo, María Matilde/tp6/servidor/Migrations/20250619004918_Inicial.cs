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
                name: "Carritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carritos", x => x.Id);
                });

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
                name: "Items",
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
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsCarrito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CarritoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsCarrito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsCarrito_Carritos_CarritoId",
                        column: x => x.CarritoId,
                        principalTable: "Carritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsCarrito_Productos_ProductoId",
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
                    { 1, "Diseño rosa con verde. Plataforma triple y gamuza supersuave para un estilo clásico renovado.", "zapa1.jpg.webp", "Zapatillas Adidas Gazelle Bold", 150000m, 50 },
                    { 2, "Verde y rosa vibrantes. Las adidas London Green Pink están inspiradas en la colaboración Gucci x Gazelle.", "zapa2.jpg.webp", "Zapatillas Adidas London", 250000m, 30 },
                    { 3, "Modelo clásico de los 80. Rosa vibrante con esencia futbolera británica.", "zapa3.jpg.webp", "Zapatillas Puma Palermo", 185000m, 30 },
                    { 4, "Comodidad y estilo para correr con tecnología Air Max.", "zapa4.jpg.webp", "Zapatillas Nike Air Max", 200000m, 40 },
                    { 5, "Diseño retro con comodidad moderna.", "zapa5.jpg.webp", "Zapatillas DC Pink Breeze", 180000m, 35 },
                    { 6, "Estilo clásico y comodidad todo el día.", "zapa6.jpg.webp", "Zapatillas New Balance 574", 220000m, 45 },
                    { 7, "Perfectas para correr largas distancias con soporte extra.", "zapa7.jpg.webp", "Zapatillas Nike Dunk Low", 270000m, 25 },
                    { 8, "Clásicas y atemporales, con estilo urbano.", "zapa8.jpg.webp", "Zapatillas Converse Chuck Taylor", 140000m, 50 },
                    { 9, "Diseño icónico para uso casual y skateboarding.", "zapa9.jpg.webp", "Zapatillas Vans Old Skool", 160000m, 40 },
                    { 10, "Comodidad y estilo para uso diario y entrenamiento.", "zapa10.jpg.jpg", "Zapatillas Saucony Jazz", 190000m, 30 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_CompraId",
                table: "Items",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ProductoId",
                table: "Items",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsCarrito_CarritoId",
                table: "ItemsCarrito",
                column: "CarritoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsCarrito_ProductoId",
                table: "ItemsCarrito",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ItemsCarrito");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Carritos");

            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
