using System.ComponentModel.DataAnnotations.Schema;
using server.Data.Models.Comments;

namespace server.Data.Models;

public class UserCommentLike{

    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId {get; set;}


    [ForeignKey("Comment")]
    public int CommentId {get; set;}

    public ApplicationUser ApplicationUser {get; set;}

    public Comment Comment {get; set;}
}