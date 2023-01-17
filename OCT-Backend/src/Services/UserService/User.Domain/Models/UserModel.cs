namespace User.Domain.Models;

public class UserModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string MiddleName { get; set; }

    public DateTime? Birthday { get; set; }

    public string PhotoLink { get; set; }

    public string Description { get; set; }
}
