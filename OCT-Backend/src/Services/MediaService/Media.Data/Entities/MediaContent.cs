namespace Media.Data.Entities;

public class MediaContent
{
    public Guid Id { get; set; }

    public Guid MediaId { get; set; }

    public DateTime IssueDate { get; set; }

    public int? Number { get; set; }
}
