namespace Messages.UserService;

public class UserUpdatedMessage
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }
}
