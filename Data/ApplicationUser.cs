using Microsoft.AspNetCore.Identity;
using server.Data.Models;
using server.Data.Models.Comments;
using server.Services;
namespace server.Data;

public class ApplicationUser: IdentityUser{


    public string? AccountType {get; set;}
    public string? ImageUrl {get; set;}

    public string? Bio {get; set;}

    public string? Country {get; set;}

    public string? FullName {get; set;}

    public ICollection<UserCommentLike> LikedComments { get; set; }    = [];
    public ICollection<UserCommentDislike> DislikedComments { get; set; } = [];
}