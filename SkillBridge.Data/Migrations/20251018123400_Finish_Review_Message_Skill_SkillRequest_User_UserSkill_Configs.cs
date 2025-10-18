using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class Finish_Review_Message_Skill_SkillRequest_User_UserSkill_Configs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SkillRequests_Skills_SkillId",
                table: "SkillRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillRequests_Users_RequesterId",
                table: "SkillRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Categories_CategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_UserSkills_SkillId",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RevieweeId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillRequests",
                table: "SkillRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillRequests_RequesterId",
                table: "SkillRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillRequests_SkillId",
                table: "SkillRequests");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "SkillRequests",
                newName: "SkillsRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Reviews",
                type: "TEXT",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SkillsRequests",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills",
                columns: new[] { "UserId", "SkillId", "IsOffering" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillsRequests",
                table: "SkillsRequests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_SkillId_IsOffering",
                table: "UserSkills",
                columns: new[] { "SkillId", "IsOffering" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_UserId",
                table: "UserSkills",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_UserId_IsOffering",
                table: "UserSkills",
                columns: new[] { "UserId", "IsOffering" });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Title_CategoryId",
                table: "Skills",
                columns: new[] { "Title", "CategoryId" },
                unique: true,
                filter: "[CategoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevieweeId_CreatedAt",
                table: "Reviews",
                columns: new[] { "RevieweeId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId_CreatedAt",
                table: "Reviews",
                columns: new[] { "ReviewerId", "CreatedAt" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Rating_1_5",
                table: "Reviews",
                sql: "\"Rating\" BETWEEN 1 AND 5");

            migrationBuilder.CreateIndex(
                name: "IX_SkillsRequests_RequesterId_Status",
                table: "SkillsRequests",
                columns: new[] { "RequesterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SkillsRequests_SkillId_Status",
                table: "SkillsRequests",
                columns: new[] { "SkillId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Categories_CategoryId",
                table: "Skills",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Categories_CategoryId",
                table: "Skills");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillsRequests_Skills_SkillId",
                table: "SkillsRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillsRequests_Users_RequesterId",
                table: "SkillsRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_UserSkills_SkillId_IsOffering",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_UserSkills_UserId",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_UserSkills_UserId_IsOffering",
                table: "UserSkills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_Title_CategoryId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RevieweeId_CreatedAt",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewerId_CreatedAt",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Rating_1_5",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkillsRequests",
                table: "SkillsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillsRequests_RequesterId_Status",
                table: "SkillsRequests");

            migrationBuilder.DropIndex(
                name: "IX_SkillsRequests_SkillId_Status",
                table: "SkillsRequests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "SkillsRequests",
                newName: "SkillRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Messages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SkillRequests",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSkills",
                table: "UserSkills",
                columns: new[] { "UserId", "SkillId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkillRequests",
                table: "SkillRequests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserSkills_SkillId",
                table: "UserSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevieweeId",
                table: "Reviews",
                column: "RevieweeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequests_RequesterId",
                table: "SkillRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequests_SkillId",
                table: "SkillRequests",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_SkillRequests_Skills_SkillId",
                table: "SkillRequests",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillRequests_Users_RequesterId",
                table: "SkillRequests",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Categories_CategoryId",
                table: "Skills",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSkills_Skills_SkillId",
                table: "UserSkills",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
