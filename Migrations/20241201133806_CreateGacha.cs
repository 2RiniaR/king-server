using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Approvers.King.Migrations
{
    /// <inheritdoc />
    public partial class CreateGacha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppStates");

            migrationBuilder.DropTable(
                name: "GachaProbabilities");

            migrationBuilder.CreateTable(
                name: "Gachas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HitProbabilityPermillage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gachas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GachaItems",
                columns: table => new
                {
                    GachaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RandomMessageId = table.Column<string>(type: "TEXT", nullable: false),
                    ProbabilityPermillage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GachaItems", x => new { x.GachaId, x.RandomMessageId });
                    table.ForeignKey(
                        name: "FK_GachaItems_Gachas_GachaId",
                        column: x => x.GachaId,
                        principalTable: "Gachas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GachaItems");

            migrationBuilder.DropTable(
                name: "Gachas");

            migrationBuilder.CreateTable(
                name: "AppStates",
                columns: table => new
                {
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStates", x => x.Type);
                });

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
    }
}
