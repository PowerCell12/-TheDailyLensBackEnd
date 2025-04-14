using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using server.Data.Models.Blogs;
using server.Data.Models.Replies;

namespace server.Data.Models.Comments;

public class Comment{

    public int Id {get; set;}

    public string Title {get; set;}

    public string Content {get; set;}

    public int Likes {get; set;}

    [ForeignKey("Author")]
    public string AuthorId {get; set;}

    public ApplicationUser Author {get; set;}

    [ForeignKey("Blog")]
    public int BlogId {get; set;}

    public Blog Blog {get; set;} = null!;

    public List<Reply> Replies {get; set;} = [];
}