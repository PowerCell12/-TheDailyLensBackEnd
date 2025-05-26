using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using server.Data.Models;
using server.Data.Models.Blogs;
using server.Data.Models.Comments;
using server.Data.Models.Tags;
using server.Models.SMTPModels;

namespace server.Data;

public class TheDailyLensContext: IdentityDbContext<ApplicationUser>{

    public DbSet<Blog> Blogs {get; set;}

    public DbSet<Comment> Comments {get; set;}


    public DbSet<Tag> Tags {get; set;}

    public DbSet<UserCommentLike> UserCommentLikes {get; set;}

    public DbSet<UserCommentDislike> UserCommentDislikes {get; set;}

    public DbSet<UserBlogLike> UserBlogLikes {get; set;}

    public DbSet<Subscribe> Subscribes {get; set;}




    public TheDailyLensContext(DbContextOptions<TheDailyLensContext> options) : base(options)
    {

    } 

    protected override void OnModelCreating(ModelBuilder builder){
        base.OnModelCreating(builder);

        builder.Entity<Comment>().HasOne(c => c.Author)
        .WithMany()
        .HasForeignKey(c => c.AuthorId)
        .OnDelete(DeleteBehavior.ClientCascade);



        builder.Entity<UserCommentLike>()
        .HasKey(ul => new { ul.ApplicationUserId, ul.CommentId });

        builder.Entity<UserCommentLike>()
        .HasOne(ul => ul.ApplicationUser)
        .WithMany(u => u.LikedComments)
        .HasForeignKey(ul => ul.ApplicationUserId)
        .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<UserCommentLike>()
        .HasOne(ul => ul.Comment)
        .WithMany(c => c.LikedByUsers)
        .HasForeignKey(ul => ul.CommentId)
        .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<UserCommentDislike>()
        .HasKey(ud => new { ud.ApplicationUserId, ud.CommentId });

        builder.Entity<UserCommentDislike>()
        .HasOne(ud => ud.ApplicationUser)
        .WithMany(u => u.DislikedComments)
        .HasForeignKey(ud => ud.ApplicationUserId)
        .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<UserCommentDislike>()
        .HasOne(ud => ud.Comment)
        .WithMany(c => c.DislikedByUsers)
        .HasForeignKey(ud => ud.CommentId)
        .OnDelete(DeleteBehavior.Cascade);

        


        
        builder.Entity<UserBlogLike>()
        .HasKey(ul => new { ul.ApplicationUserId, ul.BlogId});

        builder.Entity<UserBlogLike>()
        .HasOne(ul => ul.ApplicationUser)
        .WithMany(u => u.LikedBlogs)
        .HasForeignKey(ul => ul.ApplicationUserId)
        .OnDelete(DeleteBehavior.ClientCascade);

        builder.Entity<UserBlogLike>()
        .HasOne(ul => ul.Blog)
        .WithMany(u => u.LikedByUsers)
        .HasForeignKey(ul => ul.BlogId)
        .OnDelete(DeleteBehavior.Cascade);
    }

}