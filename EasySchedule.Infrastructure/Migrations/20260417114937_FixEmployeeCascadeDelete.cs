using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasySchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeeCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Employees_EmployeeId",
                table: "ShiftAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Employees_EmployeeId",
                table: "ShiftAssignments",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShiftAssignments_Employees_EmployeeId",
                table: "ShiftAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_ShiftAssignments_Employees_EmployeeId",
                table: "ShiftAssignments",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
