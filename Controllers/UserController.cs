using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using server.Contracts;
using server.Data;
using server.Extentions;
using server.Models.BlogModels;
using server.Models.UserModels;

[ApiController]
[Route("user")]
// [Authorize] good to do
public class UserController : ControllerBase
{

    private UserManager<ApplicationUser> _userManager;

    private readonly TheDailyLensContext _context;

    private IJwtTokenService _jwtTokenService;

    private IBlogService _blogService;

    private readonly string[] notProfilePicture = ["CreateBlog", "ShowComment"];

    public UserController(UserManager<ApplicationUser> userManager, TheDailyLensContext context, IJwtTokenService jwtTokenService, IBlogService blogService)
    {
        _blogService = blogService;
        _jwtTokenService = jwtTokenService;
        _context = context;
        _userManager = userManager;
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

    [HttpGet("{authorId}")]
    public async Task<IActionResult> GetUserInfo([FromRoute] string authorId){

        ApplicationUser user = await _userManager.FindByIdAsync(authorId);

        return Ok(new
        {
            name = user.UserName,
            imageUrl = user.ImageUrl,

        });
    }


    [HttpPost("uploadImage")]   
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file,[FromForm] string frontEndUrl)
    {
        string path;
        string path1;

        if (file.Length > 5_242_880)
        {
            return BadRequest("File is too large");
        }


        switch(frontEndUrl){
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


        if (!notProfilePicture.Contains(frontEndUrl)){
            ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
                if (System.IO.File.Exists("wwwroot/" + user.ImageUrl)) System.IO.File.Delete("wwwroot/" + user.ImageUrl);
            }


            user.ImageUrl = path1;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        return Ok(new { imageUrl = path1});
    }
    


    [HttpPost("resetProfileImage")]
    public async Task<IActionResult> ResetProfileImage(){
        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        if (user == null){
            return NotFound("User not found");
        }

        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
            if (System.IO.File.Exists("wwwroot/" + user.ImageUrl)) System.IO.File.Delete("wwwroot/" + user.ImageUrl);
        }


        user.ImageUrl = "/PersonDefault.png";
        _context.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
    }



    [HttpPost("editProfile")]
    public async Task<IActionResult> EditProfile([FromBody] EditProfiileModel model){

        if (ModelState.IsValid){

            ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.Country = model.Country;
            user.Bio = model.Bio;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new {
                username = user.UserName,
                email = user.Email,
                bio = user.Bio,
                country = user.Country,
                fullName = user.FullName
            });
        }
        else{
            return BadRequest(new { Message = "There was an error, please try again", Errors = ModelState.GetErrors()});
        }


    }

    [HttpDelete("deleteProfile")]
    public async Task<IActionResult> DeleteProfile(){

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        if (user == null)
        {
            return NotFound("User not found");
        }


        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
            if (System.IO.File.Exists("wwwroot/" + user.ImageUrl)) System.IO.File.Delete("wwwroot/" + user.ImageUrl);
        }
        
        await _userManager.DeleteAsync(user);

        return Ok();

    }


    [HttpGet("{userName}/getBlogsByUser")]
    public async Task<IActionResult> GetBlogsByUser([FromRoute] string userName){
        List<HomePageBlogData> blogs = _blogService.GetBlogsByUserId(userName);

        return Ok(blogs);
    }

    [HttpGet("{userName}/getLikedBlogs")]
    public async Task<IActionResult> GetLikedBlogs([FromRoute] string userName){
        List<HomePageBlogData> blogs =  await _blogService.getLikedBlogsByUserId(userName);

        if (blogs == null)
        {
            return NotFound("User not found");
        }

        return Ok(blogs);

    }


}
