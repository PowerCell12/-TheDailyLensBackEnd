using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Contracts;
using server.Data;
using server.Data.Models;
using server.Data.Models.Blogs;
using server.Data.Models.Comments;
using server.Data.Models.Tags;
using server.Models.BlogModels;
using server.Extentions;
using server.Models.UserModels;

namespace server.Services;

public class BlogService: IBlogService{

    private TheDailyLensContext _context;

    private ITagService _tagService;

    public BlogService(TheDailyLensContext context, ITagService tagService){
        _context = context;
        _tagService = tagService;
    }


    public List<HomePageBlogData> GetBlogsByParam(int amount, string type){
            List<HomePageBlogData> blogs = _context.Blogs
        .Select(x => new HomePageBlogData {
            Id      = x.Id,
            Title   = x.Title,
            Thumbnail = x.Thumbnail,
            Content = x.Content,
            CreatedAt = x.CreatedAt,
            Likes = x.Likes,
            AuthorId = x.AuthorId,
            UserImageUrl = x.Author.ImageUrl,
            UserName = x.Author.UserName,
            Tags = x.Tags.Select(t => t.Name).ToList()
        })
        .ToList();

        switch(type){
            case "new":         
            blogs = blogs.OrderByDescending(x => x.CreatedAt).Take(amount).ToList();
            break;

            case "top":
            blogs = blogs.OrderByDescending(x => x.Likes).Take(amount).ToList();
            break;
        }



        return blogs;
    }

    public HomePageBlogData GetBlogByTitle(int id){

        HomePageBlogData blog = _context.Blogs.Where(x => x.Id == id).Select(x => new HomePageBlogData {
            Id = x.Id,
            Title   = x.Title,
            Thumbnail = x.Thumbnail,
            Content = x.Content,
            Likes = x.Likes,
            CreatedAt = x.CreatedAt,
            AuthorId = x.Author.Id,
            UserImageUrl = x.Author.ImageUrl,
            UserName = x.Author.UserName,
            Tags = x.Tags.Select(t => t.Name).ToList(),
            LikedUsers = x.LikedByUsers.Where(x => x.BlogId == id).Select(u => u.ApplicationUserId).ToList()
        }).FirstOrDefault();

        Console.WriteLine("The Author ID IS: " + blog.LikedUsers.Count);

        return blog;
    }

    public List<Comment> GetCommentsByBlogId(int id){

        var parentComments = _context.Comments
        .Where(x => x.Blog.Id == id && x.ParentCommentId == null)
        .Include(c => c.Replies)
        .ThenInclude(r => r.Author)
        .ToList();

        // Then flatten them in memory (after the DB query)
        List<Comment> allComments = parentComments
        .SelectMany(c => new[] { c }.Concat(c.Replies))
        .ToList();

        Console.WriteLine(allComments.Count);

        return allComments;
    }

    public List<HomePageBlogData> GetBlogsByUserId(string userName){

        List<HomePageBlogData> blogs = _context.Blogs
        .Where(x => x.Author.UserName  == userName)
        .Select(x => new HomePageBlogData {
            Id      = x.Id,
            Title   = x.Title,
            Thumbnail = x.Thumbnail,
            Content = x.Content,
            CreatedAt = x.CreatedAt,
            AuthorId = x.AuthorId,
            UserImageUrl = x.Author.ImageUrl,
            UserName = x.Author.UserName,
            Tags = x.Tags.Select(t => t.Name).ToList()
        })
        .ToList();

        return blogs;
    }

    public async Task<bool> DeleteBlog(int id){

        Blog blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);

        if (blog == null) return false;

        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateBlog(int id, UpdateBlogModel data){

        Blog blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);

        if (blog == null) return false;

        blog.Title = data.Title;
        blog.Content = data.Content;
        blog.Thumbnail = data.Thumbnail;

        var incomingTags = data.Tags;
        var existingTags = _context.Tags.Where(t => t.Blogs.Any(b => b.Id == id)).ToList();

        var tagsToAdd = incomingTags.Except(existingTags.Select(t => t.Name)).ToList();
        var tagsToRemove = existingTags.Where(t => !incomingTags.Contains(t.Name)).ToList();

        List<Tag> tags = await _tagService.CreateTags(tagsToAdd, id);
        foreach (var tag in tags){
            blog.Tags.Add(tag);
        }

        bool isDeleted = await _tagService.DeleteTags(tagsToRemove, blog.Title);

        if (!isDeleted) return false;

        _context.Blogs.Update(blog);
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<int> LikeBlog(int id, ApplicationUser user, bool isLiked){
        UserBlogLike like = await _context.UserBlogLikes.Where(x => x.BlogId == id && x.ApplicationUserId == user.Id).FirstOrDefaultAsync();
        Blog blog = await _context.Blogs.Include(x => x.LikedByUsers).FirstOrDefaultAsync(x => x.Id == id);

        if (blog == null) return -2;

        if (like == null && isLiked){            
            UserBlogLike likeConnection = new(){
                BlogId = blog.Id,
                Blog = blog,
                ApplicationUserId = user.Id,
                ApplicationUser = user
            };

            await _context.UserBlogLikes.AddAsync(likeConnection);
            blog.Likes++;
        }
        else if (like != null && !isLiked){
            blog.Likes--;
            _context.UserBlogLikes.Remove(like);

            if (blog.Likes < 0){
                blog.Likes = 0;
            }
        }
        
        await _context.SaveChangesAsync();
        return blog.Likes;

    }

    public async Task<List<HomePageBlogData>> getLikedBlogsByUserId(string userName)
    {

        List<HomePageBlogData> blog = _context.Blogs.Where( x => x.LikedByUsers.Any(u => u.ApplicationUser.UserName == userName))
        .Select(x => new HomePageBlogData {
            Id = x.Id,
            Title   = x.Title,
            Thumbnail = x.Thumbnail,
            Content = x.Content,
            Likes = x.Likes,
            CreatedAt = x.CreatedAt,
            AuthorId = x.Author.Id,
            UserImageUrl = x.Author.ImageUrl,
            UserName = x.Author.UserName,
            Tags = x.Tags.Select(t => t.Name).ToList()
        }).ToList();

        return blog;
    }

    public async Task<List<SearchGetBlogs>> SearchBlogs(string query){
        var queryable = await _context.Blogs.Include(x => x.Tags).Include(x => x.Author).ToListAsync();

        List<SearchGetBlogs> serverFiltered = queryable
        .Where(x => x.Title.ToLower().Contains(query) || HelperFunctions.StripHTML(x.Content).ToLower().Contains(query) || x.Tags.Any(x => x.Name.ToLower().Contains(query)))
        .Select(x => new SearchGetBlogs{
            Id = x.Id,
            Title = x.Title,
            Thumbnail = x.Thumbnail,
            Likes = x.Likes,
            CreatedAt = x.CreatedAt,
            AuthorId = x.Author.Id,
            UserImageUrl = x.Author.ImageUrl,
            UserName = x.Author.UserName,
            Tags = x.Tags.Select(t => t.Name).ToList()
        }).ToList();


        return serverFiltered;
    }




    public async Task<List<SearchGetUsers>> SearchUsers(string query){
       var queryable = await _context.Users.ToListAsync();

       List<SearchGetUsers> users = queryable
       .Where(x => x.UserName.ToLower().Contains(query) || x.Email.ToLower().Contains(query))
       .Select(x => new SearchGetUsers{
           Id = x.Id,
           UserName = x.UserName,
           Image = x.ImageUrl,
           Email = x.Email
       }).ToList();

        return users;
    }

}