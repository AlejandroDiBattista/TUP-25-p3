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
                    { 1, "El último smartphone de Apple con chip A16 Bionic.", "https://placehold.co/300x200/png", "iPhone 14 Pro", 999.99m, 50 },
                    { 2, "El buque insignia de Samsung con un S Pen integrado.", "https://placehold.co/300x200/png", "Samsung Galaxy S23 Ultra", 1199.99m, 40 },
                    { 3, "La magia de Google en un teléfono, con el chip Tensor G2.", "https://placehold.co/300x200/png", "Google Pixel 7 Pro", 899.00m, 60 },
                    { 4, "Protección suave y elegante para tu iPhone.", "https://placehold.co/300x200/png", "Funda de Silicona para iPhone", 49.00m, 150 },
                    { 5, "Carga tu dispositivo a toda velocidad.", "https://placehold.co/300x200/png", "Cargador Rápido USB-C 30W", 35.50m, 200 },
                    { 6, "Cancelación de ruido activa y audio espacial.", "https://placehold.co/300x200/png", "AirPods Pro (2da Gen)", 249.00m, 80 },
                    { 7, "Máxima protección contra rayones y golpes.", "https://placehold.co/300x200/png", "Protector de Pantalla de Vidrio", 25.00m, 300 },
                    { 8, "Monitor de salud avanzado y diseño moderno.", "https://placehold.co/300x200/png", "Samsung Galaxy Watch 5", 279.99m, 70 },
                    { 9, "Nunca te quedes sin batería fuera de casa.", "https://placehold.co/300x200/png", "Batería Externa 10000mAh", 45.00m, 120 },
                    { 10, "Mantén tu teléfono seguro y a la vista mientras conduces.", "https://placehold.co/300x200/png", "Soporte de Coche Magnético", 22.99m, 180 }
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
