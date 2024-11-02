using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using server.Contracts;
using server.Models.AuthModels;

namespace server.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration configuration;

    private   readonly UserManager<IdentityUser> _userManager;



    public JwtTokenService(IConfiguration configuration, UserManager<IdentityUser> userManager){
        this.configuration = configuration;
        _userManager = userManager;
    }



    public  async Task<string> GenerateJwtToken(AuthFormModel model){

                var result1 = await _userManager.FindByEmailAsync(model.Email);

                
                var issuer = configuration["Jwt:Issuer"];

                var audience = configuration["Jwt:Audience"];

                var key = Encoding.UTF8.GetBytes
                (configuration["Jwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                    [
                        new Claim("Id", result1.Id),
                        new Claim(JwtRegisteredClaimNames.Email, model.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                    ]),
                    Expires = DateTime.Now.AddHours(5),
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





}