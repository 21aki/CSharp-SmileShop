using Microsoft.EntityFrameworkCore.Migrations;

namespace SmileShop.Migrations
{
    public partial class remodelrelatedtostock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_User",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Product",
                table: "Stock");

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailId",
                table: "Stock",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_User",
                table: "Stock",
                column: "CreatedByUserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_Product",
                table: "Stock",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stock_User",
                table: "Stock");

            migrationBuilder.DropForeignKey(
                name: "FK_Stock_Product",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "OrderDetailId",
                table: "Stock");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_User",
                table: "Stock",
                column: "CreatedByUserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Product",
                table: "Stock",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
