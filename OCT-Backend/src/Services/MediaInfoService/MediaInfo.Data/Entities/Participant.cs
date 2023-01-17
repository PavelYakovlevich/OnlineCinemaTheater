using Enums.MediaInfoService;

namespace MediaInfo.Data.Entities;

public class Participant
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public DateTime? Birthday { get; set; }

    public string Description { get; set; }

    public ParticipantRole Role { get; set; }

    public string Country { get; set; }
}
