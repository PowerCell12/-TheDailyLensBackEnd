
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

public class GetUserByJwtTokenClass{

    private UserManager<IdentityUser> _userManager;

    public GetUserByJwtTokenClass(UserManager<IdentityUser> userManager){
        _userManager = userManager;
    }

    public async Task<IdentityUser> GetUserByJwtToken(String token){

        // Validate the token
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        return await _userManager.FindByIdAsync(userId);
    }

}

