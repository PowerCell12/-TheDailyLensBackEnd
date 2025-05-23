using server.Data;
using server.Models.UserModels;

namespace server.Contracts;

public interface IUserService{

    Task<string> UploadImage(IFormFile file, string frontEndUrl);

    Task<bool> ResetProfileImage(ApplicationUser user);

    Task<bool> EditProfile(EditProfiileModel model, ApplicationUser user);

    Task<bool> DeleteProfile(ApplicationUser user);

}