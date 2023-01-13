using OperationResults;
using SimpleApi.Shared.Models;
using SimpleApi.Shared.Requests;

namespace SimpleApi.BusinessLayer.Services.Interfaces;

public interface IPeopleService
{
    Task<IEnumerable<Person>> GetListAsync();
    Task<Result<Person>> SaveAsync(SavePersonRequest request);
}