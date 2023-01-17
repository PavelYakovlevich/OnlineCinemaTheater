using System.Runtime.Serialization;

namespace Exceptions.AuthentificationService;

public class RefreshTokenException : Exception
{
    public RefreshTokenException(string message) 
        : base(message)
    {
    }

    public RefreshTokenException(string message, string token) 
        : this(message)
    {
        Token = token;
    }

    public RefreshTokenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected RefreshTokenException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public string Token { get; }
}
