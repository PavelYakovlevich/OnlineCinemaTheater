using Enums.MediaInfoService;

namespace MediaInfo.Domain.Participant;

public class ParticipantFiltersModel
{
    private string _nameStartsWith;
    private string _surnameStartsWith;

    public string NameStartsWith
{
        get => _nameStartsWith;
        set => _nameStartsWith = value?.ToUpper();
    }

    public string SurnameStartsWith
    { 
        get => _surnameStartsWith;
        set => _surnameStartsWith = value?.ToUpper(); 
    }

    public ParticipantRole? Role { get; set; }

    public int Limit { get; set; }

    public int Offset { get; set; }
}
