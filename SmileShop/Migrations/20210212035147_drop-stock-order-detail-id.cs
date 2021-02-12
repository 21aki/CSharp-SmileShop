using Microsoft.EntityFrameworkCore.Migrations;

namespace SmileShop.Migrations
{
    public partial class dropstockorderdetailid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderDetailId",
                table: "Stock");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderDetailId",
                table: "Stock",
                type: "int",
                nullable: true);
        }
    }
}
