using Microsoft.EntityFrameworkCore.Migrations;

namespace SmileShop.Migrations
{
    public partial class inventorytostock2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory");

            migrationBuilder.RenameTable(
                name: "Inventory",
                newName: "Stock");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_ProductId",
                table: "Stock",
                newName: "IX_Stock_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_CreatedByUserId",
                table: "Stock",
                newName: "IX_Stock_CreatedByUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stock",
                table: "Stock",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stock",
                table: "Stock");

            migrationBuilder.RenameTable(
                name: "Stock",
                newName: "Inventory");

            migrationBuilder.RenameIndex(
                name: "IX_Stock_ProductId",
                table: "Inventory",
                newName: "IX_Inventory_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Stock_CreatedByUserId",
                table: "Inventory",
                newName: "IX_Inventory_CreatedByUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory",
                column: "Id");
        }
    }
}
