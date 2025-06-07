using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Contracts;
using server.Data;
using server.Extentions;
using server.Models.BlogModels;
using server.Models.UserModels;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{

    private UserManager<ApplicationUser> _userManager;

    private readonly TheDailyLensContext _context;

    private IJwtTokenService _jwtTokenService;

    private IBlogService _blogService;

    private IUserService _userService;


    public UserController(UserManager<ApplicationUser> userManager, TheDailyLensContext context, IJwtTokenService jwtTokenService, IBlogService blogService, IUserService userService)
    {
        _blogService = blogService;
        _jwtTokenService = jwtTokenService;
        _context = context;
        _userManager = userManager;
        _userService = userService;
    }



    [HttpGet("info")]
    public async Task<IActionResult> GetUserInfo()
    {
        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        ApplicationUser user1 = await _context.Users
        .Include(u => u.LikedComments)
        .ThenInclude(c => c.Comment)
        .Include(u => u.DislikedComments)
        .ThenInclude(c => c.Comment)
        .Include(u => u.LikedBlogs)
        .ThenInclude(b => b.Blog)
        .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            name = user.UserName,
            email = user.Email,
            accountType = (int)user.AccountType,
            imageUrl = user.ImageUrl,
            bio = user.Bio,
            country = user.Country,
            fullName = user.FullName,
            id = user.Id,
            likedComments = user1.LikedComments.Select(x => x.Comment.Id).ToList(),
            dislikedComments = user1.DislikedComments.Select(x => x.Comment.Id).ToList(),
            likedBlogs = user.LikedBlogs.Select(x => x.Blog.Id).ToList(),
        });

    }

    [HttpGet("title/{username}")]
    public async Task<IActionResult> GetUserInfoByTitle([FromRoute] string username)
    {
        ApplicationUser user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }


        ApplicationUser user1 = await _context.Users
        .Include(u => u.LikedComments)
        .ThenInclude(c => c.Comment)
        .Include(u => u.DislikedComments)
        .ThenInclude(c => c.Comment)
        .Include(u => u.LikedBlogs)
        .ThenInclude(b => b.Blog)
        .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (user1 == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            name = user.UserName,
            email = user.Email,
            accountType = (int)user.AccountType,
            imageUrl = user.ImageUrl,
            bio = user.Bio,
            country = user.Country,
            fullName = user.FullName,
            id = user.Id,
            likedComments = user1.LikedComments.Select(x => x.Comment.Id).ToList(),
            dislikedComments = user1.DislikedComments.Select(x => x.Comment.Id).ToList(),
            likedBlogs = user.LikedBlogs.Select(x => x.Blog.Id).ToList(),
        });
    }


    [HttpGet("{authorId}")]
    public async Task<IActionResult> GetUserInfo([FromRoute] string authorId)
    {

        ApplicationUser user = await _userManager.FindByIdAsync(authorId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            name = user.UserName,
            imageUrl = user.ImageUrl,

        });
    }


    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string frontEndUrl, [FromForm] string userId)
    {
        string path1 = await _userService.UploadImage(file, frontEndUrl, userId);


        if (path1 == "File is too large")
        {
            return BadRequest(new { message = path1 });
        }
        else if (path1 == "Not Found")
        {
            return NotFound(new { message = path1 });
        }

        return Ok(new { imageUrl = path1 });
    }



    [HttpPost("resetProfileImage")]
    public async Task<IActionResult> ResetProfileImage([FromBody] string userId)
    {
        ApplicationUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);


        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        bool result = await _userService.ResetProfileImage(user);

        if (!result)
        {
            return BadRequest(new { message = "There was an error, please try again" });
        }

        return Ok();
    }



    [HttpPost("editProfile")]
    [Authorize]
    public async Task<IActionResult> EditProfile([FromBody] EditProfiileModel model)
    {

        if (ModelState.IsValid)
        {
            ApplicationUser user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == model.CurrentName);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            bool result = await _userService.EditProfile(model, user);

            if (!result)
            {
                return BadRequest(new { message = "There was an error, please try again" });
            }

            return Ok(new
            {
                username = user.UserName,
                email = user.Email,
                bio = user.Bio,
                country = user.Country,
                fullName = user.FullName
            });
        }
        else
        {
            return BadRequest(new { message = "There was an error, please try again", Errors = ModelState.GetErrors() });
        }


    }


    [HttpDelete("deleteProfile")]
    [Authorize]
    public async Task<IActionResult> DeleteProfile([FromBody] string userId)
    {

        ApplicationUser user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        await _context.Entry(user).Collection(x => x.LikedBlogs).LoadAsync();





        bool result = await _userService.DeleteProfile(user);

        if (!result)
        {
            return BadRequest(new { message = "There was an error, please try again" });
        }

        return Ok();

    }


    [HttpGet("{userName}/getBlogsByUser")]
    [Authorize]
    public async Task<IActionResult> GetBlogsByUser([FromRoute] string userName)
    {
        List<HomePageBlogData> blogs = _blogService.GetBlogsByUserId(userName);

        return Ok(blogs);
    }

    [HttpGet("{userName}/getLikedBlogs")]
    [Authorize]
    public async Task<IActionResult> GetLikedBlogs([FromRoute] string userName)
    {
        List<HomePageBlogData> blogs = await _blogService.getLikedBlogsByUserId(userName);

        if (blogs == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(blogs);

    }

    [HttpGet("postedComments/{id}")]
    [Authorize]
    public async Task<IActionResult> GetPostedComments([FromRoute] string id)
    {
        List<PostedComments> comments = await _userService.GetPostedComments(id);

        if (comments == null)
        {
            return NotFound(new { message = "User not found or has no comments" });
        }

        return Ok(comments);
    }


    [HttpGet("getAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        List<ApplicationUser> users = await _userService.GetAllUsers();

        return Ok(users);
    }


    [HttpPost("updateAccountTypeForUsers")]
    [Authorize]
    public async Task<IActionResult> UpdateAccountTypeForUsers([FromBody] List<ApplicationUserModel> users)
    {
        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        if (user.AccountType == null || user.AccountType != AccountType.Admin)
        {
            return Forbid();
        }


        bool result = await _userService.UpdateAccountTypeForUsers(users);

        if (result == false)
        {
            return BadRequest(new { message = "There was an error, please try again" });
        }

        return Ok();
    }


    [HttpPost("getUserInfoByByBlogId")]
    public async Task<IActionResult> GetUserInfoByBlogId([FromBody] int blogId)
    {
        ApplicationUser user = await _userService.GetUserInfoByBlogId(blogId);

        if (user == null)
        {
            return NotFound(new { message = "User is not found" });
        }

        return Ok(new
        {
            name = user.UserName,
            email = user.Email,
            accountType = (int)user.AccountType,
            imageUrl = user.ImageUrl,
            bio = user.Bio,
            country = user.Country,
            fullName = user.FullName,
            id = user.Id,
            likedComments = user.LikedComments.Select(x => x.Comment.Id).ToList(),
            dislikedComments = user.DislikedComments.Select(x => x.Comment.Id).ToList(),
            likedBlogs = user.LikedBlogs.Select(x => x.Blog.Id).ToList(),
        });
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        // NEEDS SECURITY WITH TOKENS

        bool isValid = await _userService.ResetPassword(model);


        if (!isValid)
        {
            return BadRequest(new { message = "There was an error resetting the password." });
        }

        return Ok(new { message = "Password reset successfully" });
    }


    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
    {
        ApplicationUser user = await _userService.GetUserByEmail(email);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(user);
    }


}
