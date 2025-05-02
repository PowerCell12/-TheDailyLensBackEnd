using System.ComponentModel.DataAnnotations.Schema;
using server.Data.Models.Comments;

namespace server.Data.Models;

public class UserCommentDislike{

    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId { get; set; }
    
    public ApplicationUser ApplicationUser { get; set; }
    
    
    [ForeignKey("Comment")]
    public int CommentId { get; set; }
    public Comment Comment { get; set; }

}