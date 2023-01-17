using System.Runtime.Serialization;

namespace Exceptions.UserService;

public class BlobOperationException : Exception
{
    public BlobOperationException()
    {
    }

    public BlobOperationException(string message) 
        : base(message)
    {
    }

    public BlobOperationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    protected BlobOperationException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }
}
