namespace Models.UserService.Requests;

public class ChangeUserRequest
{
    public string Name { get; set; }

    public string Surname { get; set; }

    public string MiddleName { get; set; }

    public DateTime Birthday { get; set; }

    public string Description { get; set; }
}
