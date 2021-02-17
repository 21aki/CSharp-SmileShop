using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SmileShop.Migrations
{
    public partial class inventoryaddusercreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserID",
                table: "Order",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreatedByUserID",
                table: "Order",
                newName: "IX_Order_CreatedByUserId");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "OrderDetail",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Inventory",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Inventory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_CreatedByUserId",
                table: "Inventory",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_User",
                table: "Inventory",
                column: "CreatedByUserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_User",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_CreatedByUserId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Inventory");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Order",
                newName: "CreatedByUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreatedByUserId",
                table: "Order",
                newName: "IX_Order_CreatedByUserID");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
