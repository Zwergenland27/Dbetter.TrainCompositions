using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBetter.TrainCompositions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CoachLayouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoachLayouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    ConstructionType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachLayouts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachLayouts_Identifier",
                table: "CoachLayouts",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoachLayouts");
        }
    }
}
