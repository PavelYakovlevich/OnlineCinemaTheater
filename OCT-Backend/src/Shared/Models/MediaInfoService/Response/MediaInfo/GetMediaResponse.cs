using Enums.MediaInfoService;
using Models.MediaInfoService.Response.Genre;
using Models.MediaInfoService.Response.Participant;

namespace Models.MediaInfoService.Response.MediaInfo;

public class GetMediaResponse
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

    public IList<ParticipantResponse> Participants { get; set; }

    public IList<GenreResponse> Genres { get; set; }
}
