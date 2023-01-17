using System.Runtime.Serialization;

namespace Exceptions.NotificationService;

public abstract class EmailSendException : Exception
{
    protected EmailSendException()
    {
    }

    protected EmailSendException(string message)
        : base(message)
    {
    }

    protected EmailSendException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    protected EmailSendException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
