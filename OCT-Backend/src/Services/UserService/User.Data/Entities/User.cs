namespace User.Data.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string MiddleName { get; set; }

    public DateTime? Birthday { get; set; }

    public string Photo { get; set; }

    public string Description { get; set; }
}
