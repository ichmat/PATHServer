using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PATHServer.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    puid = table.Column<int>(name: "pu_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    puemail = table.Column<string>(name: "pu_email", type: "TEXT", maxLength: 500, nullable: false),
                    puname = table.Column<string>(name: "pu_name", type: "TEXT", maxLength: 255, nullable: false),
                    pusurname = table.Column<string>(name: "pu_surname", type: "TEXT", maxLength: 255, nullable: false),
                    puadmin = table.Column<bool>(name: "pu_admin", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.puid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
