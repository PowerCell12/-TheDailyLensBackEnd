using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using server.Data;
using server.Models.BlogModels;
using server.Data.Models.Blogs;
using server.Data.Models.Comments;
using server.Data.Models.Tags;
using server.Models.UserModels;

namespace server.Controllers;

[ApiController]
[Route("blog")]
public class BlogController: ControllerBase{

    private readonly List<string> validBlogTypes = ["new", "top"];


    private readonly TheDailyLensContext _context;

    private IJwtTokenService _jwtTokenService;

    private IBlogService _blogService;

    private ITagService _tagService;


    public BlogController(TheDailyLensContext context, IJwtTokenService jwtTokenService, IBlogService blogService, ITagService tagService){
        _context = context;
        _jwtTokenService = jwtTokenService;
        _blogService = blogService;
        _tagService = tagService;
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
            CreatedAt = DateTime.Now,
            Thumbnail = data.Thumbnail,
        };


        //  ASK IF IT WORKS BOTH THIS AND THE TAGSERVICE


        _context.Blogs.Add(blog);
        await _context.SaveChangesAsync();

        Console.WriteLine("THE BLOG ID IS:"  + blog.Id);

        List<Tag> tags = await _tagService.CreateTags(data.Tags, blog.Id);
        foreach (var tag in tags){
            blog.Tags.Add(tag);
        }


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

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateBlog([FromRoute] int id, [FromBody] UpdateBlogModel data){

        bool isUpdated = await _blogService.UpdateBlog(id, data);

        if (!isUpdated) return BadRequest("Blog can't be updated");

        return Ok("Blog updated");

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

    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeBlog([FromRoute] int id, [FromBody] LikeBlog data){

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        int likes = await _blogService.LikeBlog(id, user, data.Liked);

        if (likes == -2) return BadRequest("Blog not found");

        return Ok(likes);
    }


    [HttpGet("search/{query}")]
    public async Task<IActionResult> Search([FromRoute] string query){        
        List<SearchGetBlogs> blogs = await _blogService.SearchBlogs(query.ToLower());
        List<SearchGetUsers> users = await _blogService.SearchUsers(query.ToLower());

        return Ok(new {users, blogs});
    }

}