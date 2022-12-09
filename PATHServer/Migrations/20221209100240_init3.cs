using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PATHServer.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "node_type_data",
                table: "Nodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "dh_bool_value",
                table: "DataHistories",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "node_type_data",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "dh_bool_value",
                table: "DataHistories");
        }
    }
}
