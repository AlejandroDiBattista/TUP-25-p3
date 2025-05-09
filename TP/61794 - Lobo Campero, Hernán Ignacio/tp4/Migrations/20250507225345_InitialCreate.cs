using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tp4.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                name: "ResultadosExamen",
                columns: table => new
                {
                    ResultadoExamenId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Alumno = table.Column<string>(type: "TEXT", nullable: false),
                    RespuestasCorrectas = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPreguntas = table.Column<int>(type: "INTEGER", nullable: false),
                    NotaFinal = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosExamen", x => x.ResultadoExamenId);
                });

            migrationBuilder.CreateTable(
                name: "RespuestasExamen",
                columns: table => new
                {
                    RespuestaExamenId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResultadoExamenId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreguntaId = table.Column<int>(type: "INTEGER", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasExamen", x => x.RespuestaExamenId);
                    table.ForeignKey(
                        name: "FK_RespuestasExamen_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "PreguntaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasExamen_ResultadosExamen_ResultadoExamenId",
                        column: x => x.ResultadoExamenId,
                        principalTable: "ResultadosExamen",
                        principalColumn: "ResultadoExamenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasExamen_PreguntaId",
                table: "RespuestasExamen",
                column: "PreguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasExamen_ResultadoExamenId",
                table: "RespuestasExamen",
                column: "ResultadoExamenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespuestasExamen");

            migrationBuilder.DropTable(
                name: "Preguntas");

            migrationBuilder.DropTable(
                name: "ResultadosExamen");
        }
    }
}
