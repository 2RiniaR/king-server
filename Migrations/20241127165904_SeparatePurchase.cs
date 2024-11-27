using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Approvers.King.Migrations
{
    /// <inheritdoc />
    public partial class SeparatePurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscordID",
                table: "Users",
                newName: "DiscordId");

            migrationBuilder.RenameColumn(
                name: "MonthlySlotReward",
                table: "Users",
                newName: "MonthlySlotProfitPrice");

            migrationBuilder.RenameColumn(
                name: "MonthlyPurchase",
                table: "Users",
                newName: "MonthlyGachaPurchasePrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscordId",
                table: "Users",
                newName: "DiscordID");

            migrationBuilder.RenameColumn(
                name: "MonthlySlotProfitPrice",
                table: "Users",
                newName: "MonthlySlotReward");

            migrationBuilder.RenameColumn(
                name: "MonthlyGachaPurchasePrice",
                table: "Users",
                newName: "MonthlyPurchase");
        }
    }
}
