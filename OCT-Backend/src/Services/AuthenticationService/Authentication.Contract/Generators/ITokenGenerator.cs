namespace Authentication.Contract.Generators;

public interface ITokenGenerator<T>
{
    string GenerateToken(T value);

    bool ValidateToken(T value, string token);
}
