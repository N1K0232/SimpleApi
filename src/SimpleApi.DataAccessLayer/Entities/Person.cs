using SimpleApi.DataAccessLayer.Entities.Common;

namespace SimpleApi.DataAccessLayer.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}