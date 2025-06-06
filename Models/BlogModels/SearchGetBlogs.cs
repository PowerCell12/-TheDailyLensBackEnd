namespace server.Models.BlogModels;

public class SearchGetBlogs{

    public int Id {get; set;}

    public string Title {get; set;}

    public string Thumbnail {get; set;}

    public int Likes {get; set;}

    public DateTime CreatedAt {get; set;}

    public string AuthorId {get; set;}

    public string UserImageUrl {get; set;}

    public string UserName {get; set;}

    public List<string> Tags {get; set;}
}