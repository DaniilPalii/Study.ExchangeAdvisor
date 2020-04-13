using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeAdvisor.DB.Migrations
{
    public partial class _001_InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RateForecast",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrency = table.Column<string>(type: "nvarchar(4)", nullable: false),
                    ComparingCurrency = table.Column<string>(type: "nvarchar(4)", nullable: false),
                    CreationDay = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateForecast", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RateHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrency = table.Column<string>(type: "nvarchar(4)", nullable: false),
                    ComparingCurrency = table.Column<string>(type: "nvarchar(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForecastedRate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<DateTime>(nullable: false),
                    Value = table.Column<float>(nullable: false),
                    ForecastId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForecastedRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForecastedRate_RateForecast_ForecastId",
                        column: x => x.ForecastId,
                        principalTable: "RateForecast",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalRate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<DateTime>(nullable: false),
                    Value = table.Column<float>(nullable: false),
                    HistoryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricalRate_RateHistory_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "RateHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForecastedRate_ForecastId",
                table: "ForecastedRate",
                column: "ForecastId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalRate_HistoryId",
                table: "HistoricalRate",
                column: "HistoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForecastedRate");

            migrationBuilder.DropTable(
                name: "HistoricalRate");

            migrationBuilder.DropTable(
                name: "RateForecast");

            migrationBuilder.DropTable(
                name: "RateHistory");
        }
    }
}
