using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tp4.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Examenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Alumno = table.Column<string>(type: "TEXT", nullable: false),
                    Correctas = table.Column<int>(type: "INTEGER", nullable: false),
                    Total = table.Column<int>(type: "INTEGER", nullable: false),
                    Nota = table.Column<double>(type: "REAL", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examenes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    PreguntaId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Enunciado = table.Column<string>(type: "TEXT", nullable: false),
                    RespuestaA = table.Column<string>(type: "TEXT", nullable: false),
                    RespuestaB = table.Column<string>(type: "TEXT", nullable: false),
                    RespuestaC = table.Column<string>(type: "TEXT", nullable: false),
                    Correcta = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.PreguntaId);
                });

            migrationBuilder.CreateTable(
                name: "Respuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResultadoExamenId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreguntaId = table.Column<int>(type: "INTEGER", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "INTEGER", nullable: false),
                    RespuestaSeleccionada = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Respuestas_Examenes_ResultadoExamenId",
                        column: x => x.ResultadoExamenId,
                        principalTable: "Examenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Respuestas_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "PreguntaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_PreguntaId",
                table: "Respuestas",
                column: "PreguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_ResultadoExamenId",
                table: "Respuestas",
                column: "ResultadoExamenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Respuestas");

            migrationBuilder.DropTable(
                name: "Examenes");

            migrationBuilder.DropTable(
                name: "Preguntas");
        }
    }
}
