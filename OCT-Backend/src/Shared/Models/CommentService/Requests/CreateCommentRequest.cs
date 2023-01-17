using Microsoft.AspNetCore.Mvc;

namespace Models.CommentService.Requests;

public class CreateCommentRequest
{
    public Guid UserId { get; set; }

    public string Text { get; set; }
}
