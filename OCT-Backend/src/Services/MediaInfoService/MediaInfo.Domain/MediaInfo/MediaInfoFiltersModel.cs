namespace MediaInfo.Domain.MediaInfo;

public class MediaInfoFiltersModel
{
    private string _country;
    private string _nameStartsWith;

    public string Country
    {
        get => _country;
        set => _country = value?.ToUpper();
    }

    public IEnumerable<string> Genres { get; set; }

    public string NameStartsWith
    { 
        get => _nameStartsWith; 
        set => _nameStartsWith = value?.ToUpper(); 
    }

    public bool? IsTvSerias { get; set; }

    public int Offset { get; set; }

    public int Limit { get; set; }
}
