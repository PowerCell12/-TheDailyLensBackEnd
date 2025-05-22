using Microsoft.AspNetCore.Identity;
using server.Contracts;
using server.Data;
using server.Models.UserModels;
using System.Drawing;
using System.IO;

namespace server.Services;

public class UserService: IUserService{

    private readonly TheDailyLensContext _context;

    private readonly IJwtTokenService _jwtTokenService;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly string[] notProfilePicture = ["CreateBlog", "ShowComment"];


    public UserService(TheDailyLensContext context, IJwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager){
        _context = context;
        _jwtTokenService = jwtTokenService;
        _userManager = userManager;
    }


    public async Task<string> UploadImage(IFormFile file, string frontEndUrl){
        string path;
        string path1;

        if (file.Length > 5_242_880)
        {
            return "File is too large";
        }


        switch(frontEndUrl){
            case "CreateBlog":
                path = "wwwroot/images/BlogsImages/" + file.FileName;
                path1 = "images/BlogsImages/" + file.FileName;
                break;
            case "ShowComment":
                path = "wwwroot/images/CommentImages/" + file.FileName;
                path1 = "images/CommentImages/" + file.FileName;
                break;
            default:
                string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                path = "wwwroot/images/" + uniqueFileName;
                path1 = "images/" + uniqueFileName;
                break;
        }

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }


        if (!notProfilePicture.Contains(frontEndUrl)){
            ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

            if (user == null)
            {
                return "Not Found";
            }

            if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
                if (File.Exists("wwwroot/" + user.ImageUrl)) File.Delete("wwwroot/" + user.ImageUrl);
            }


            user.ImageUrl = path1;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
        
        return path1;
    }


    public async Task<bool> ResetProfileImage(ApplicationUser user){
        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
            if (File.Exists("wwwroot/" + user.ImageUrl)) File.Delete("wwwroot/" + user.ImageUrl);
        }


        user.ImageUrl = "/PersonDefault.png";
        _context.Update(user);
        await _context.SaveChangesAsync();
    
        return true;
    }


    public async Task<bool> EditProfile(EditProfiileModel model, ApplicationUser user){
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.Country = model.Country;
            user.Bio = model.Bio;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return true;
    }


    public async Task<bool> DeleteProfile(ApplicationUser user){
        if (!string.IsNullOrEmpty(user.ImageUrl) && !user.ImageUrl.EndsWith("/PersonDefault.png")){
            if (File.Exists("wwwroot/" + user.ImageUrl)) File.Delete("wwwroot/" + user.ImageUrl);
        }
        
        await _userManager.DeleteAsync(user);
    
    
        return true;
    }

    
}