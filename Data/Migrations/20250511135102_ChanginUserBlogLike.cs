using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChanginUserBlogLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLikes_AspNetUsers_ApplicationUserId",
                table: "UserBlogLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLikes_Blogs_BlogId",
                table: "UserBlogLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogLikes_AspNetUsers_ApplicationUserId",
                table: "UserBlogLikes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogLikes_Blogs_BlogId",
                table: "UserBlogLikes",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLikes_AspNetUsers_ApplicationUserId",
                table: "UserBlogLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLikes_Blogs_BlogId",
                table: "UserBlogLikes");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogLikes_AspNetUsers_ApplicationUserId",
                table: "UserBlogLikes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogLikes_Blogs_BlogId",
                table: "UserBlogLikes",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
