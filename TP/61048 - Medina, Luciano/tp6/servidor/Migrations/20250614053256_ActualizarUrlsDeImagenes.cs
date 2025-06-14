using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarUrlsDeImagenes : Migration
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
                    { 1, "Clásico e icónico, floral-aldehídico.", "https://i5.walmartimages.com/seo/Chanel-No-5-Eau-de-Parfum-Spray-Perfume-for-Women-3-4-oz-100-ml_a41d96f8-fe32-487d-9f77-ce30d05d8b72.f0424d696214b0da62c279964a8174fd.jpeg", "Chanel N°5", 150.00m, 25 },
                    { 2, "Fresco y amaderado, para hombres.", "https://www.myperfumeshop.qa/cdn/shop/files/dior-sauvage-edt-perfume-cologne-408783.png?v=1742526282&width=400", "Dior Sauvage", 120.00m, 30 },
                    { 3, "Oscuro, opulento y especiado.", "https://static.sweetcare.com/img/prd/488/v-638235637318829790/tom-ford-000011tf_03.webp", "Tom Ford Black Orchid", 200.00m, 15 },
                    { 4, "Fresco y mineral, unisex.", "https://api-assets.wikiparfum.com/_resized/nyf437ux1psfya0ekv1imq67nlx0gq8qxbdkiw2rcrkc47cug5ly8vi1ogyk-w250-q85.webp", "Jo Malone Wood Sage & Sea Salt", 90.00m, 40 },
                    { 5, "Floral blanco, empolvado.", "https://mcgrocer.com/cdn/shop/files/gucci-bloom-for-her-eau-de-toilette-50ml-40505979896046.jpg?v=1741307863", "Gucci Bloom", 110.00m, 20 },
                    { 6, "Amaderado especiado, para hombres.", "https://www.farmaciasrp.com.ar/22845-large_default/paco-rabanne-1-million-elixir-parfum-intense-50-ml.jpg", "Paco Rabanne 1 Million", 95.00m, 35 },
                    { 7, "Gourmand floral, dulce.", "https://www.farmacialeloir.com.ar/img/articulos/2024/08/imagen1_lancome_la_vie_est_belle_eau_de_parfum_x_75ml_imagen1.webp", "Lancôme La Vie Est Belle", 105.00m, 28 },
                    { 8, "Aromático fougère, para hombres.", "https://i5.walmartimages.com/seo/Versace-Eros-Eau-De-Toilette-Natural-Spray-Cologne-for-Men-6-7-oz_db99fcd0-1642-47d8-9fe4-901b3de6fbb8_1.cdfc3acf51b7b1159936f22e63daf3fe.jpeg", "Versace Eros", 85.00m, 45 },
                    { 9, "Oriental especiado, café, vainilla.", "https://static.sweetcare.com/img/prd/488/v-638200527221023353/yves-saint-laurent-017473ys_03.webp", "YSL Black Opium", 115.00m, 22 },
                    { 10, "Floral oriental, dulce.", "https://d3cdlnm7te7ky2.cloudfront.net/media/catalog/product/cache/e8f012862bd8df4f2e4f3ce158c4a16c/d/-/d-good-girl-edp_1.jpg", "Carolina Herrera Good Girl", 130.00m, 18 }
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
