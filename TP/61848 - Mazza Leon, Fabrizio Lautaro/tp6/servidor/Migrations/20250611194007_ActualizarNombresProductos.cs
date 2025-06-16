using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarNombresProductos : Migration
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
                name: "CarritoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    CarritoId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "TEXT", nullable: false)
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

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Celular Ultima Generacion", "https://i5.walmartimages.com/seo/Restored-Apple-iPhone-15-Pro-Max-256GB-Unlocked-Blue-Titanium-MU693LL-A-Excellent-Condition_dd2d42c6-cc25-4bee-81ef-7847120498d5.663475b807d168a41e9082d258d9c7ce.jpeg", "Iphone 15 Pro Max", 2000000m });

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_CarritoId",
                table: "CarritoItems",
                column: "CarritoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_ProductoId",
                table: "CarritoItems",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarritoItems");

            migrationBuilder.DropTable(
                name: "Carritos");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[] { "Celular potente", "https://via.placeholder.com/150", "Celular X", 120000m });
        }
    }
}
