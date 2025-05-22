using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixingCascade2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLikes_Comments_CommentId",
                table: "UserCommentLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentLikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentLikes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentLikes_Comments_CommentId",
                table: "UserCommentLikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLikes_Comments_CommentId",
                table: "UserCommentLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentLikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentLikes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentLikes_Comments_CommentId",
                table: "UserCommentLikes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
