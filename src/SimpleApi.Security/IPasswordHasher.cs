using SimpleApi.Security.Models;

namespace SimpleApi.Security;

public interface IPasswordHasher : IDisposable
{
    string Hash(string password);

    PasswordResponse Check(string hash, string password);
}