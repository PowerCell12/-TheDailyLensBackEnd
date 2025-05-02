using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using server.Data.Models.Blogs;

namespace server.Data.Models.Comments;

public class Comment{

    public int Id {get; set;}

    public string Title {get; set;}

    public string Content {get; set;}

    public int Likes {get; set;}

    public int Dislikes {get; set;}

    public DateTime CreatedAt {get; set;}

    [ForeignKey("Author")]
    public string AuthorId {get; set;}

    public ApplicationUser Author {get; set;}

    [ForeignKey("Blog")]
    public int BlogId {get; set;}

    public Blog Blog {get; set;} = null!;

    public int? ParentCommentId { get; set; }
    
    public Comment? ParentComment { get; set; }

    public List<Comment> Replies { get; set; } = [];    


    public ICollection<UserCommentLike> LikedByUsers { get; set; }    = [];
    public ICollection<UserCommentDislike> DislikedByUsers { get; set; } = [];

}