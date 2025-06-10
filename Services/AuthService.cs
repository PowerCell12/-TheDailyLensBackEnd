using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models.AuthModels;

namespace server.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        private readonly TheDailyLensContext _context;


        public AuthService(UserManager<ApplicationUser> userManager, TheDailyLensContext context)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<bool> CreateUser(AuthFormModel model){

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, ImageUrl = model.ImageUrl };

            var createduUser =  await _userManager.CreateAsync(user, model.Password);

            return createduUser.Succeeded;
        }

        public async Task<(ApplicationUser, bool)> GetOrCreateUserFromGoogle(string email)
        {

            ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                return (user, false);
            }
            else
            {
                user = new ApplicationUser()
                {
                    UserName = email,
                    Email = email,
                    ImageUrl = "/PersonDefault.png"
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    return (user, true);
                }
                else
                {
                    throw new Exception("Failed to create user from Google");
                }
            }

        }
    }
}