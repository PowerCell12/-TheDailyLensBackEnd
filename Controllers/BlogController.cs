using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using server.Data;
using server.Models.BlogModels;
using server.Data.Models.Comments;
using server.Models.UserModels;
using Microsoft.AspNetCore.Authorization;

namespace server.Controllers;

[ApiController]
[Route("blog")]
public class BlogController : ControllerBase
{

    private readonly List<string> validBlogTypes = ["new", "top"];

    private readonly TheDailyLensContext _context;

    private IJwtTokenService _jwtTokenService;

    private IBlogService _blogService;



    public BlogController(TheDailyLensContext context, IJwtTokenService jwtTokenService, IBlogService blogService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _blogService = blogService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateBlog([FromBody] CreateBlogModel data)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Validation failed" });
        }

        int blogId = await _blogService.CreateBlog(data);

        if (blogId == -1 || blogId == null) return BadRequest(new { message =  "Blog can't be created" });

        return Ok(blogId);
    }


    [HttpGet("list")]
    public async Task<IActionResult> GetBlogs([FromQuery] int amount, [FromQuery] string type)
    {

        if (amount < 1) amount = 10;

        if (!validBlogTypes.Contains(type)) return BadRequest(new { message = "Invalid blog type" } );

        List<HomePageBlogData> blogs = _blogService.GetBlogsByParam(amount, type);

        return Ok(blogs);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBlog([FromRoute] int id)
    {
        bool isDeleted = await _blogService.DeleteBlog(id);

        if (!isDeleted) return BadRequest(new { message = "Blog not found" });

        return Ok("Blog deleted");
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateBlog([FromRoute] int id, [FromBody] UpdateBlogModel data)
    {

        bool isUpdated = await _blogService.UpdateBlog(id, data);

        if (!isUpdated) return BadRequest(new { message = "Blog can't be updated" } );

        return Ok("Blog updated");

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBlog([FromRoute] int id)
    {
        HomePageBlogData blog = _blogService.GetBlogByTitle(id);

        if (blog == null) return BadRequest(new { message = "Blog not found" });

        return Ok(blog);
    }


    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetBlogComments([FromRoute] int id)
    {
        List<Comment> comments = _blogService.GetCommentsByBlogId(id).OrderByDescending(x => x.CreatedAt).ToList();

        if (comments == null) return BadRequest(new { message = "Blog not found" } );

        return Ok(comments);
    }

    [HttpPost("{id}/like")]
    [Authorize]
    public async Task<IActionResult> LikeBlog([FromRoute] int id, [FromBody] LikeBlog data)
    {

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        int likes = await _blogService.LikeBlog(id, user, data.Liked);

        if (likes == -2) return BadRequest(new { message  ="Blog not found" } );

        return Ok(likes);
    }


    [HttpGet("search/{query}")]
    public async Task<IActionResult> Search([FromRoute] string query)
    {
        List<SearchGetUsers> users = [];

        if (query == "top" || query == "latest")
        {
            users = await _blogService.SearchUsers(query.ToLower());
        }

        List<SearchGetBlogs> blogs = await _blogService.SearchBlogs(query.ToLower());

        return Ok(new { users, blogs });
    }


    [HttpGet("tags")]
    public async Task<IActionResult> getAllTags()
    {
        return Ok(_context.Tags.Select(x => x.Name).ToList());
    }


}