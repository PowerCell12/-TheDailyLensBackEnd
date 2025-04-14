
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
        if (file.Length > 5_242_880)
        {
            return BadRequest("File is too large");
        }

        string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        string path = "wwwroot/images/" + uniqueFileName;
        string path1 = "images/" + uniqueFileName;

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }


        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);

        if (user == null)
        {
            return NotFound("User not found");
        }


        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
            if (System.IO.File.Exists("wwwroot/" + user.ImageUrl)) System.IO.File.Delete("wwwroot/" + user.ImageUrl);
        }


        user.ImageUrl = path1;
        _context.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { imageUrl = path1});
    }
    

    [HttpPost("resetProfileImage")]
    public async Task<IActionResult> ResetProfileImage(){
        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ApplicationUser user = await new GetUserByJwtTokenClass(_userManager).GetUserByJwtToken(token);

        if (user == null){
            return NotFound("User not found");
        }

        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
            if (System.IO.File.Exists("wwwroot/" + user.ImageUrl)) System.IO.File.Delete("wwwroot/" + user.ImageUrl);
        }


        user.ImageUrl = "/PersonDefault.png";
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
