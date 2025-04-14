using Microsoft.AspNetCore.Identity;
using server.Data;
using server.Models.AuthModels;

namespace server.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public AuthService(UserManager<ApplicationUser> userManager){
            _userManager = userManager;
        }


        public async Task<bool> CreateUser(AuthFormModel model){

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, ImageUrl = model.ImageUrl };

            var createduUser =  await _userManager.CreateAsync(user, model.Password);

            return createduUser.Succeeded;
        }

    }
}