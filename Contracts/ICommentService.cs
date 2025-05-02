using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Data.Models.Comments;
using server.Models.BlogModels;

namespace server.Contracts;

public interface ICommentService{
        Task<Comment> CreateComment(CommentModel model, int id, ApplicationUser user);

        Task<int> LikeComment(int id, bool isLiked, ApplicationUser user);


        Task<int> DislikeComment(int id, bool isLiked, ApplicationUser user);

        Task<bool> DeleteComment(int id);

        Task<bool> UpdateComment(CommentModel model);

        Task<Comment> CreateReply(ReplyModel model);
}