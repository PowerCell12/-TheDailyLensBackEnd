using Microsoft.AspNetCore.Identity;
namespace server.Data;

public class ApplicationUser: IdentityUser{


    public string? AccountType {get; set;}
    public string? ImageUrl {get; set;}

    public string? Bio {get; set;}

    public string? Country {get; set;}

    public string? FullName {get; set;}

}