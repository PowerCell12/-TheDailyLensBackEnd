
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using server.Data;

[ApiController]
[Route("user")]
// [Authorize] good to do
public class UserController : ControllerBase
{

    private UserManager<ApplicationUser> _userManager;

    private readonly TheDailyLensContext _context;

    public UserController(UserManager<ApplicationUser> userManager, TheDailyLensContext context)
    {
        _userManager = userManager;
        _context = context;
    }


    [HttpGet("info")]
    public async Task<IActionResult> GetUserInfo()
    {

        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);


        return Ok(new
        {
            name = user.UserName,
            email = user.Email,
            accountType = user.AccountType,
            imageUrl = user.ImageUrl,
            bio = user.Bio,
            country = user.Country,
            fullName = user.FullName
        });

    }

    [HttpPost("uploadImage")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        string path = "wwwroot/images/" + file.FileName;
        string path1 = "images/" + file.FileName;

        using (Stream stream = file.OpenReadStream())
        {
            using (var fileStream = System.IO.File.Create(path))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        if (file.Length > 5_242_880)
        {
            return BadRequest("File is too large");
        }

        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);

        if (user == null)
        {
            return NotFound("User not found");
        }

        user.ImageUrl = path1;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
    }


}
