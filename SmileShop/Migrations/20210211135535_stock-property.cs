using Microsoft.EntityFrameworkCore.Migrations;

namespace SmileShop.Migrations
{
    public partial class stockproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockAfter",
                table: "Stock",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockBefore",
                table: "Stock",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockAfter",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "StockBefore",
                table: "Stock");
        }
    }
}
