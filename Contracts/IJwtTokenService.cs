using server.Data;

namespace server.Contracts;

public interface IJwtTokenService{

    public  Task<string> GenerateJwtToken(string Email);

    public Task<bool> ValidateJwtToken(string token); 

    public Task<ApplicationUser> GetUserByJwtToken(string token);

}