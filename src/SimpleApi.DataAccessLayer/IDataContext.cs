using SimpleApi.DataAccessLayer.Entities;

namespace SimpleApi.DataAccessLayer;

public interface IDataContext : IDisposable
{
    Task<IEnumerable<Person>> GetListAsync();

    Task CreateAsync(Person person);

    Task<int> SaveAsync();
}