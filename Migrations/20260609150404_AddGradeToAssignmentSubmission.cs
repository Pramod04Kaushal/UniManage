using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniManage.Migrations
{
    /// <inheritdoc />
    public partial class AddGradeToAssignmentSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Grade",
                table: "AssignmentSubmissions",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "AssignmentSubmissions");
        }
    }
}
