using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasySchedule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfessionId",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ProfessionId",
                table: "Employees",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Professions_ProfessionId",
                table: "Employees",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Professions_ProfessionId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ProfessionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "Employees");
        }
    }
}
