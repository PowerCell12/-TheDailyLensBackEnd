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
            return NotFound("User not found");
        }

        return Ok(new
        {
            name = user.UserName,
            email = user.Email,
            accountType = user.AccountType,
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
            return NotFound("User not found");
        }


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
            return NotFound("User not found");
        }

        return Ok(new
        {
            name = user.UserName,
            email = user.Email,
            accountType = user.AccountType,
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

        return Ok(new
        {
            name = user.UserName,
            imageUrl = user.ImageUrl,

        });
    }


    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string frontEndUrl)
    {
        string path1 = await _userService.UploadImage(file, frontEndUrl);

        if (path1 == "File is too large")
        {
            return BadRequest(path1);
        }
        else if (path1 == "Not Found")
        {
            return NotFound(path1);
        }

        return Ok(new { imageUrl = path1 });
    }



    [HttpPost("resetProfileImage")]
    public async Task<IActionResult> ResetProfileImage()
    {
        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        if (user == null)
        {
            return NotFound("User not found");
        }

        bool result = await _userService.ResetProfileImage(user);

        if (!result)
        {
            return BadRequest("There was an error, please try again");
        }

        return Ok();
    }



    [HttpPost("editProfile")]
    public async Task<IActionResult> EditProfile([FromBody] EditProfiileModel model)
    {

        if (ModelState.IsValid)
        {

            ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

            if (user == null)
            {
                return NotFound("User not found");
            }

            bool result = await _userService.EditProfile(model, user);

            if (!result)
            {
                return BadRequest("There was an error, please try again");
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
            return BadRequest(new { Message = "There was an error, please try again", Errors = ModelState.GetErrors() });
        }


    }

    [HttpDelete("deleteProfile")]
    public async Task<IActionResult> DeleteProfile()
    {

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        if (user == null)
        {
            return NotFound("User not found");
        }

        await _context.Entry(user).Collection(x => x.LikedBlogs).LoadAsync();





        bool result = await _userService.DeleteProfile(user);

        if (!result)
        {
            return BadRequest("There was an error, please try again");
        }

        return Ok();

    }


    [HttpGet("{userName}/getBlogsByUser")]
    public async Task<IActionResult> GetBlogsByUser([FromRoute] string userName)
    {
        List<HomePageBlogData> blogs = _blogService.GetBlogsByUserId(userName);

        return Ok(blogs);
    }

    [HttpGet("{userName}/getLikedBlogs")]
    public async Task<IActionResult> GetLikedBlogs([FromRoute] string userName)
    {
        List<HomePageBlogData> blogs = await _blogService.getLikedBlogsByUserId(userName);

        if (blogs == null)
        {
            return NotFound("User not found");
        }

        return Ok(blogs);

    }

    [HttpGet("postedComments/{id}")]
    public async Task<IActionResult> GetPostedComments([FromRoute] string id)
    {
        List<PostedComments> comments = await _userService.GetPostedComments(id);

        if (comments == null)
        {
            return NotFound("User not found");
        }

        return Ok(comments);
    }


}
