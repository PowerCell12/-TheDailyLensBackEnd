using server.Contracts;
using server.Data;
using server.Data.Models.Blogs;
using server.Models.BlogModels;

namespace server.Services;

public class BlogService: IBlogService{

    private TheDailyLensContext _context;

    public BlogService(TheDailyLensContext context){
        _context = context;
    }


    public List<HomePageBlogData> GetBlogsByParam(int amount, string type){
            List<HomePageBlogData> blogs = _context.Blogs
        .Select(x => new HomePageBlogData {
            Id      = x.Id,
            Title   = x.Title,
            Thumbnail = x.Thumbnail,
            Content = x.Content,
            Likes   = x.Likes,
            CreatedAt = x.CreatedAt,
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

}