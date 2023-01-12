namespace SimpleApi.DataAccessLayer;

public interface IDataContext : IDisposable
{
    Task<int> SaveAsync();
}