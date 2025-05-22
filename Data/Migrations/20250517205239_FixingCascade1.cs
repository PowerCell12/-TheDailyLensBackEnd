using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixingCascade1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislikes_Comments_CommentId",
                table: "UserCommentDislikes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentDislikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentDislikes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentDislikes_Comments_CommentId",
                table: "UserCommentDislikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislikes_Comments_CommentId",
                table: "UserCommentDislikes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentDislikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentDislikes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentDislikes_Comments_CommentId",
                table: "UserCommentDislikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
