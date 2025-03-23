
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{

    private UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager){
        _userManager = userManager;
    }


    [HttpGet("info")]
    public async  Task<IActionResult> GetUserInfo(){

        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        IdentityUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);


        return Ok(new {
            name = user.UserName,
            email = user.Email
        });

    }
    

}
