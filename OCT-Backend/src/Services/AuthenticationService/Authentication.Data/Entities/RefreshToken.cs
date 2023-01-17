namespace Authentication.Data.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    public string Token { get; set; }

    public DateTime IssueDate { get; set; }

    public Guid AccountId { get; set; }

    public Account Account { get; set; }
}
