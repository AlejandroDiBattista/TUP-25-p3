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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    NombreCliente = table.Column<string>(type: "TEXT", nullable: false),
                    ApellidoCliente = table.Column<string>(type: "TEXT", nullable: false),
                    EmailCliente = table.Column<string>(type: "TEXT", nullable: false)
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
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagenUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarritoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    CarritoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarritoItems_Carritos_CarritoId",
                        column: x => x.CarritoId,
                        principalTable: "Carritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarritoItems_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    { 1, "El último smartphone de Apple con chip A16 Bionic.", "https://laplatacells.com.ar/img/Public/1169/62057-producto-iphone-14-pro-space-black-pdp-image-position-1a-mxla.jpg", "iPhone 14 Pro", 999.99m, 50 },
                    { 2, "El buque insignia de Samsung con un S Pen integrado.", "https://images.samsung.com/is/image/samsung/p6pim/in/2302/gallery/in-galaxy-s23-s918-446812-sm-s918bzrcins-534868449?$684_547_PNG$", "Samsung Galaxy S23 Ultra", 1199.99m, 40 },
                    { 3, "La magia de Google en un teléfono, con el chip Tensor G2.", "https://www.alemaniacell.com/uploads/imagen-principal23306-1-1690494033.JPG", "Google Pixel 7 Pro", 899.00m, 60 },
                    { 4, "Protección suave y elegante para tu iPhone.", "https://acdn-us.mitiendanube.com/stores/001/643/020/products/silicone-case-con-logo-iphone-16-pro-azul-oscuro-18c6317df9dedf6c3817260731913348-640-0.png", "Funda de Silicona para iPhone", 49.00m, 150 },
                    { 5, "Carga tu dispositivo a toda velocidad.", "https://http2.mlstatic.com/D_NQ_NP_903078-MLU77836658064_072024-O.webp", "Cargador Rápido USB-C 30W", 35.50m, 200 },
                    { 6, "Cancelación de ruido activa y audio espacial.", "https://ipoint.com.ar/25134-thickbox_default/apple-airpods-pro-2da-generacion.jpg", "AirPods Pro (2da Gen)", 249.00m, 80 },
                    { 7, "Máxima protección contra rayones y golpes.", "https://acdn-us.mitiendanube.com/stores/078/254/products/full-glue-03-8c85bbc0b5da12d85016298455797656-640-0-972d15eba548f5d7f317107817449057-640-0.png", "Protector de Pantalla de Vidrio", 25.00m, 300 },
                    { 8, "Monitor de salud avanzado y diseño moderno.", "https://cdn.kemik.gt/2023/06/R-920-BLACK-SAMSUNG-1200X1200-1-1-768x768.-700x700.jpg", "Samsung Galaxy Watch 5", 279.99m, 70 },
                    { 9, "Nunca te quedes sin batería fuera de casa.", "https://static.bidcom.com.ar/publicacionesML/productos/KCABLE03/1000x1000-KCABLE03.jpg", "Batería Externa 10000mAh", 45.00m, 120 },
                    { 10, "Mantén tu teléfono seguro y a la vista mientras conduces.", "https://dcdn-us.mitiendanube.com/stores/002/611/582/products/soporte-auto-8ffc2a832392b199f917190750449559-1024-1024.webp", "Soporte de Coche Magnético", 22.99m, 180 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_CarritoId",
                table: "CarritoItems",
                column: "CarritoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_ProductoId",
                table: "CarritoItems",
                column: "ProductoId");

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
                name: "CarritoItems");

            migrationBuilder.DropTable(
                name: "ItemsCompra");

            migrationBuilder.DropTable(
                name: "Carritos");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
