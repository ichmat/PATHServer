using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PATHServer.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionHistories",
                columns: table => new
                {
                    ahid = table.Column<int>(name: "ah_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ahdate = table.Column<DateTime>(name: "ah_date", type: "TEXT", nullable: false),
                    ahiid = table.Column<int>(name: "ahi_id", type: "INTEGER", nullable: false),
                    puid = table.Column<int>(name: "pu_id", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionHistories", x => x.ahid);
                });

            migrationBuilder.CreateTable(
                name: "ActionHistoryInfos",
                columns: table => new
                {
                    ahiid = table.Column<int>(name: "ahi_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ahiname = table.Column<string>(name: "ahi_name", type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionHistoryInfos", x => x.ahiid);
                });

            migrationBuilder.CreateTable(
                name: "DataHistories",
                columns: table => new
                {
                    dhid = table.Column<int>(name: "dh_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    dhdate = table.Column<DateTime>(name: "dh_date", type: "TEXT", nullable: false),
                    diid = table.Column<int>(name: "di_id", type: "INTEGER", nullable: false),
                    nodeid = table.Column<int>(name: "node_id", type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    dhdatevalue = table.Column<DateTime>(name: "dh_date_value", type: "TEXT", nullable: true),
                    dhdoublevalue = table.Column<double>(name: "dh_double_value", type: "REAL", nullable: true),
                    dhintvalue = table.Column<int>(name: "dh_int_value", type: "INTEGER", nullable: true),
                    dhstringvalue = table.Column<string>(name: "dh_string_value", type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataHistories", x => x.dhid);
                });

            migrationBuilder.CreateTable(
                name: "DataInfos",
                columns: table => new
                {
                    diid = table.Column<int>(name: "di_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    diname = table.Column<string>(name: "di_name", type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataInfos", x => x.diid);
                });

            migrationBuilder.CreateTable(
                name: "Keys",
                columns: table => new
                {
                    keyid = table.Column<string>(name: "key_id", type: "TEXT", maxLength: 255, nullable: false),
                    keycreated = table.Column<DateTime>(name: "key_created", type: "TEXT", maxLength: 255, nullable: false),
                    keylastUpdated = table.Column<DateTime>(name: "key_lastUpdated", type: "TEXT", nullable: false),
                    keyquota = table.Column<int>(name: "key_quota", type: "INTEGER", nullable: false),
                    keyquotaRefresh = table.Column<DateTime>(name: "key_quotaRefresh", type: "TEXT", nullable: false),
                    puid = table.Column<int>(name: "pu_id", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keys", x => x.keyid);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    nodeid = table.Column<int>(name: "node_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nodename = table.Column<string>(name: "node_name", type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.nodeid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionHistories");

            migrationBuilder.DropTable(
                name: "ActionHistoryInfos");

            migrationBuilder.DropTable(
                name: "DataHistories");

            migrationBuilder.DropTable(
                name: "DataInfos");

            migrationBuilder.DropTable(
                name: "Keys");

            migrationBuilder.DropTable(
                name: "Nodes");
        }
    }
}
