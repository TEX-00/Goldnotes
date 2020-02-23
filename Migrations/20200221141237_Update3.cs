using Microsoft.EntityFrameworkCore.Migrations;

namespace Goldnote.Migrations
{
    public partial class Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WithDiscount",
                table: "Goldnote",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WithDiscount",
                table: "Goldnote");
        }
    }
}
