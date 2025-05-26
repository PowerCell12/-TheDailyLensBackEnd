namespace server.Models.BlogModels;

public class PostedComments
{

    public int Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public int Likes { get; set; }

    public int Dislikes { get; set; }

    public bool IsLiked { get; set; }

    public bool IsDisliked { get; set; }

    public int BlogId { get; set; }


    public bool Show { get; set; }
}