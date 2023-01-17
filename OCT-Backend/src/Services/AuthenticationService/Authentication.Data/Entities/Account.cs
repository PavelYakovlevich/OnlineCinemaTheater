using Enums.AuthentificationService;

namespace Authentication.Data.Entities;

public class Account
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool EmailConfirmed { get; set; }

    public AccountRole Role { get; set; }
}
