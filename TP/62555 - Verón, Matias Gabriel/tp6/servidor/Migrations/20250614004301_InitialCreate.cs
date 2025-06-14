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
                name: "Carritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
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
                name: "DetallesCompras",
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
                    table.PrimaryKey("PK_DetallesCompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesCompras_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesCompras_Productos_ProductoId",
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
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CarritoId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    { 1, "Modelo Retro 2012/2013", "/imagenes/3a4bed5882aa0026981e07e2ee9c6ed2.jpg", "Camiseta de fútbol Inter", 8000.00m, 10 },
                    { 2, "Modelo Retro 1999/2000", "/imagenes/528af923e61317453e9a761674c5e040.jpg", "Camiseta de fútbol Real Madrid", 9500.00m, 5 },
                    { 3, "Modelo Retro 1998/1999", "/imagenes/03ddd2e5d92dbb21f0501f704bca1de9.jpg", "Camiseta de fútbol Boca Juniors", 9000.00m, 7 },
                    { 4, "Modelo Retro 2013/2014", "/imagenes/9c8af3ba7529610d89b06096247552fc.jpg", "Camiseta de fútbol Borussia Dortmund", 4000.00m, 8 },
                    { 5, "Modelo Retro 2012", "/imagenes/542a001d4f6b03edb143f320b220b5d7.jpg", "Camiseta de fútbol Atletico Tucumán", 7000.00m, 15 },
                    { 6, "Modelo Retro ", "/imagenes/7579afdf0e7e8cc92cd8bfd79c610381.jpg", "Camiseta de fútbol Selección Italia", 8250.00m, 3 },
                    { 7, "Modelo Retro 1998/2000", "/imagenes/a9184fa085b21f0ba123a9cd7103148d.jpg", "Camiseta de fútbol Real Madrid", 7350.00m, 6 },
                    { 8, "Modelo Retro 1996", "/imagenes/a452159f6193fe98782e42dd9687ffa4.jpg", "Camiseta de fútbol Riber Plate", 9450.00m, 4 },
                    { 9, "Modelo Retro 2007", "/imagenes/cd11d87f448389e60c7bb108796cfab7.jpg", "Camiseta de fútbol Boca Juniors", 5550.00m, 1 },
                    { 10, "Modelo Retro 2012/2013", "/imagenes/ce4b1803a674d7e715cc90b610366632.jpg", "Camiseta de fútbol Milan", 5600.00m, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompras_CompraId",
                table: "DetallesCompras",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompras_ProductoId",
                table: "DetallesCompras",
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
                name: "DetallesCompras");

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
