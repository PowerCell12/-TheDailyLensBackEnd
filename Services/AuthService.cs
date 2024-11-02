using Microsoft.AspNetCore.Identity;
using server.Models.AuthModels;

namespace server.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;


        public AuthService(UserManager<IdentityUser> userManager){
            _userManager = userManager;
        }


        public async Task<bool> CreateUser(AuthFormModel model){

            var user = new IdentityUser() { UserName = model.Email, Email = model.Email };

            var createduUser =  await _userManager.CreateAsync(user, model.Password);

            return createduUser.Succeeded;
        }

    }
}