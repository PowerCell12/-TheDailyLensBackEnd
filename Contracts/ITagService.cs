using server.Data.Models.Tags;

namespace server.Contracts;

public interface ITagService{

    Task<List<Tag>> CreateTags(List<string> tags, int blogId);

    Task<bool> DeleteTags(List<Tag> tags, string blogTitle);
    
}