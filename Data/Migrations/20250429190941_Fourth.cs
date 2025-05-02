using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislike_AspNetUsers_ApplicationUserId",
                table: "UserCommentDislike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislike_Comments_CommentId",
                table: "UserCommentDislike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLike_AspNetUsers_ApplicationUserId",
                table: "UserCommentLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLike_Comments_CommentId",
                table: "UserCommentLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCommentLike",
                table: "UserCommentLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCommentDislike",
                table: "UserCommentDislike");

            migrationBuilder.RenameTable(
                name: "UserCommentLike",
                newName: "UserCommentLikes");

            migrationBuilder.RenameTable(
                name: "UserCommentDislike",
                newName: "UserCommentDislikes");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommentLike_CommentId",
                table: "UserCommentLikes",
                newName: "IX_UserCommentLikes_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommentDislike_CommentId",
                table: "UserCommentDislikes",
                newName: "IX_UserCommentDislikes_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCommentLikes",
                table: "UserCommentLikes",
                columns: new[] { "ApplicationUserId", "CommentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCommentDislikes",
                table: "UserCommentDislikes",
                columns: new[] { "ApplicationUserId", "CommentId" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentDislikes_Comments_CommentId",
                table: "UserCommentDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLikes_AspNetUsers_ApplicationUserId",
                table: "UserCommentLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommentLikes_Comments_CommentId",
                table: "UserCommentLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCommentLikes",
                table: "UserCommentLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCommentDislikes",
                table: "UserCommentDislikes");

            migrationBuilder.RenameTable(
                name: "UserCommentLikes",
                newName: "UserCommentLike");

            migrationBuilder.RenameTable(
                name: "UserCommentDislikes",
                newName: "UserCommentDislike");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommentLikes_CommentId",
                table: "UserCommentLike",
                newName: "IX_UserCommentLike_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommentDislikes_CommentId",
                table: "UserCommentDislike",
                newName: "IX_UserCommentDislike_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCommentLike",
                table: "UserCommentLike",
                columns: new[] { "ApplicationUserId", "CommentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCommentDislike",
                table: "UserCommentDislike",
                columns: new[] { "ApplicationUserId", "CommentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentDislike_AspNetUsers_ApplicationUserId",
                table: "UserCommentDislike",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentDislike_Comments_CommentId",
                table: "UserCommentDislike",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentLike_AspNetUsers_ApplicationUserId",
                table: "UserCommentLike",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommentLike_Comments_CommentId",
                table: "UserCommentLike",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
