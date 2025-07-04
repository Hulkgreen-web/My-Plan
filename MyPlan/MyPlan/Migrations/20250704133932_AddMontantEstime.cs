using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPlan.Migrations
{
    /// <inheritdoc />
    public partial class AddMontantEstime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "montantEstime",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "montantEstime",
                table: "Categories");
        }
    }
}
