namespace Messages.MediaInfoService;

public class MediaInfoUpdatedMessage
{
    public Guid Id { get; set; }

    public bool IsTvSerias { get; set; }

    public bool IsVisible { get; set; }
}
