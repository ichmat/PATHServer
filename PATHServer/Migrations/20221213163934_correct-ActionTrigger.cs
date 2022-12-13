using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PATHServer.Migrations
{
    /// <inheritdoc />
    public partial class correctActionTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ah_id",
                table: "ActionTriggers",
                newName: "act_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "act_id",
                table: "ActionTriggers",
                newName: "ah_id");
        }
    }
}
