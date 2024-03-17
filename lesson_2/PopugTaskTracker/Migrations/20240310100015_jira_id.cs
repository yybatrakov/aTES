using Microsoft.EntityFrameworkCore.Migrations;

namespace PopugTaskTracker.Migrations
{
    public partial class jira_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JiraId",
                table: "PopugTasks",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JiraId",
                table: "PopugTasks");
        }
    }
}
