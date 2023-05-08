using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedditAnalyzer.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RevertChangeRelationUrlSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Urls_Submissions_Id",
                table: "Urls");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Urls_Id",
                table: "Submissions",
                column: "Id",
                principalTable: "Urls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Urls_Id",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Urls_Submissions_Id",
                table: "Urls",
                column: "Id",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
