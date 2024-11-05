namespace server.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using server.Contracts;
using server.Extentions;
using server.Models.AuthModels;
using server.Services;

[ApiController]
[Route("auth")]
public class AuthenticationController: ControllerBase 
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    private readonly IAuthService _authService;
    
    public AuthenticationController(SignInManager<IdentityUser> signInManager, IJwtTokenService jwtTokenService, IAuthService authService)
    {
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _authService = authService;
    }

    // public IList<AuthenticationScheme> ExternalLogins { get; set; }




    [HttpPost("login")]
    public async Task<IActionResult> LoginPost([FromBody] AuthFormModel model)
    {
        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                string stringToken = await _jwtTokenService.GenerateJwtToken(model.Email);

                return Ok(stringToken);
            }
            else
            {
                var UserFailedError = ModelState.GetErrors();
                return BadRequest(new { Message = "Logging in the user failed", Errors = UserFailedError });
            }
        }

        var DataErrros = ModelState.GetErrors();

        return Unauthorized(new { Message = "Validation failed", Errors = DataErrros });
    }



    [HttpPost("register")]
    public async Task<IActionResult> RegisterPost([FromBody] AuthFormModel model){

        if (ModelState.IsValid){

            bool createdUser = await _authService.CreateUser(model);

            if (createdUser){
                var stringToken = await _jwtTokenService.GenerateJwtToken(model.Email);
                return Ok(stringToken);
            }
            else{
                var creatingUserErrors = ModelState.GetErrors();

                return BadRequest(new { Message = "Creating user failed", Errors = creatingUserErrors });
            }

        }
 
        var DataErrros = ModelState.GetErrors();
        
        return Unauthorized(new { Message = "Validation failed", Errors = DataErrros });
    }


    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutPost(){
        await _signInManager.SignOutAsync();

        return Ok();
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(){

        string token = Request.Headers.Authorization.ToString().Split(" ")[1];  


        var isValid = await _jwtTokenService.ValidateJwtToken(token);
        if (!isValid){
            return Unauthorized("Invalid token");
        }


        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);


        bool isExpired = DateTime.UtcNow >= jwtToken.ValidTo;

        if (isExpired){
                var userEmail = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
                var newToken = await _jwtTokenService.GenerateJwtToken(userEmail);

                return Ok(newToken);
        }
        else{
            return BadRequest("Token is not expired");
        }
        
    
    }

}



