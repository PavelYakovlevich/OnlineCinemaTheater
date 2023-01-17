using System.Runtime.Serialization;

namespace Exceptions.NotificationService;

public class EmailSendInternalException : EmailSendException
{
    public EmailSendInternalException()
    {
    }

    public EmailSendInternalException(string message)
        : base(message)
    {
    }

    public EmailSendInternalException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public EmailSendInternalException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
