using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Contracts;
using server.Data;
using server.Models.BlogModels;
using server.Models.UserModels;

namespace server.Services;

public class UserService : IUserService
{

    private readonly TheDailyLensContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly string[] notProfilePicture = ["CreateBlog", "ShowComment"];


    public UserService(TheDailyLensContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public async Task<string> UploadImage(IFormFile file, string frontEndUrl, string userId)
    {
        string path;
        string path1;

        if (file.Length > 5_242_880)
        {
            return "File is too large";
        }


        switch (frontEndUrl)
        {
            case "CreateBlog":
                path = "wwwroot/images/BlogsImages/" + file.FileName;
                path1 = "images/BlogsImages/" + file.FileName;
                break;
            case "ShowComment":
                path = "wwwroot/images/CommentImages/" + file.FileName;
                path1 = "images/CommentImages/" + file.FileName;
                break;
            default:
                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                path = "wwwroot/images/" + uniqueFileName;
                path1 = "images/" + uniqueFileName;
                break;
        }

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }


        if (!notProfilePicture.Contains(frontEndUrl))
        {
            ApplicationUser user = await _context.Users.FindAsync(userId);
            

            if (user == null)
            {
                return "Not Found";
            }

            if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png"))
            {
                if (File.Exists("wwwroot/" + user.ImageUrl)) File.Delete("wwwroot/" + user.ImageUrl);
            }


            user.ImageUrl = path1;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        return path1;
    }


    public async Task<bool> ResetProfileImage(ApplicationUser user)
    {
        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png"))
        {
            if (File.Exists("wwwroot/" + user.ImageUrl)) File.Delete("wwwroot/" + user.ImageUrl);
        }


        user.ImageUrl = "/PersonDefault.png";
        _context.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> EditProfile(EditProfiileModel model, ApplicationUser user)
    {
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.FullName = model.FullName;
        user.Country = model.Country;
        user.Bio = model.Bio;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> DeleteProfile(ApplicationUser user)
    {
        var Comments = _context.Comments.Where(x => x.AuthorId == user.Id).ToList();
        var Blogs = _context.Blogs.Where(x => x.AuthorId == user.Id).ToList();


        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png"))
        {
            if (File.Exists("wwwroot/" + user.ImageUrl)) File.Delete("wwwroot/" + user.ImageUrl);
        }

        _context.Comments.RemoveRange(Comments);
        _context.Blogs.RemoveRange(Blogs);

        await _context.SaveChangesAsync();

        await _userManager.DeleteAsync(user);

        return true;
    }


    public async Task<List<PostedComments>> GetPostedComments(string id)
    {
        return await _context.Comments.Where(x => x.AuthorId == id).Select(x => new PostedComments
        {
            Id = x.Id,
            Title = x.Title,
            Content = x.Content,
            CreatedAt = x.CreatedAt,
            Likes = x.Likes,
            Dislikes = x.Dislikes,
            IsLiked = x.LikedByUsers.Any(y => y.ApplicationUserId == id && y.CommentId == x.Id),
            IsDisliked = x.DislikedByUsers.Any(y => y.ApplicationUserId == id && y.CommentId == x.Id),
            BlogId = x.BlogId,
            Show = false,
        }).ToListAsync();
    }

    public async Task<List<ApplicationUser>> GetAllUsers()
    {
        return await _context.Users
            .Include(u => u.LikedComments)
            .ThenInclude(c => c.Comment)
            .Include(u => u.DislikedComments)
            .ThenInclude(c => c.Comment)
            .Include(u => u.LikedBlogs)
            .ThenInclude(b => b.Blog).ToListAsync();
    }

    public async Task<bool> UpdateAccountTypeForUsers(List<ApplicationUserModel> users)
    {
        foreach (var user in users)
        {
            ApplicationUser userWithId = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

            if (userWithId == null)
            {
                continue; // Skip if user not found
            }

            if (userWithId.AccountType != (AccountType)user.AccountType)
            {
                userWithId.AccountType = (AccountType)user.AccountType;
            }
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ApplicationUser> GetUserInfoByBlogId(int blogId)
    {

        string authorId = _context.Blogs.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == blogId).Result.AuthorId;

        ApplicationUser user = await _context.Users
        .Include(u => u.LikedComments)
        .ThenInclude(c => c.Comment)
        .Include(u => u.DislikedComments)
        .ThenInclude(c => c.Comment)
        .Include(u => u.LikedBlogs)
        .ThenInclude(b => b.Blog)
        .FirstOrDefaultAsync(x => x.Id == authorId);

        return user;

    }
}