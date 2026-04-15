using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasySchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftRequirementsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShiftTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecificDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    RequiredEmployeeCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftRequirements_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShiftRequirements_ShiftTypes_ShiftTypeId",
                        column: x => x.ShiftTypeId,
                        principalTable: "ShiftTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRequirements_ScheduleId",
                table: "ShiftRequirements",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftRequirements_ShiftTypeId",
                table: "ShiftRequirements",
                column: "ShiftTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftRequirements");
        }
    }
}
