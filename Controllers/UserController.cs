
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using server.Data;

[ApiController]
[Route("user")]
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
    

}
