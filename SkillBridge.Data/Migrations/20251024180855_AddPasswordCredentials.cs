using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "SkillsRequests",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PasswordCredential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Hash = table.Column<byte[]>(type: "BLOB", maxLength: 32, nullable: false),
                    Salt = table.Column<byte[]>(type: "BLOB", maxLength: 16, nullable: false),
                    Iterations = table.Column<int>(type: "INTEGER", nullable: false),
                    Algorithm = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastVerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsRevoked = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordCredential", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordCredential_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordCredential_UserId",
                table: "PasswordCredential",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordCredential_UserId_IsRevoked",
                table: "PasswordCredential",
                columns: new[] { "UserId", "IsRevoked" },
                unique: true,
                filter: "IsRevoked = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordCredential");

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "SkillsRequests",
                type: "BLOB",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");
        }
    }
}
