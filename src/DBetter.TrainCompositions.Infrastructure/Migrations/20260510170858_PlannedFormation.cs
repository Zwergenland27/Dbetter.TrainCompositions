using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DBetter.TrainCompositions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PlannedFormation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlannedFormations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoachSequenceHash = table.Column<string>(type: "text", nullable: false),
                    ReverseCoachSequenceHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedFormations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlannedCoaches",
                columns: table => new
                {
                    PlannedFormationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Position = table.Column<short>(type: "smallint", nullable: false),
                    LayoutId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedCoaches", x => new { x.PlannedFormationId, x.Id });
                    table.ForeignKey(
                        name: "FK_PlannedCoaches_PlannedFormations_PlannedFormationId",
                        column: x => x.PlannedFormationId,
                        principalTable: "PlannedFormations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlannedCoaches_PlannedFormationId_Position",
                table: "PlannedCoaches",
                columns: new[] { "PlannedFormationId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_PlannedFormations_CoachSequenceHash",
                table: "PlannedFormations",
                column: "CoachSequenceHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlannedFormations_ReverseCoachSequenceHash",
                table: "PlannedFormations",
                column: "ReverseCoachSequenceHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlannedCoaches");

            migrationBuilder.DropTable(
                name: "PlannedFormations");
        }
    }
}
