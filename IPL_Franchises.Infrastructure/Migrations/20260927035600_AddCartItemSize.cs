using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPL_Franchises.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCartItemSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "SelectedSize",
                table: "CartItems",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductId_SelectedSize",
                table: "CartItems",
                columns: new[] { "CartId", "ProductId", "SelectedSize" },
                unique: true,
                filter: "[SelectedSize] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_ProductId_SelectedSize",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "SelectedSize",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "CartId", "ProductId" },
                unique: true);
        }
    }
}
