using OperationResults.AspNetCore;
using SimpleApi.BusinessLayer.Extensions;
using SimpleApi.BusinessLayer.Services;
using SimpleApi.BusinessLayer.Services.Interfaces;
using SimpleApi.DataAccessLayer;
using SimpleApi.Security;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services);

var app = builder.Build();
Configure(app);

await app.RunAsync();

void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddOperationResult();

    services.AddMapperProfiles();
    services.AddValidators();

    services.AddScoped<IDataContext, DataContext>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();

    services.AddScoped<IPeopleService, PeopleService>();
}

void Configure(IApplicationBuilder app)
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseRouting();

    app.UseHttpsRedirection();
    app.UseAuthorization();


    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}