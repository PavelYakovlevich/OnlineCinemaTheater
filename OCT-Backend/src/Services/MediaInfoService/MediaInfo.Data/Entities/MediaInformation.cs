using Enums.MediaInfoService;

namespace MediaInfo.Data.Entities;

public class MediaInformation
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal? Budget { get; set; }

    public decimal? Aid { get; set; }

    public string Description { get; set; }

    public DateTime IssueDate { get; set; }

    public bool IsFree { get; set; }

    public AgeRating AgeRating { get; set; }

    public bool IsTvSerias { get; set; }

    public bool IsVisible { get; set; }

    public string Country { get; set; }

    public IList<Participant> Participants { get; set; }

    public IList<Genre> Genres { get; set; }
}
