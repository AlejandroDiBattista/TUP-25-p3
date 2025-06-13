using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Data.Migrations
{

    public partial class CorregirNombresConfirmacion : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCompra_Productos_ProductoId1",
                table: "ItemsCompra");

            migrationBuilder.DropIndex(
                name: "IX_ItemsCompra_ProductoId1",
                table: "ItemsCompra");

            migrationBuilder.DropColumn(
                name: "ProductoId1",
                table: "ItemsCompra");
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoId1",
                table: "ItemsCompra",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemsCompra_ProductoId1",
                table: "ItemsCompra",
                column: "ProductoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsCompra_Productos_ProductoId1",
                table: "ItemsCompra",
                column: "ProductoId1",
                principalTable: "Productos",
                principalColumn: "Id");
        }
    }
}
