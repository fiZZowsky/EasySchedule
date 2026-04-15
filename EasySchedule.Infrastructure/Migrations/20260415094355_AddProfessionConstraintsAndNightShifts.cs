using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasySchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionConstraintsAndNightShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNightShift",
                table: "ShiftTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProfessionId",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CanWorkNightShifts",
                table: "Professions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ProfessionId",
                table: "Schedules",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Professions_ProfessionId",
                table: "Schedules",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Professions_ProfessionId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_ProfessionId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "IsNightShift",
                table: "ShiftTypes");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CanWorkNightShifts",
                table: "Professions");
        }
    }
}
