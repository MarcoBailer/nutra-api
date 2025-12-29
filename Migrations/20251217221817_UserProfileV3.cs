using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nutra.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistroAlimentar",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AlimentoIdOrigem = table.Column<int>(type: "int", nullable: false),
                    NomeAlimentoSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoTabela = table.Column<int>(type: "int", nullable: false),
                    QuantidadeConsumidaG = table.Column<double>(type: "float", nullable: false),
                    DataConsumo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Refeicao = table.Column<int>(type: "int", nullable: false),
                    EnergiaKcalTotal = table.Column<double>(type: "float", nullable: false),
                    ProteinaTotal = table.Column<double>(type: "float", nullable: false),
                    CarboTotal = table.Column<double>(type: "float", nullable: false),
                    GorduraTotal = table.Column<double>(type: "float", nullable: false),
                    FibraTotal = table.Column<double>(type: "float", nullable: false),
                    AguaTotal = table.Column<double>(type: "float", nullable: false),
                    DadosNutricionaisCompletosJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAlimentar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroAlimentar_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAlimentar_UserId",
                table: "RegistroAlimentar",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistroAlimentar");
        }
    }
}
