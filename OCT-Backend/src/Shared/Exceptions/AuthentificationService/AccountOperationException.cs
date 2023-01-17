using System.Runtime.Serialization;

namespace Exceptions.AuthentificationService;

public class AccountOperationException : Exception
{
    public AccountOperationException()
    {
    }

    public AccountOperationException(string message) 
        : base(message)
    {
    }

    public AccountOperationException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }

    public AccountOperationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
