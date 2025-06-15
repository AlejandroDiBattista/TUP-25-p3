using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRelacionCompraItem : Migration
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
                    Total = table.Column<double>(type: "REAL", nullable: false),
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
                    Imagen = table.Column<string>(type: "TEXT", nullable: true),
                    Precio = table.Column<double>(type: "REAL", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioUnitario = table.Column<double>(type: "REAL", nullable: false),
                    CompraId = table.Column<int>(type: "INTEGER", nullable: false)
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
                columns: new[] { "Id", "Cantidad", "Descripcion", "Imagen", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 1, 10, "Heladera con freezer", "https://assets.hotsale.com.ar/uploads/offers/465371/680fe0b634761.jpg?w=500&h=375", "Heladera", 150000.0 },
                    { 2, 8, "Lavarropas automático carga frontal", "https://authogar.vtexassets.com/arquivos/ids/198954-500-auto?v=638767894197430000&width=500&height=auto&aspect=true", "Lavarropas", 120000.0 },
                    { 3, 15, "Microondas digital 20L", "https://thumbs.dreamstime.com/b/abra-el-horno-de-microondas-84772112.jpg", "Microondas", 45000.0 },
                    { 4, 6, "Aire acondicionado split frío/calor", "https://static.vecteezy.com/system/resources/thumbnails/026/484/369/small/white-air-conditioner-and-remote-isolated-on-white-wall-background-cooling-product-for-in-summer-clipping-path-free-photo.jpg", "Aire Acondicionado", 180000.0 },
                    { 5, 12, "Horno eléctrico 45 litros", "https://http2.mlstatic.com/D_Q_NP_2X_697835-MLU74245658329_012024-E.webp", "Horno eléctrico", 60000.0 },
                    { 6, 20, "Batidora de mano 5 velocidades", "https://http2.mlstatic.com/D_Q_NP_2X_978238-MLU72674744248_112023-V.webp", "Batidora", 20000.0 },
                    { 7, 18, "Licuadora de vaso 1.5L", "https://img.freepik.com/fotos-premium/licuadora-electrica_909293-2672.jpg?semt=ais_hybrid&w=740", "Licuadora", 25000.0 },
                    { 8, 14, "Cafetera eléctrica 12 tazas", "https://http2.mlstatic.com/D_Q_NP_2X_956659-MLA83391699886_042025-V.webp", "Cafetera", 30000.0 },
                    { 9, 25, "Plancha a vapor con suela antiadherente", "https://http2.mlstatic.com/D_Q_NP_2X_889265-MLA84235372773_042025-V.webp", "Plancha", 18000.0 },
                    { 10, 22, "Tostadora 2 rebanadas con regulador", "https://http2.mlstatic.com/D_Q_NP_2X_957292-MLA52160650303_102022-V.webp", "Tostadora", 15000.0 }
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
