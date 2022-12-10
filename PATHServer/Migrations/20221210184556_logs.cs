using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PATHServer.Migrations
{
    /// <inheritdoc />
    public partial class logs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    logid = table.Column<string>(name: "log_id", type: "TEXT", nullable: false),
                    logwho = table.Column<int>(name: "log_who", type: "INTEGER", nullable: true),
                    logwhat = table.Column<string>(name: "log_what", type: "TEXT", nullable: false),
                    logtype = table.Column<string>(name: "log_type", type: "TEXT", nullable: false),
                    logwhen = table.Column<DateTime>(name: "log_when", type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.logid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
