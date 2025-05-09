using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tp4.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Resultados",
                columns: table => new
                {
                    ResultadoExamenId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreAlumno = table.Column<string>(type: "TEXT", nullable: false),
                    CantidadCorrectas = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPreguntas = table.Column<int>(type: "INTEGER", nullable: false),
                    NotaFinal = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resultados", x => x.ResultadoExamenId);
                });

            migrationBuilder.CreateTable(
                name: "Respuestas",
                columns: table => new
                {
                    RespuestaExamenId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResultadoExamenId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreguntaId = table.Column<int>(type: "INTEGER", nullable: false),
                    RespuestaAlumno = table.Column<string>(type: "TEXT", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respuestas", x => x.RespuestaExamenId);
                    table.ForeignKey(
                        name: "FK_Respuestas_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "PreguntaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Respuestas_Resultados_ResultadoExamenId",
                        column: x => x.ResultadoExamenId,
                        principalTable: "Resultados",
                        principalColumn: "ResultadoExamenId",
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
                name: "Preguntas");

            migrationBuilder.DropTable(
                name: "Resultados");
        }
    }
}
