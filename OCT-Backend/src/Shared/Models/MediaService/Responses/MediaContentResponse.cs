namespace Models.MediaService.Responses;

public class MediaContentResponse
{
    public Guid Id { get; set; }

    public Guid MediaId { get; set; }

    public DateTime IssueDate { get; set; }

    public int? Season { get; set; }

    public int? Number { get; set; }
}
