using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Approvers.King.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlySlotReward",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlySlotReward",
                table: "Users");
        }
    }
}
