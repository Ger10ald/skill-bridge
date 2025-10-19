using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillRequestsLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillsRequests_Skills_SkillId",
                table: "SkillsRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillsRequests_Users_RequesterId",
                table: "SkillsRequests");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "SkillsRequests",
                type: "INTEGER",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CanceledAt",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CapturedAt",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeclinedAt",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndUtc",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredAt",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoldUntil",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAmount",
                table: "SkillsRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceCurrency",
                table: "SkillsRequests",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceCode",
                table: "SkillsRequests",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SkillsRequests",
                type: "BLOB",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartUtc",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_SkillsRequests_CreatedAt",
                table: "SkillsRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SkillsRequests_ReferenceCode",
                table: "SkillsRequests",
                column: "ReferenceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillsRequests_RequesterId",
                table: "SkillsRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillsRequests_SkillId",
                table: "SkillsRequests",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillsRequests_Status_StartUtc",
                table: "SkillsRequests",
                columns: new[] { "Status", "StartUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_SkillsRequests_Skills_SkillId",
                table: "SkillsRequests",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillsRequests_Users_RequesterId",
                table: "SkillsRequests",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillsRequests_Skills_SkillId",
                table: "SkillsRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillsRequests_Users_RequesterId",
                table: "SkillsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillsRequests_CreatedAt",
                table: "SkillsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillsRequests_ReferenceCode",
                table: "SkillsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillsRequests_RequesterId",
                table: "SkillsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillsRequests_SkillId",
                table: "SkillsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillsRequests_Status_StartUtc",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "CanceledAt",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "CapturedAt",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "DeclinedAt",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "EndUtc",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "HoldUntil",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "PriceAmount",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "PriceCurrency",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "ReferenceCode",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "StartUtc",
                table: "SkillsRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SkillsRequests",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldMaxLength: 30);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillsRequests_Skills_SkillId",
                table: "SkillsRequests",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillsRequests_Users_RequesterId",
                table: "SkillsRequests",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
