using AutoMapper;
using FluentValidation;
using OperationResults;
using SimpleApi.BusinessLayer.Services.Interfaces;
using SimpleApi.DataAccessLayer;
using SimpleApi.Shared.Models;
using SimpleApi.Shared.Requests;
using Entities = SimpleApi.DataAccessLayer.Entities;


namespace SimpleApi.BusinessLayer.Services;

public class PeopleService : IPeopleService
{
    private readonly IDataContext dataContext;
    private readonly IMapper mapper;
    private readonly IValidator<SavePersonRequest> personValidator;

    public PeopleService(IDataContext dataContext, IMapper mapper, IValidator<SavePersonRequest> personValidator)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
        this.personValidator = personValidator;
    }


    public async Task<IEnumerable<Person>> GetListAsync()
    {
        var dbPeople = await dataContext.GetListAsync();

        var people = mapper.Map<IEnumerable<Person>>(dbPeople);
        return people;
    }

    public async Task<Result<Person>> SaveAsync(SavePersonRequest request)
    {
        var validationResult = await personValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var validationErrors = new List<ValidationError>();
            foreach (var error in validationResult.Errors)
            {
                validationErrors.Add(new(error.PropertyName, error.PropertyName));
            }

            return Result.Fail(FailureReasons.ClientError, "Invalid request", validationErrors);
        }

        int savedEntries;
        var person = mapper.Map<Entities.Person>(request);

        try
        {
            await dataContext.CreateAsync(person);
            savedEntries = await dataContext.SaveAsync();
        }
        catch (Exception ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }

        if (savedEntries > 0)
        {
            var savedPerson = mapper.Map<Person>(person);
            return savedPerson;
        }
        else
        {
            return Result.Fail(FailureReasons.GenericError, "Couldn't saved entity");
        }
    }
}