using SimpleApi.Security.Models;
using System.Security.Cryptography;

namespace SimpleApi.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;

    private Rfc2898DeriveBytes algorithm;
    private bool disposed;

    public PasswordHasher()
    {
        algorithm = null;
        disposed = false;
    }


    public string Hash(string password)
    {
        ThrowIfDisposed();

        algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA512);

        string key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
        string salt = Convert.ToBase64String(algorithm.Salt);

        string hashedPassword = $"{Iterations}.{salt}.{key}";
        return hashedPassword;
    }

    public PasswordResponse Check(string hash, string password)
    {
        ThrowIfDisposed();

        string[] parts = hash.Split('.', 3);
        if (parts.Length != 3)
        {
            throw new FormatException("invalid string format");
        }

        int iterations = int.Parse(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] key = Convert.FromBase64String(parts[2]);

        bool needsUpgrade = iterations != Iterations;

        algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);

        byte[] keyToCheck = algorithm.GetBytes(KeySize);
        bool verified = keyToCheck.SequenceEqual(key);

        return new(verified, needsUpgrade);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && !disposed)
        {
            if (algorithm != null)
            {
                algorithm.Dispose();
                algorithm = null;
            }

            disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }
}