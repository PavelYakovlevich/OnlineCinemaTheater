using System.Runtime.Serialization;

namespace Exceptions.NotificationService;

public class EmailSendUserException : EmailSendException
{
    public EmailSendUserException()
    {
    }

    public EmailSendUserException(string message)
        : base(message)
    {
    }

    public EmailSendUserException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public EmailSendUserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
