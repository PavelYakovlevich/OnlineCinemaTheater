namespace Comment.Domain;

public class CommentModel
{
    public Guid Id { get; set; }

    public Guid MediaId { get; set; }

    public Guid UserId { get; set; }

    public string Text { get; set; }

    public DateTime IssueDate { get; set; }

    public UserModel User { get; set; }
}
