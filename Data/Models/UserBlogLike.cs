using System.ComponentModel.DataAnnotations.Schema;
using server.Data.Models.Blogs;

namespace server.Data.Models;

public class UserBlogLike{
    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId {get; set;}
    public ApplicationUser ApplicationUser {get; set;}


    [ForeignKey("Blog")]
    public int BlogId {get; set;}
    public Blog Blog {get; set;}
}

