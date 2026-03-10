using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nutra.Migrations
{
    /// <inheritdoc />
    public partial class AddStructuredFoodPortions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dose",
                table: "Genericos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PorcaoTexto",
                table: "Genericos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unidade",
                table: "Genericos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dose",
                table: "FastFoods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PorcaoTexto",
                table: "FastFoods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unidade",
                table: "FastFoods",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dose",
                table: "Fabricantes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PorcaoTexto",
                table: "Fabricantes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unidade",
                table: "Fabricantes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dose",
                table: "Genericos");

            migrationBuilder.DropColumn(
                name: "PorcaoTexto",
                table: "Genericos");

            migrationBuilder.DropColumn(
                name: "Unidade",
                table: "Genericos");

            migrationBuilder.DropColumn(
                name: "Dose",
                table: "FastFoods");

            migrationBuilder.DropColumn(
                name: "PorcaoTexto",
                table: "FastFoods");

            migrationBuilder.DropColumn(
                name: "Unidade",
                table: "FastFoods");

            migrationBuilder.DropColumn(
                name: "Dose",
                table: "Fabricantes");

            migrationBuilder.DropColumn(
                name: "PorcaoTexto",
                table: "Fabricantes");

            migrationBuilder.DropColumn(
                name: "Unidade",
                table: "Fabricantes");
        }
    }
}
