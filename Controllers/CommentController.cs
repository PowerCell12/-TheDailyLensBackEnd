using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using server.Data;
using server.Data.Models.Comments;
using server.Models.BlogModels;
using server.Models.CommentModels;

namespace server.Controllers;

[ApiController]
[Route("comment")]
public class CommentController : ControllerBase{

    private readonly IJwtTokenService _jwtTokenService;

    private readonly ICommentService _commentService;

    public CommentController(IJwtTokenService jwtTokenService, ICommentService commentService){
        _jwtTokenService = jwtTokenService;
        _commentService = commentService;
    }


    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CommentModel data){
        
        if (!ModelState.IsValid){
            return BadRequest("Validation failed");
        }

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        Comment comment = await _commentService.CreateComment(data, data.Id, user);

        if (comment == null) return BadRequest("Can't create the comment, try again");

        return Ok(comment);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateComment([FromBody] CommentModel data){

        if (!ModelState.IsValid){
            return BadRequest("Validation failed");
        }

        bool isUpdated = await _commentService.UpdateComment(data);

        if (!isUpdated) return BadRequest("Can't update the comment, try again");

        return Ok();
    }



    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikeComment([FromRoute] int id, [FromBody] LikeDislikeRequest model){
        if (!ModelState.IsValid){
            return BadRequest("Validation failed");
        }

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        int Likes = await _commentService.LikeComment(id, model.IsLiked, user);


        return Ok(Likes);
    }


    [HttpPost("{id}/dislike")]
    public async Task<IActionResult> DislikeComment([FromRoute] int id, [FromBody] LikeDislikeRequest model){
    
        if( !ModelState.IsValid){
            return BadRequest("Validation failed");
        }

        ApplicationUser user = await _jwtTokenService.GetUserByJwtToken();

        int Dislikes = await _commentService.DislikeComment(id, model.IsLiked, user);

        return Ok(Dislikes);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id){
        bool isDeleted = await _commentService.DeleteComment(id);

        if (!isDeleted) return BadRequest("Can't delete the comment, try again");

        return Ok();
    }

    [HttpPost("reply")]
    public async Task<IActionResult> CreateReply([FromBody] ReplyModel model ){

        if (!ModelState.IsValid){
            return BadRequest("Validation failed");
        }

        Comment comment = await _commentService.CreateReply(model);

        if (comment == null) return BadRequest("Can't create the reply, try again");

        return Ok(comment);
    }

}