using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Data.Migrations
{

    public partial class NombreDeLaMigracion : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsCompra",
                table: "ItemsCompra");

            migrationBuilder.DropIndex(
                name: "IX_ItemsCompra_CompraId",
                table: "ItemsCompra");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Compras",
                newName: "FechaCreacion");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompraId",
                table: "ItemsCompra",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ItemsCompra",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoId1",
                table: "ItemsCompra",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "NombreCliente",
                table: "Compras",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "EmailCliente",
                table: "Compras",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ApellidoCliente",
                table: "Compras",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Compras",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompra",
                table: "Compras",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsCompra",
                table: "ItemsCompra",
                columns: new[] { "CompraId", "ProductoId" });

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


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsCompra_Productos_ProductoId1",
                table: "ItemsCompra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemsCompra",
                table: "ItemsCompra");

            migrationBuilder.DropIndex(
                name: "IX_ItemsCompra_ProductoId1",
                table: "ItemsCompra");

            migrationBuilder.DropColumn(
                name: "ProductoId1",
                table: "ItemsCompra");

            migrationBuilder.DropColumn(
                name: "FechaCompra",
                table: "Compras");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Compras",
                newName: "Fecha");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ItemsCompra",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "CompraId",
                table: "ItemsCompra",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Compras",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreCliente",
                table: "Compras",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailCliente",
                table: "Compras",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApellidoCliente",
                table: "Compras",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Compras",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemsCompra",
                table: "ItemsCompra",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsCompra_CompraId",
                table: "ItemsCompra",
                column: "CompraId");
        }
    }
}
