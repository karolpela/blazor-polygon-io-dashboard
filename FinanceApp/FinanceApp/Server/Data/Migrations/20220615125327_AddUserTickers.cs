using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Server.Data.Migrations
{
    public partial class AddUserTickers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Address1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Address1);
                });

            migrationBuilder.CreateTable(
                name: "Branding",
                columns: table => new
                {
                    LogoUrl = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branding", x => x.LogoUrl);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Ticker = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Locale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrimaryExchange = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CurrencyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompositeFigi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShareClassFigi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketCap = table.Column<double>(type: "float", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SicCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SicDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TickerRoot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomepageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalEmployees = table.Column<int>(type: "int", nullable: false),
                    ListDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandingLogoUrl = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShareClassSharesOutstanding = table.Column<long>(type: "bigint", nullable: false),
                    WeightedSharesOutstanding = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Ticker);
                    table.ForeignKey(
                        name: "FK_Results_Address_Address1",
                        column: x => x.Address1,
                        principalTable: "Address",
                        principalColumn: "Address1",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Branding_BrandingLogoUrl",
                        column: x => x.BrandingLogoUrl,
                        principalTable: "Branding",
                        principalColumn: "LogoUrl",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TickerDetails",
                columns: table => new
                {
                    TickerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResultsTicker = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TickerDetails", x => x.TickerId);
                    table.ForeignKey(
                        name: "FK_TickerDetails_Results_ResultsTicker",
                        column: x => x.ResultsTicker,
                        principalTable: "Results",
                        principalColumn: "Ticker",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TickerDetailsApplicationUser",
                columns: table => new
                {
                    ApplicationUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TickerWatchlistTickerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TickerDetailsApplicationUser", x => new { x.ApplicationUsersId, x.TickerWatchlistTickerId });
                    table.ForeignKey(
                        name: "FK_TickerDetailsApplicationUser_AspNetUsers_ApplicationUsersId",
                        column: x => x.ApplicationUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TickerDetailsApplicationUser_TickerDetails_TickerWatchlistTickerId",
                        column: x => x.TickerWatchlistTickerId,
                        principalTable: "TickerDetails",
                        principalColumn: "TickerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_Address1",
                table: "Results",
                column: "Address1");

            migrationBuilder.CreateIndex(
                name: "IX_Results_BrandingLogoUrl",
                table: "Results",
                column: "BrandingLogoUrl");

            migrationBuilder.CreateIndex(
                name: "IX_TickerDetails_ResultsTicker",
                table: "TickerDetails",
                column: "ResultsTicker");

            migrationBuilder.CreateIndex(
                name: "IX_TickerDetailsApplicationUser_TickerWatchlistTickerId",
                table: "TickerDetailsApplicationUser",
                column: "TickerWatchlistTickerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TickerDetailsApplicationUser");

            migrationBuilder.DropTable(
                name: "TickerDetails");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Branding");
        }
    }
}
