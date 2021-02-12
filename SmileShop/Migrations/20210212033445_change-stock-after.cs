using Microsoft.EntityFrameworkCore.Migrations;

namespace SmileShop.Migrations
{
    public partial class changestockafter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockAfter",
                table: "Stock");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockAfter",
                table: "Stock",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
