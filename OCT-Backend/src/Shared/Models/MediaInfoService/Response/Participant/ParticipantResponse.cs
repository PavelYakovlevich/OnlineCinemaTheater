using Enums.MediaInfoService;

namespace Models.MediaInfoService.Response.Participant;

public class ParticipantResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public DateTime? Birthday { get; set; }

    public string Description { get; set; }

    public ParticipantRole Role { get; set; }

    public string Country { get; set; }
}
