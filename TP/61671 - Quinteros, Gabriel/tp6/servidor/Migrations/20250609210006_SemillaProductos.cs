using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class SemillaProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCarrito_Carritos_CarritoId",
                table: "ItemsCarrito");

            migrationBuilder.AlterColumn<Guid>(
                name: "CarritoId",
                table: "ItemsCarrito",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Descripcion", "ImagUrl", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, "Sedán compacto, motor 1.8L", "https://acroadtrip.blob.core.windows.net/catalogo-imagenes/xl/RT_V_9b7e7e2b5e7e4f8c8b7e7e2b5e7e4f8c.jpg", "Toyota Corolla", 12000000m, 5 },
                    { 2, "Hatchback, motor 1.6L", "https://acroadtrip.blob.core.windows.net/catalogo-imagenes/xl/RT_V_8b7e7e2b5e7e4f8c8b7e7e2b5e7e4f8c.jpg", "Ford Fiesta", 9500000m, 7 },
                    { 3, "Hatchback, motor 1.4L", "https://acroadtrip.blob.core.windows.net/catalogo-imagenes/xl/RT_V_7b7e7e2b5e7e4f8c8b7e7e2b5e7e4f8c.jpg", "Volkswagen Golf", 11000000m, 4 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCarrito_Carritos_CarritoId",
                table: "ItemsCarrito",
                column: "CarritoId",
                principalTable: "Carritos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCarrito_Carritos_CarritoId",
                table: "ItemsCarrito");

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<Guid>(
                name: "CarritoId",
                table: "ItemsCarrito",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCarrito_Carritos_CarritoId",
                table: "ItemsCarrito",
                column: "CarritoId",
                principalTable: "Carritos",
                principalColumn: "Id");
        }
    }
}
