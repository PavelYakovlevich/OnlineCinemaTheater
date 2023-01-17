namespace Authentication.Domain.Models;

public class EmailConfirmationTokenModel
{
    public Guid Id { get; set; }

    public string Token { get; set; }

    public Guid AccountId { get; set; }

    public AccountModel Account { get; set; }

    public DateTime IssueDate { get; set; }
}
