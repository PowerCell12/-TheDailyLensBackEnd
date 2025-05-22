using server.Data;
using server.Data.Models.Blogs;
using server.Data.Models.Comments;
using server.Models.BlogModels;
using Microsoft.EntityFrameworkCore;
using server.Contracts;
using server.Data.Models;

namespace server.Services;


public class CommentService: ICommentService{

    private readonly TheDailyLensContext _context;

    private readonly IJwtTokenService _jwtTokenService;

    public CommentService(TheDailyLensContext context, IJwtTokenService jwtTokenService){
        _context = context;
        _jwtTokenService = jwtTokenService;
    }


     public async Task<Comment> CreateComment(CommentModel model, int BlogId, ApplicationUser user){
        Blog blog = await  _context.Blogs.Where(x => x.Id == BlogId).FirstOrDefaultAsync();

        Comment comment = new(){
            Title = model.Title,
            Content = model.Content,
            Blog = blog,
            BlogId = blog.Id,
            AuthorId = user.Id,
            Author = await _context.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync(),
            Likes = 0,
            Dislikes = 0,
            CreatedAt = DateTime.Now,
            ParentCommentId = null,
            ParentComment = null
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return comment;
    }





    public async Task<int> LikeComment(int id, bool isLiked, ApplicationUser user){
        UserCommentLike commentLike = await _context.UserCommentLikes.Where(x => x.CommentId == id && x.ApplicationUserId == user.Id).FirstOrDefaultAsync();
        Comment comment = await _context.Comments.Where(x => x.Id == id).FirstOrDefaultAsync() ?? throw new InvalidOperationException("Comment not found");


        if (commentLike == null && isLiked){
            commentLike = new(){
                    CommentId = id,
                    Comment = comment,
                    ApplicationUserId = user.Id,
                    ApplicationUser = user
                };

            await _context.UserCommentLikes.AddAsync(commentLike);
            comment.Likes++;
        }
        else if (commentLike != null && !isLiked){
            comment.Likes--;
            _context.UserCommentLikes.Remove(commentLike);


            if ( comment.Likes < 0){
                comment.Likes = 0;
            }
        }

        await _context.SaveChangesAsync();

        return  comment.Likes;
    }



    public async Task<int> DislikeComment(int id, bool isDisliked, ApplicationUser user){
       UserCommentDislike commentDislike = await _context.UserCommentDislikes.Where(x => x.CommentId == id && x.ApplicationUserId == user.Id).FirstOrDefaultAsync();
       Comment comment = await _context.Comments.Where(x => x.Id == id).FirstOrDefaultAsync() ?? throw new InvalidOperationException("Comment not found");

        if (commentDislike == null && isDisliked){
            commentDislike = new(){
                CommentId = id,
                Comment = comment,
                ApplicationUserId = user.Id,
                ApplicationUser = user
            };

            await _context.UserCommentDislikes.AddAsync(commentDislike);
            comment.Dislikes++;
        }else if (commentDislike != null && !isDisliked){
            commentDislike.Comment.Dislikes--;
            _context.UserCommentDislikes.Remove(commentDislike);

            if (comment.Dislikes < 0){
                comment.Dislikes = 0;
            }        
        }


        await _context.SaveChangesAsync();

        return comment.Dislikes;
    }



    public async Task<bool> DeleteComment(int id)
    {
        Comment comment = await _context.Comments.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (comment == null) return false;

        _context.Comments.Remove(comment);
        _context.UserCommentDislikes.RemoveRange(_context.UserCommentDislikes.Where(x => x.CommentId == id));
        _context.UserCommentLikes.RemoveRange(_context.UserCommentLikes.Where(x => x.CommentId == id));

        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> UpdateComment(CommentModel model){

        Comment comment = await _context.Comments.FindAsync(model.Id);

        if (comment == null) return false;

        comment.Title = model.Title;
        comment.Content = model.Content;

        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return true;

    }


    public async Task<Comment> CreateReply(ReplyModel model){
        Comment ParentComment = await _context.Comments.FindAsync(model.ParentCommentId);

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        if (ParentComment == null) return null;

        Comment reply = new(){
            Title = model.Title,
            Content = model.Content,
            Likes = 0,
            Dislikes = 0,
            CreatedAt = DateTime.Now,
            AuthorId = user.Id,
            Author = user,
            BlogId = ParentComment.BlogId,
            Blog = ParentComment.Blog,
            ParentCommentId = model.ParentCommentId,
            ParentComment = ParentComment,
        };

        _context.Comments.Add(reply);
        _context.Comments.Update(ParentComment);
        await _context.SaveChangesAsync();

        return reply;
    }


}