namespace server.Controllers;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Contracts;
using server.Data;
using server.Extentions;
using server.Models.AuthModels;
using server.Services;

[ApiController]
[Route("auth")]
public class AuthenticationController: ControllerBase 
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    private readonly IAuthService _authService;

    private readonly TheDailyLensContext _context;

    public AuthenticationController(SignInManager<ApplicationUser> signInManager, IJwtTokenService jwtTokenService, IAuthService authService, TheDailyLensContext context)
    {
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _authService = authService;
        _context = context;
    }

    // public IList<AuthenticationScheme> ExternalLogins { get; set; }




    [HttpPost("login")]
    public async Task<IActionResult> LoginPost([FromBody] AuthFormModel model)
    {
        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            ApplicationUser user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: false); 
            if (result.Succeeded)
            {
                string stringToken = await _jwtTokenService.GenerateJwtToken(user.UserName);

                return Ok(new { token = stringToken });
            }
            else
            {
                var UserFailedError = ModelState.GetErrors();
                return BadRequest(new { message = "Logging in the user failed", Errors = UserFailedError });
            }
        }
        
        var DataErrros = ModelState.GetErrors();

        return Unauthorized(new { message = "Validation failed", Errors = DataErrros });
    }



    [HttpPost("register")]
    public async Task<IActionResult> RegisterPost([FromBody] AuthFormModel model){

        if (ModelState.IsValid){
            bool createdUser = await _authService.CreateUser(model);

            if (createdUser){
                var stringToken = await _jwtTokenService.GenerateJwtToken(model.Email);
                return Ok(new { token = stringToken } );
            }
            else{
                var creatingUserErrors = ModelState.GetErrors();

                return BadRequest(new { message = "Username or Gmail is already in use", Errors = creatingUserErrors });
            }

        }
 
        var DataErrros = ModelState.GetErrors();
        
        return Unauthorized(new { message = "Validation failed", Errors = DataErrros });
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutPost()
    {
        await _signInManager.SignOutAsync();

        return Ok();
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {

        string token = Request.Headers.Authorization.ToString().Split(" ")[1];


        var isValid = await _jwtTokenService.ValidateJwtToken(token);
        if (!isValid)
        {
            return Unauthorized(new { message = "Invalid token" } );
        }


        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);


        bool isExpired = DateTime.UtcNow >= jwtToken.ValidTo;

        if (isExpired)
        {
            var userEmail = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            var newToken = await _jwtTokenService.GenerateJwtToken(userEmail);

            return Ok(newToken);
        }
        else
        {
            return BadRequest(new { message = "Token is not expired" });
        }


    }

}



