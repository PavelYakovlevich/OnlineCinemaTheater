using Enums.MediaInfoService;
using MediaInfo.Domain.Genre;
using MediaInfo.Domain.Participant;

namespace MediaInfo.Domain.MediaInfo;

public class MediaInfoModel
{
    private string _name;
    private string _country;

    public Guid Id { get; set; }

    public string Name
    {
        get => _name;
        set => _name = value?.ToUpper();
    }

    public decimal? Budget { get; set; }

    public decimal? Aid { get; set; }

    public string Description { get; set; }

    public DateTime IssueDate { get; set; }

    public bool IsFree { get; set; }

    public AgeRating AgeRating { get; set; }

    public bool IsTvSerias { get; set; }

    public bool IsVisible { get; set; }

    public string Country
    {
        get => _country;
        set => _country = value?.ToUpper(); 
    }

    public IEnumerable<ParticipantModel> Participants { get; set; }

    public IEnumerable<GenreModel> Genres { get; set; }
}
