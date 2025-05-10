using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JuanApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DbBasketItems_ProductId",
                table: "DbBasketItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DbBasketItems_Products_ProductId",
                table: "DbBasketItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DbBasketItems_Products_ProductId",
                table: "DbBasketItems");

            migrationBuilder.DropIndex(
                name: "IX_DbBasketItems_ProductId",
                table: "DbBasketItems");
        }
    }
}
