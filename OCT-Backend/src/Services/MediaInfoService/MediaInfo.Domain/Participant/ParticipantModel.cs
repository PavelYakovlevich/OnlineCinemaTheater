using Enums.MediaInfoService;

namespace MediaInfo.Domain.Participant;

public class ParticipantModel
{
    private string _surname;
    private string _name;
    private string _country;

    public Guid Id { get; set; }

    public string Name
    {
        get => _name;
        set => _name = value?.ToUpper();
    }

    public string Surname
    {
        get => _surname;
        set => _surname = value?.ToUpper();
    }

    public DateTime? Birthday { get; set; }

    public string Description { get; set; }

    public ParticipantRole Role { get; set; }

    public string Country
    { 
        get => _country;
        set => _country = value?.ToUpper(); 
    }

    public string Picture { get; set; }
}