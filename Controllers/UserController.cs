
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using server.Data;

[ApiController]
[Route("user")]
// [Authorize] good to do
public class UserController : ControllerBase
{

    private UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager){
        _userManager = userManager;
    }


    [HttpGet("info")]
    public async  Task<IActionResult> GetUserInfo(){

        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);


        return Ok(new {
            name = user.UserName,
            email = user.Email
        });

    }

    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        string path = "wwwroot/images/" + file.FileName;

        using (Stream stream = file.OpenReadStream())
        {
            using (var fileStream = System.IO.File.Create(path))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
        
        // save to the database the path, to the exact user, make a limit for the size of an image

        Console.WriteLine("File uploaded: " + file.FileName);
        return Ok();
    }
    

}
