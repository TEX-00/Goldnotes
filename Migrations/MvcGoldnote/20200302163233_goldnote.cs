using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Goldnote.Migrations.MvcGoldnote
{
    public partial class goldnote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goldnote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GoldNoteName = table.Column<string>(nullable: false),
                    Change = table.Column<bool>(nullable: false),
                    WithDiscount = table.Column<bool>(nullable: false),
                    Destination = table.Column<bool>(nullable: false),
                    OnAccounting = table.Column<string>(nullable: true),
                    CreditCardMachine = table.Column<string>(nullable: true),
                    GoldNoteSendingPaper = table.Column<string>(nullable: true),
                    SpecialOptions = table.Column<string>(nullable: true),
                    ImageAdress = table.Column<string>(nullable: true),
                    EditDate = table.Column<DateTime>(nullable: false),
                    EditerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goldnote", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goldnote");
        }
    }
}
