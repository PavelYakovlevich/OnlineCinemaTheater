using Enums.AuthentificationService;

namespace Authentication.Domain.Models;

public class AccountModel
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool EmailConfirmed { get; set; }

    public AccountRole Role { get; set; }
}
