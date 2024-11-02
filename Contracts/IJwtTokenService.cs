using server.Models.AuthModels;

namespace server.Contracts;

public interface IJwtTokenService{

    public  Task<string> GenerateJwtToken(AuthFormModel model);

}