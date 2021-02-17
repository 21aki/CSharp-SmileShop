using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SmileShop.Migrations
{
    public partial class SmileShopRenamePropperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Product",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup");

            migrationBuilder.DropIndex(
                name: "IX_ProductGroup_CreatedUserId",
                table: "ProductGroup");

            migrationBuilder.RenameColumn(
                name: "CreatedUserId",
                table: "ProductGroup",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Itemcount",
                table: "Order",
                newName: "ItemCount");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "ProductGroup",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductGroup",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroup_CreatedByUserId",
                table: "ProductGroup",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Product",
                table: "OrderDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup",
                column: "CreatedByUserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Product",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup");

            migrationBuilder.DropIndex(
                name: "IX_ProductGroup_CreatedByUserId",
                table: "ProductGroup");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "ProductGroup",
                newName: "CreatedUserId");

            migrationBuilder.RenameColumn(
                name: "ItemCount",
                table: "Order",
                newName: "Itemcount");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "ProductGroup",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "ProductGroup",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedUserId",
                table: "ProductGroup",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroup_CreatedUserId",
                table: "ProductGroup",
                column: "CreatedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Product",
                table: "OrderDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup",
                column: "CreatedUserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
