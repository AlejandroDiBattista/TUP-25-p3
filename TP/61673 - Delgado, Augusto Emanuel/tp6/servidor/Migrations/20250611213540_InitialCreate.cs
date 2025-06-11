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
                    { 1, "Televisor de 50 pulgadas con resolución 4K y HDR.", "/images/tv.jpg", "Smart TV 4K", 450000m, 10 },
                    { 2, "Teléfono de última generación con cámara de 108MP y pantalla AMOLED.", "/images/smartphone.jpg", "Smartphone X10", 300000m, 25 },
                    { 3, "Auriculares inalámbricos con cancelación de ruido activa.", "/images/auriculares.jpg", "Auriculares Bluetooth", 50000m, 50 },
                    { 4, "Teclado gaming con switches Cherry MX y retroiluminación RGB personalizable.", "/images/teclado.jpg", "Teclado Mecánico RGB", 75000m, 15 },
                    { 5, "Mouse con sensor óptico de alta precisión y batería de larga duración.", "/images/mouse.jpg", "Mouse Gamer Inalámbrico", 30000m, 20 },
                    { 6, "Monitor QHD con tasa de refresco de 144Hz y panel VA curvo.", "/images/monitor.jpg", "Monitor Curvo 27''", 200000m, 8 },
                    { 7, "Cámara web con micrófono integrado, ideal para videollamadas y streaming.", "/images/webcam.jpg", "Webcam Full HD", 25000m, 30 },
                    { 8, "Unidad de estado sólido NVMe PCIe Gen4 para almacenamiento ultrarrápido.", "/images/ssd.jpg", "Disco SSD 1TB", 90000m, 12 },
                    { 9, "Router de doble banda con tecnología Wi-Fi 6 para conexiones ultrarrápidas y estables.", "/images/router.jpg", "Router Wi-Fi 6", 60000m, 18 },
                    { 10, "Impresora, escáner y copiadora con conectividad Wi-Fi y impresión a doble cara.", "/images/impresora.jpg", "Impresora Multifunción", 110000m, 7 }
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
