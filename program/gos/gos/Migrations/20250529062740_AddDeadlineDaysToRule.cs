using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gos.Migrations
{
    public partial class AddDeadlineDaysToRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "deadline_days",
                table: "rules",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deadline_days",
                table: "rules");
        }
    }
}
