using System;
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
                name: "ActionHistories",
                columns: table => new
                {
                    ahid = table.Column<int>(name: "ah_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ahdate = table.Column<DateTime>(name: "ah_date", type: "TEXT", nullable: false),
                    ahiid = table.Column<int>(name: "ahi_id", type: "INTEGER", nullable: false),
                    puid = table.Column<int>(name: "pu_id", type: "INTEGER", nullable: false),
                    actid = table.Column<int>(name: "act_id", type: "INTEGER", nullable: false)
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
                name: "ActionTriggers",
                columns: table => new
                {
                    ahid = table.Column<int>(name: "ah_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    acttypedata = table.Column<int>(name: "act_type_data", type: "INTEGER", nullable: false),
                    actname = table.Column<string>(name: "act_name", type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTriggers", x => x.ahid);
                });

            migrationBuilder.CreateTable(
                name: "DataHistories",
                columns: table => new
                {
                    dhid = table.Column<int>(name: "dh_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    dhdate = table.Column<DateTime>(name: "dh_date", type: "TEXT", nullable: false),
                    nodeid = table.Column<int>(name: "node_id", type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    dhboolvalue = table.Column<bool>(name: "dh_bool_value", type: "INTEGER", nullable: true),
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
                    nodename = table.Column<string>(name: "node_name", type: "TEXT", maxLength: 50, nullable: false),
                    nodetypedata = table.Column<int>(name: "node_type_data", type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.nodeid);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    puid = table.Column<int>(name: "pu_id", type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    puemail = table.Column<string>(name: "pu_email", type: "TEXT", maxLength: 500, nullable: false),
                    puname = table.Column<string>(name: "pu_name", type: "TEXT", maxLength: 255, nullable: false),
                    pusurname = table.Column<string>(name: "pu_surname", type: "TEXT", maxLength: 255, nullable: false),
                    pupassword = table.Column<string>(name: "pu_password", type: "TEXT", maxLength: 255, nullable: false),
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
                name: "ActionHistories");

            migrationBuilder.DropTable(
                name: "ActionHistoryInfos");

            migrationBuilder.DropTable(
                name: "ActionTriggers");

            migrationBuilder.DropTable(
                name: "DataHistories");

            migrationBuilder.DropTable(
                name: "Keys");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
