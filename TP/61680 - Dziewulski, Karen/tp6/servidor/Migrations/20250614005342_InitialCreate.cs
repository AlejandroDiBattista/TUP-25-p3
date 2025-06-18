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
                    { 1, "Base crocante con crema pastelera y frutillas frescas", "https://www.recetasnestlecam.com/sites/default/files/styles/recipe_detail_mobile/public/srh_recipes/07892f02f7c57b83d5703b4ee924221e.jpg?itok=Vrlk1qve", "Tarta de Frutilla", 45000.00m, 10 },
                    { 2, "Tarta de queso con base de galleta y coulis de frutos rojos", "https://laopinionaustral.com.ar/media/uploads/2024/05/receta-lemon-pie.jpg", "Cheesecake", 48000.00m, 8 },
                    { 3, "Brownie húmedo con nueces y baño de chocolate", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSWk6QSUvVAwMUCKo4rWpU5yAuBsmfw0JwftnblS3skQIm_7cG85vIKik-wOE4OlNaS4K0&usqp=CAU", "Brownie", 32000.00m, 15 },
                    { 4, "Tarta de limón con merengue italiano", "https://laopinionaustral.com.ar/media/uploads/2024/05/receta-lemon-pie.jpg", "Lemon Pie", 44000.00m, 7 },
                    { 5, "Bizcochuelo de chocolate, crema y cerezas", "https://tofuu.getjusto.com/orioneat-local/resized2/zqaT5XLQ7RAqLXsKd-2400-x.webp", "Torta Selva Negra", 60000.00m, 5 },
                    { 6, "Torta con crema de oreo y base de galletas", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS8QoDdXTxTbBYqfQlOBARSbi2NMifsivPdUA&s", "Torta Oreo", 55000.00m, 6 },
                    { 7, "Cupcake con buttercream de colores", "https://storage.googleapis.com/fitia_public_images/recipes%2FGR-R-V-00001923_kh2jkptyj90wdbzs6jrjm2dn_large.jpeg", "Cupcake de Vainilla", 9000.00m, 25 },
                    { 8, "Caja con 6 macarons surtidos", "https://assets.tmecosys.com/image/upload/t_web_rdp_recipe_584x480/img/recipe/ras/Assets/B9FF58C7-D19C-4699-ADA6-E49760836EBB/Derivates/e1b80516-57ce-471c-bdee-eed02a742326.jpg", "Macarons", 27000.00m, 10 },
                    { 9, "Relleno de dulce de leche y coco rallado", "https://via.placeholder.com/150?text=Alfajor", "Alfajor de Maicena", 7000.00m, 30 },
                    { 10, "Bizcochuelo de zanahoria con frosting de queso", "https://via.placeholder.com/150?text=Zanahoria", "Torta de Zanahoria", 50000.00m, 14 }
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
