using SimpleApi.Shared.Common;

namespace SimpleApi.Shared.Requests;

public class SavePersonRequest : BaseRequestObject
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}