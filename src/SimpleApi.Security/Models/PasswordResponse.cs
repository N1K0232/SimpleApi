namespace SimpleApi.Security.Models;

public class PasswordResponse
{
    internal PasswordResponse(bool verified, bool needsUpgrade)
    {
        Verified = verified;
        NeedsUpgrade = needsUpgrade;
    }

    public bool Verified { get; }

    public bool NeedsUpgrade { get; }
}