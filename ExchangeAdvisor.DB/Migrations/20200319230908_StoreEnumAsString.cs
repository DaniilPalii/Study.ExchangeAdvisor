using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeAdvisor.DB.Migrations
{
    public partial class StoreEnumAsString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ClearHistoricalRatesTable(migrationBuilder);

            migrationBuilder.AlterColumn<string>(
                name: "ComparingCurrency",
                table: "HistoricalRates",
                type: "nvarchar(4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "BaseCurrency",
                table: "HistoricalRates",
                type: "nvarchar(4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ClearHistoricalRatesTable(migrationBuilder);

            migrationBuilder.AlterColumn<int>(
                name: "ComparingCurrency",
                table: "HistoricalRates",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)");

            migrationBuilder.AlterColumn<int>(
                name: "BaseCurrency",
                table: "HistoricalRates",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)");
        }

        private static void ClearHistoricalRatesTable(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM [HistoricalRates]",
                suppressTransaction: true);
        }
    }
}
