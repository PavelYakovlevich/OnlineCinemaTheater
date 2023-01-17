namespace Models.UserService.Responses;

public class GetUserReponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string MiddleName { get; set; }

    public DateTime Birthday { get; set; }

    public string Description { get; set; }
}
