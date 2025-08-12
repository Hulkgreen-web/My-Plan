using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPlan.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Objectif",
                table: "Epargnes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Objectif",
                table: "Epargnes");
        }
    }
}
