using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SmileShop.Migrations
{
    public partial class UpdateProductGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "ProductGroup",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup",
                column: "CreatedByUserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "ProductGroup",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGroup_User",
                table: "ProductGroup",
                column: "CreatedByUserId",
                principalSchema: "auth",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
