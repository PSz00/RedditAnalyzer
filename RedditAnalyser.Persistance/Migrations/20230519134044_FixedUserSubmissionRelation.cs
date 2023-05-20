using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedditAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixedUserSubmissionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_Id",
                table: "Submissions");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_CreatorId",
                table: "Submissions",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_CreatorId",
                table: "Submissions",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_CreatorId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_CreatorId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_Id",
                table: "Submissions",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
