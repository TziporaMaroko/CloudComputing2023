using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class CartItemsFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartItems_Flavour_FlavourId",
                table: "ShoppingCartItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCartItems_FlavourId",
                table: "ShoppingCartItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ShoppingCartItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ShoppingCartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_FlavourId",
                table: "ShoppingCartItems",
                column: "FlavourId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartItems_Flavour_FlavourId",
                table: "ShoppingCartItems",
                column: "FlavourId",
                principalTable: "Flavour",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
