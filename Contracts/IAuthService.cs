using server.Models.AuthModels;

namespace server.Services
{
    public interface IAuthService
    {

        public Task<bool> CreateUser(AuthFormModel model);

    }
}