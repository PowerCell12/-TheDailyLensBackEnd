using System.ComponentModel.DataAnnotations.Schema;
using server.Data;
using server.Data.Models.Comments;
using server.Data.Models.Tags;

namespace server.Data.Models.Blogs;

public class Blog{

    public int Id {get; set;}

    public string Title {get; set;}

    public string Thumbnail {get; set;}

    public string Content {get; set;}

    public DateTime CreatedAt {get; set;}

    [ForeignKey("Author")]
    public string AuthorId {get; set;}

    public ApplicationUser Author {get; set;}

    public List<Comment> Comments {get; set;} = [];

    public List<Tag> Tags {get; set;} = [];

    public List<UserBlogLike> LikedByUsers {get; set;} = [];

    public int Likes {get; set;} = 0;

}
