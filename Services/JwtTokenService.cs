using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using server.Contracts;
using server.Data;
using server.Models.AuthModels;

namespace server.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    private   readonly UserManager<ApplicationUser> _userManager;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor){
        _configuration = configuration;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }



    public  async Task<string> GenerateJwtToken(string userName){

                var result1 = await _userManager.FindByNameAsync(userName);
                result1 ??= await _userManager.FindByEmailAsync(userName);

                var issuer = _configuration["Jwt:Issuer"];

                var audience = _configuration["Jwt:Audience"];

                var key = Encoding.UTF8.GetBytes
                (_configuration["Jwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                    [
                        new Claim("Id", result1.Id),
                        new Claim(JwtRegisteredClaimNames.Email, result1.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                    ]),
                    Expires = DateTime.Now.AddMinutes(30),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var stringToken = tokenHandler.WriteToken(token);

                return stringToken;
    }



    public async Task<bool> ValidateJwtToken(string token){
        var handler = new JwtSecurityTokenHandler();

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, 
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"]
        };

        var result = await handler.ValidateTokenAsync(token, tokenValidationParameters);

        if (!result.IsValid){
            return false;
        }

        return true;

    }


    public async Task<ApplicationUser> GetUserByJwtToken(){
        string token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        // Validate the token
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        ApplicationUser user = await _userManager.FindByIdAsync(userId);

        return user;
    }

}