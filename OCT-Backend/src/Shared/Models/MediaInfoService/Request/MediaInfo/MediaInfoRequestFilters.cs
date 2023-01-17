namespace Models.MediaInfoService.Request.MediaInfo;

public class MediaInfoRequestFilters
{
    public string Country { get; set; }

    public string[] Genres { get; set; }

    public string NameStartsWith { get; set; }

    public bool? IsTvSerias { get; set; }

    public int Offset { get; set; }

    public int Limit { get; set; } = int.MaxValue;
}
