using server.Data;
using server.Models.BlogModels;
using server.Models.UserModels;

namespace server.Contracts;

public interface IUserService
{

    Task<string> UploadImage(IFormFile file, string frontEndUrl, string userId);

    Task<bool> ResetProfileImage(ApplicationUser user);

    Task<bool> EditProfile(EditProfiileModel model, ApplicationUser user);

    Task<bool> DeleteProfile(ApplicationUser user);

    Task<List<PostedComments>> GetPostedComments(string id);

    Task<List<ApplicationUser>> GetAllUsers();

    Task<bool> UpdateAccountTypeForUsers(List<ApplicationUserModel> users);
    
    Task<ApplicationUser> GetUserInfoByBlogId(int blogId);
}