
using server.Data.Models.Blogs;

namespace server.Data.Models.Tags;

public class Tag{

    public int Id {get; set;}

    public string Name {get; set;}

    public List<Blog> Blogs {get; set;}  = [];

}