using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedditAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUrlIdInSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Urls_Id",
                table: "Submissions");

            migrationBuilder.AddColumn<Guid>(
                name: "UrlId",
                table: "Submissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UrlId",
                table: "Submissions",
                column: "UrlId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Urls_UrlId",
                table: "Submissions",
                column: "UrlId",
                principalTable: "Urls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Urls_UrlId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_UrlId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "UrlId",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Urls_Id",
                table: "Submissions",
                column: "Id",
                principalTable: "Urls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
