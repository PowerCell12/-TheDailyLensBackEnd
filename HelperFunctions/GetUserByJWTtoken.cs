
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using server.Data;

public class GetUserByJwtTokenClass{

    private UserManager<ApplicationUser> _userManager;

    public GetUserByJwtTokenClass(UserManager<ApplicationUser> userManager){
        _userManager = userManager;
    }

    public async Task<ApplicationUser> GetUserByJwtToken(String token){

        // Validate the token
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        return await _userManager.FindByIdAsync(userId);
    }

}

