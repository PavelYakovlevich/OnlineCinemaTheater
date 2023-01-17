using Microsoft.AspNetCore.Http;

namespace Media.Domain;

public class MediaContentModel
{
    public Guid Id { get; set; }

    public Guid MediaId { get; set; }

    public DateTime IssueDate { get; set; }

    public int? Season { get; set; }

    public int? Number { get; set; }

    public IFormFile ContentFile { get; set; }
}
