using server.Data.Models.Blogs;
using server.Models.BlogModels;

namespace server.Contracts;

public interface IBlogService{

    List<HomePageBlogData> GetBlogsByParam(int amount, string type); 

}