using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeAdvisor.DB.Migrations
{
    public partial class AddIdToHistoricalRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricalRates",
                table: "HistoricalRates");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "HistoricalRates",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricalRates",
                table: "HistoricalRates",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricalRates",
                table: "HistoricalRates");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "HistoricalRates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricalRates",
                table: "HistoricalRates",
                column: "Day");
        }
    }
}
