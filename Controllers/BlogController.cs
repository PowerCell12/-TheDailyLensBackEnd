using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using server.Data;
using server.Data.Models.Blogs;
using server.Models.BlogModels;

namespace server.Controllers;

[ApiController]
[Route("blog")]
public class BlogController: ControllerBase{

    private readonly List<string> validBlogTypes = ["new", "top"];


    private readonly TheDailyLensContext _context;

    private IJwtTokenService _jwtTokenService;

    private IBlogService _blogService;


    public BlogController(TheDailyLensContext context, IJwtTokenService jwtTokenService, IBlogService blogService){
        _context = context;
        _jwtTokenService = jwtTokenService;
        _blogService = blogService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlog([FromBody] CreateBlogModel data){

        if (!ModelState.IsValid){
            return BadRequest("Validation failed");
        }

        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken(token);

        Blog blog = new(){
            Title = data.Title,
            Content = data.Content,
            AuthorId = user.Id,
            Author = user,
            Likes = 0,
            CreatedAt = DateTime.Now,
            Thumbnail = data.Thumbnail
        };


        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();

        return Ok();

    } 


    [HttpGet]
    public async Task<IActionResult> GetBlogs([FromQuery] int amount, [FromQuery] string type){

        if (amount < 1) amount = 10;

        if (!validBlogTypes.Contains(type)) return BadRequest("Invalid blog type");

        List<HomePageBlogData> blogs = _blogService.GetBlogsByParam(amount, type);

        return Ok(blogs);
    }


}