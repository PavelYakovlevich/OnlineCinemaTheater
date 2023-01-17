namespace MediaInfo.Domain.Genre;

public class GenreModel
{
    private string _name;

    public Guid Id { get; set; }

    public string Name
    {
        get => _name;
        set => _name = value?.ToUpper();
    }
}