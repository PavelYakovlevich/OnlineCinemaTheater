using System.Runtime.Serialization;

namespace Exceptions.AuthentificationService;

public class EmailConfirmationTokenOperationException : Exception
{
    public EmailConfirmationTokenOperationException()
    {
    }

    public EmailConfirmationTokenOperationException(string message)
        : base(message)
    {
    }

    public EmailConfirmationTokenOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected EmailConfirmationTokenOperationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
