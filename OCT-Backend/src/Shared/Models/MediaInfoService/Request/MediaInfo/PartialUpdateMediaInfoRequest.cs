using Enums.MediaInfoService;

namespace Models.MediaInfoService.Request.MediaInfo;

public class PartialUpdateMediaInfoRequest
{
    public string Name { get; set; }

    public decimal? Budget { get; set; }

    public decimal? Aid { get; set; }

    public string Description { get; set; }

    public DateTime? IssueDate { get; set; }

    public bool? IsFree { get; set; }

    public AgeRating? AgeRating { get; set; }

    public bool? IsTvSerias { get; set; }

    public bool? IsVisible { get; set; }

    public string Country { get; set; }

    public IEnumerable<Guid> Participants { get; set; }

    public IEnumerable<Guid> Genres { get; set; }
}
