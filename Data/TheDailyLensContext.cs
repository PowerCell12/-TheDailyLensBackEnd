using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using server.Data.Models.Blogs;
using server.Data.Models.Comments;
using server.Data.Models.Replies;
using server.Data.Models.Tags;

namespace server.Data;

public class TheDailyLensContext: IdentityDbContext<ApplicationUser>{

    public DbSet<Blog> Blogs {get; set;}

    public DbSet<Comment> Comments {get; set;}

    public DbSet<Reply> Replies {get; set;}

    public DbSet<Tag> Tags {get; set;}


    public TheDailyLensContext(DbContextOptions<TheDailyLensContext> options): base(options){

    } 

    protected override void OnModelCreating(ModelBuilder builder){
     
        builder.Entity<Comment>().HasOne(c => c.Author)
        .WithMany()
        .HasForeignKey(c => c.AuthorId)
        .OnDelete(DeleteBehavior.Restrict);


        builder.Entity<Reply>().HasOne(r => r.Author)
        .WithMany()
        .HasForeignKey(r => r.AuthorId)
        .OnDelete(DeleteBehavior.Restrict); 


     
        base.OnModelCreating(builder);
    }

}