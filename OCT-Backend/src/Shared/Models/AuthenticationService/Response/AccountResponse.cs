using Enums.AuthentificationService;

namespace Models.AuthenticationService.Response;

public class AccountResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool EmailConfirmed { get; set; }

    public AccountRole Role { get; set; }
}
