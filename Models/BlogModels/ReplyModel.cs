namespace server.Models.BlogModels;

public class ReplyModel{
    public string Content { get; set; }
    public string Title{ get; set; }

    public int ParentCommentId {get; set;}
}
