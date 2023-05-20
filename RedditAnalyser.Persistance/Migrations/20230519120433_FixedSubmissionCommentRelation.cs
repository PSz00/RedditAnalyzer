using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedditAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixedSubmissionCommentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Submissions_Id",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Comments_SubmissionId",
                table: "Comments",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Submissions_SubmissionId",
                table: "Comments",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Submissions_SubmissionId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_SubmissionId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Submissions_Id",
                table: "Comments",
                column: "Id",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
