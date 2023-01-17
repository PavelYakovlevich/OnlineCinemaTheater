using System.Runtime.Serialization;

namespace Exceptions.AuthentificationService;

public class AccountCreationException : AccountOperationException
{
    public AccountCreationException()
    {
    }

    public AccountCreationException(string message) 
        : base(message)
    {
    }

    public AccountCreationException(string message, string email)
        : this (message, email, null)
    {
    }

    public AccountCreationException(string message, Guid id)
        : this (message, null, id)
    {
    }

    public AccountCreationException(string message, string email, Guid? id)
        : base(message)
    {
        AccountId = id;
        Email = email;
    }

    public AccountCreationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    protected AccountCreationException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }

    public string Email { get; }

    public Guid? AccountId { get; }
}
