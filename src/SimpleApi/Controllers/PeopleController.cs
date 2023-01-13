using Microsoft.AspNetCore.Mvc;
using SimpleApi.BusinessLayer.Services.Interfaces;
using SimpleApi.Shared.Requests;

namespace SimpleApi.Controllers;

public class PeopleController : ControllerBase
{
    private readonly IPeopleService peopleService;

    public PeopleController(IPeopleService peopleService)
    {
        this.peopleService = peopleService;
    }


    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var people = await peopleService.GetListAsync();
        return Ok(people);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SavePersonRequest request)
    {
        var result = await peopleService.SaveAsync(request);
        return CreateResponse(result, StatusCodes.Status201Created);
    }
}