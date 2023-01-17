namespace Models.CommentService.Requests;

public class UpdateCommentRequest
{
    public Guid UserId { get; set; }

    public string Text { get; set; }
}
