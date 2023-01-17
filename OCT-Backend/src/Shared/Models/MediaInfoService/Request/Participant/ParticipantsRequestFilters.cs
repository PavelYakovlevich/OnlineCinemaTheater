using Enums.MediaInfoService;

namespace Models.MediaInfoService.Request.Participant;

public class ParticipantsRequestFilters
{
    public string NameStartsWith { get; set; }

    public string SurnameStartsWith { get; set; }

    public ParticipantRole? Role { get; set; }

    public int Limit { get; set; } = int.MaxValue;

    public int Offset { get; set; }
}
