using AutoMapper;
using SimpleApi.Shared.Models;
using SimpleApi.Shared.Requests;
using Entities = SimpleApi.DataAccessLayer.Entities;

namespace SimpleApi.BusinessLayer.MapperProfiles;

public class PersonMapperProfile : Profile
{
    public PersonMapperProfile()
    {
        CreateMap<Entities.Person, Person>();
        CreateMap<SavePersonRequest, Entities.Person>();
    }
}