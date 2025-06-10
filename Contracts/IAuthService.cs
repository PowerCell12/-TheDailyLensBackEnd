using server.Data;
using server.Models.AuthModels;

namespace server.Services
{
    public interface IAuthService
    {

        public Task<bool> CreateUser(AuthFormModel model);

        public Task<(ApplicationUser, bool)> GetOrCreateUserFromGoogle(string email);

    }
}