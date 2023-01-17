namespace Models.CommentService.Requests;

public class GetCommentsFilters
{
    public int Offset { get; set; } = 0;

    public int Limit { get; set; } = int.MaxValue;
}
