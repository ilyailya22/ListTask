using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ListTask.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskList_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskListShare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TaskListId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskListShare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskListShare_TaskList_TaskListId",
                        column: x => x.TaskListId,
                        principalTable: "TaskList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskListShare_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskList_Created",
                table: "TaskList",
                column: "Created",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskList_OwnerId",
                table: "TaskList",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskList_UniqueId",
                table: "TaskList",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskListShare_TaskListId_UserId",
                table: "TaskListShare",
                columns: new[] { "TaskListId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskListShare_UniqueId",
                table: "TaskListShare",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskListShare_UserId",
                table: "TaskListShare",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UniqueId",
                table: "User",
                column: "UniqueId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskListShare");

            migrationBuilder.DropTable(
                name: "TaskList");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
