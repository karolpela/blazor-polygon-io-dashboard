using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Server.Data.Migrations
{
    public partial class MapResultsToListItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "TickerResults",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "TickerListItem",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "StockChartData",
                type: "nvarchar(8)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)");

            migrationBuilder.AlterColumn<string>(
                name: "TickerResultsTicker",
                table: "NewsResultTickerResults",
                type: "nvarchar(8)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "Logo",
                type: "nvarchar(8)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "DailyOpenClose",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<string>(
                name: "TickerWatchlistTicker",
                table: "ApplicationUserTickerResults",
                type: "nvarchar(8)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)");

            migrationBuilder.AddForeignKey(
                name: "FK_TickerResults_TickerListItem_Ticker",
                table: "TickerResults",
                column: "Ticker",
                principalTable: "TickerListItem",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TickerResults_TickerListItem_Ticker",
                table: "TickerResults");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "TickerResults",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "TickerListItem",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "StockChartData",
                type: "nvarchar(4)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)");

            migrationBuilder.AlterColumn<string>(
                name: "TickerResultsTicker",
                table: "NewsResultTickerResults",
                type: "nvarchar(4)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "Logo",
                type: "nvarchar(4)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "DailyOpenClose",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "TickerWatchlistTicker",
                table: "ApplicationUserTickerResults",
                type: "nvarchar(4)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)");
        }
    }
}
