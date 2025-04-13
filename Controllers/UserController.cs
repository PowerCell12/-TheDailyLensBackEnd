
using System;
using System.Data.OleDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using server.Data;
using server.Extentions;
using server.Models.UserModels;

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
    

    [HttpPost("uploadImageWithCancel")]
    public async Task<IActionResult> UploadImage([FromForm] string file){
        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);


        if (user == null)
        {
            return NotFound("User not found");
        }


        // FIRST DON'T MAKE THE IMAGE URL NULL make it the pat to the default image and fix in frontend
        // SECOND REMOVE THE IMAGE FROM THE SERVER 
        // THIRD THIS METHOD IS PROBABLY TWO METHODS SEE IF TRUE (CHECK DEEPSEEK)


        if (file == "1"){
            user.ImageUrl = null;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        if (file.Contains("wwwroot/images/")){
            while (file.Contains("wwwroot/images/")){
                file = file.Replace("wwwroot/images/", "");
            }
        }
        
        string path = "images/" + file;

        user.ImageUrl = path;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return Ok();

    }


    [HttpPost("editProfile")]
    public async Task<IActionResult> EditProfile([FromBody] EditProfiileModel model){

        if (ModelState.IsValid){

            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.Country = model.Country;
            user.Bio = model.Bio;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new {
                username = user.UserName,
                email = user.Email,
                bio = user.Bio,
                country = user.Country,
                fullName = user.FullName
            });
        }
        else{
            return BadRequest(new { Message = "There was an error, please try again", Errors = ModelState.GetErrors()});
        }


    }

    [HttpDelete("deleteProfile")]
    public async Task<IActionResult> DeleteProfile(){

        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);

        if (user == null)
        {
            return NotFound("User not found");
        }

        await _userManager.DeleteAsync(user);

        return Ok();

    }


}
