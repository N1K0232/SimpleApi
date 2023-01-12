using System.Text;

namespace SimpleApi.Security;

public static class StringHasher
{
    public static string GetString(string decodedString)
    {
        byte[] bytes = Convert.FromBase64String(decodedString);
        string originalString = Encoding.UTF8.GetString(bytes);
        return originalString;
    }
}