using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace servidor.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Gadnic PITOOOOOO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Gadnic T605");
        }
    }
}
