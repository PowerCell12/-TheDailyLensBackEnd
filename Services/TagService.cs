using Microsoft.EntityFrameworkCore;
using server.Contracts;
using server.Data;
using server.Data.Models.Blogs;
using server.Data.Models.Tags;

namespace server.Services;

public class TagService: ITagService{

    private readonly TheDailyLensContext _context;

    public TagService(TheDailyLensContext context){
        _context = context;
    }


    public async Task<List<Tag>> CreateTags(List<string> tags, int blogId){
    // if it exists just add to the list if not create it
        List<Tag> tagList = [];
        Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == blogId);

        foreach(string tag in tags){
            Tag? existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tag);

            if (existingTag == null){
                Tag newTag = new(){
                    Name = tag
                };

                _context.Tags.Add(newTag);
                newTag.Blogs.Add(blog);
                await _context.SaveChangesAsync();
                tagList.Add(newTag);
            }
            else{
                tagList.Add(existingTag);
            }
        }
        
        return tagList;
    }

    public async Task<bool> DeleteTags(List<Tag> tags, string blogTitle){
        //  check if used in any blogs

        foreach (var tag in tags){
            List<Blog> blogs = await _context.Blogs.Include(b => b.Tags)
                .Where(b => b.Tags.Any(t => t.Id == tag.Id))
                .ToListAsync();
            
            if (blogs.Count == 1 && blogs[0].Title == blogTitle){
                _context.Tags.Remove(tag);
            }
            else{
                foreach (var blog in blogs){
                    if (blogTitle == blog.Title)
                        blog.Tags.Remove(tag);
                    }
            }


        }

        await _context.SaveChangesAsync();
        return true;
    }


}

    
