using System.Runtime.Serialization;

namespace Exceptions.UserService;

public class UserOperationException : Exception
{
    public UserOperationException()
    {
    }

    public UserOperationException(string message) 
        : base(message)
    {
    }

    public UserOperationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    protected UserOperationException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}
