using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PATHServer.Migrations
{
    /// <inheritdoc />
    public partial class DataLiveModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataLives",
                columns: table => new
                {
                    dlid = table.Column<int>(name: "dl_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    dlname = table.Column<string>(name: "dl_name", type: "TEXT", maxLength: 255, nullable: false),
                    dlvalint = table.Column<string>(name: "dl_val_int", type: "TEXT", nullable: false),
                    dlvaldouble = table.Column<float>(name: "dl_val_double", type: "REAL", nullable: false),
                    dlvalstring = table.Column<string>(name: "dl_val_string", type: "TEXT", maxLength: 255, nullable: false),
                    dlvaldatetime = table.Column<DateTime>(name: "dl_val_datetime", type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLives", x => x.dlid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataLives");
        }
    }
}
