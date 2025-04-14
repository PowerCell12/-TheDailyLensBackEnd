

using System.ComponentModel.DataAnnotations.Schema;
using server.Data.Models.Comments;

namespace server.Data.Models.Replies;

public class Reply{

    public int Id {get; set;}

    public string Title {get; set;}

    public string Content {get; set;}

    public int Likes {get; set;}

    [ForeignKey("Author")]
    public string AuthorId {get; set;}

    public ApplicationUser Author {get; set;}

    [ForeignKey("Comment")]
    public int CommentId {get; set;}

    public Comment Comment {get; set;}
}