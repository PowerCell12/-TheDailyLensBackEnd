using server.Data;
using server.Data.Models.Blogs;
using server.Data.Models.Comments;
using server.Models.BlogModels;

namespace server.Contracts;

public interface IBlogService{

    List<HomePageBlogData> GetBlogsByParam(int amount, string type); 

    HomePageBlogData GetBlogByTitle(int id);

    List<Comment> GetCommentsByBlogId(int id);

    List<HomePageBlogData> GetBlogsByUserId(string tag);

    Task<bool> DeleteBlog(int id);

    Task<bool> UpdateBlog(int id, UpdateBlogModel data);

    Task<int> LikeBlog(int id, ApplicationUser user, bool isLiked);
}

