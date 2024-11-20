using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Approvers.King.Migrations
{
    /// <inheritdoc />
    public partial class AddGachaProbability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GachaProbabilities",
                columns: table => new
                {
                    RandomMessageId = table.Column<string>(type: "TEXT", nullable: false),
                    Probability = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GachaProbabilities", x => x.RandomMessageId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GachaProbabilities");
        }
    }
}
