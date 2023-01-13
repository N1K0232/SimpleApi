using SimpleApi.Shared.Common;

namespace SimpleApi.Shared.Models;

public class Person : BaseObject
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}