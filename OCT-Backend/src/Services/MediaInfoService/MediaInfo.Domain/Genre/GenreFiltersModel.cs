namespace MediaInfo.Domain.Genre;

public class GenreFiltersModel
{
    private string _nameStartsWith;

    public string NameStartsWith
    {
        get => _nameStartsWith;
        set => _nameStartsWith = value?.ToUpper();
    }
}
