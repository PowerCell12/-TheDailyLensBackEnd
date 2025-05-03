using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class LikesForBlogs3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLike_AspNetUsers_ApplicationUserId",
                table: "UserBlogLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLike_Blogs_BlogId",
                table: "UserBlogLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBlogLike",
                table: "UserBlogLike");

            migrationBuilder.RenameTable(
                name: "UserBlogLike",
                newName: "UserBlogLikes");

            migrationBuilder.RenameIndex(
                name: "IX_UserBlogLike_BlogId",
                table: "UserBlogLikes",
                newName: "IX_UserBlogLikes_BlogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBlogLikes",
                table: "UserBlogLikes",
                columns: new[] { "ApplicationUserId", "BlogId" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLikes_AspNetUsers_ApplicationUserId",
                table: "UserBlogLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBlogLikes_Blogs_BlogId",
                table: "UserBlogLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBlogLikes",
                table: "UserBlogLikes");

            migrationBuilder.RenameTable(
                name: "UserBlogLikes",
                newName: "UserBlogLike");

            migrationBuilder.RenameIndex(
                name: "IX_UserBlogLikes_BlogId",
                table: "UserBlogLike",
                newName: "IX_UserBlogLike_BlogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBlogLike",
                table: "UserBlogLike",
                columns: new[] { "ApplicationUserId", "BlogId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogLike_AspNetUsers_ApplicationUserId",
                table: "UserBlogLike",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBlogLike_Blogs_BlogId",
                table: "UserBlogLike",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
