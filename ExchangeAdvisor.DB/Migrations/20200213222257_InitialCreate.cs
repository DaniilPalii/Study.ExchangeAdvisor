using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeAdvisor.DB.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricalRates",
                columns: table => new
                {
                    Day = table.Column<DateTime>(nullable: false),
                    Value = table.Column<float>(nullable: false),
                    BaseCurrency = table.Column<int>(nullable: false),
                    ComparingCurrency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalRates", x => x.Day);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricalRates");
        }
    }
}
