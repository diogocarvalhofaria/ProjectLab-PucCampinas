using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectLab.PucCampinas.Migrations
{
    /// <inheritdoc />
    public partial class AddViaCepAndAddressToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "Users");
        }
    }
}
