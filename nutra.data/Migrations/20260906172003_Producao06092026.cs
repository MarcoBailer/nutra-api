using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace nutra.api.Migrations
{
    /// <inheritdoc />
    public partial class Producao06092026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanosAlimentares_ModelosDieta_ModeloDietaOrigemId",
                table: "PlanosAlimentares");

            migrationBuilder.DropTable(
                name: "ItensModelosDieta");

            migrationBuilder.DropTable(
                name: "RefeicoeModelosDieta");

            migrationBuilder.DropTable(
                name: "ModelosDieta");

            migrationBuilder.DropIndex(
                name: "IX_PlanosAlimentares_ModeloDietaOrigemId",
                table: "PlanosAlimentares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelosDieta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CriadoPorProfissionalId = table.Column<string>(type: "character varying(128)", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CaloriasBase = table.Column<double>(type: "double precision", nullable: false),
                    CarboidratoBaseG = table.Column<double>(type: "double precision", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    GorduraBaseG = table.Column<double>(type: "double precision", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NumeroRefeicoesDia = table.Column<int>(type: "integer", nullable: false),
                    ObjetivoAlvo = table.Column<int>(type: "integer", nullable: false),
                    PreferenciaAlimentarAlvo = table.Column<int>(type: "integer", nullable: false),
                    ProteinaBaseG = table.Column<double>(type: "double precision", nullable: false),
                    Publico = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelosDieta_ApplicationUsers_CriadoPorProfissionalId",
                        column: x => x.CriadoPorProfissionalId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RefeicoeModelosDieta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModeloDietaId = table.Column<int>(type: "integer", nullable: false),
                    HorarioSugerido = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    PercentualCaloricoSugerido = table.Column<double>(type: "double precision", nullable: false),
                    TipoRefeicao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefeicoeModelosDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefeicoeModelosDieta_ModelosDieta_ModeloDietaId",
                        column: x => x.ModeloDietaId,
                        principalTable: "ModelosDieta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensModelosDieta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefeicaoModeloDietaId = table.Column<int>(type: "integer", nullable: false),
                    AlimentoId = table.Column<int>(type: "integer", nullable: false),
                    CarboidratoG = table.Column<double>(type: "double precision", nullable: false),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: false),
                    FibraG = table.Column<double>(type: "double precision", nullable: false),
                    GorduraG = table.Column<double>(type: "double precision", nullable: false),
                    NomeAlimentoSnapshot = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    ProteinaG = table.Column<double>(type: "double precision", nullable: false),
                    QuantidadeG = table.Column<double>(type: "double precision", nullable: false),
                    TipoTabela = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensModelosDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensModelosDieta_RefeicoeModelosDieta_RefeicaoModeloDietaId",
                        column: x => x.RefeicaoModeloDietaId,
                        principalTable: "RefeicoeModelosDieta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanosAlimentares_ModeloDietaOrigemId",
                table: "PlanosAlimentares",
                column: "ModeloDietaOrigemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensModelosDieta_RefeicaoModeloDietaId",
                table: "ItensModelosDieta",
                column: "RefeicaoModeloDietaId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosDieta_CriadoPorProfissionalId",
                table: "ModelosDieta",
                column: "CriadoPorProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_RefeicoeModelosDieta_ModeloDietaId",
                table: "RefeicoeModelosDieta",
                column: "ModeloDietaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanosAlimentares_ModelosDieta_ModeloDietaOrigemId",
                table: "PlanosAlimentares",
                column: "ModeloDietaOrigemId",
                principalTable: "ModelosDieta",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
