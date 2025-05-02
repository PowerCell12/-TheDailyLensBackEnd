using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using server.Data;
using server.Models.BlogModels;
using server.Data.Models.Blogs;
using server.Data.Models.Comments;

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

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

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


    [HttpGet("list")]
    public async Task<IActionResult> GetBlogs([FromQuery] int amount, [FromQuery] string type){

        if (amount < 1) amount = 10;

        if (!validBlogTypes.Contains(type)) return BadRequest("Invalid blog type");

        List<HomePageBlogData> blogs = _blogService.GetBlogsByParam(amount, type);

        return Ok(blogs);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog([FromRoute] int id){
        bool isDeleted = await _blogService.DeleteBlog(id);

        if (!isDeleted) return BadRequest("Blog not found");

        return Ok("Blog deleted");
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlog([FromRoute] int id){
        HomePageBlogData blog = _blogService.GetBlogByTitle(id);

        if (blog == null) return BadRequest("Blog not found");

        return Ok(blog);
    }


    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetBlogComments([FromRoute] int id){
        List<Comment> comments = _blogService.GetCommentsByBlogId(id).OrderByDescending(x => x.CreatedAt).ToList();
        
        if (comments == null) return BadRequest("Blog not found");

        return Ok(comments);
    }
}