namespace Authentication.Data.Entities;

public class EmailConfirmationToken
{
    public Guid Id { get; set; }

    public string Token { get; set; }

    public Guid AccountId { get; set; }

    public DateTime IssueDate { get; set; }
}
