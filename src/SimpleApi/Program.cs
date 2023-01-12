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

    services.AddScoped<IDataContext, DataContext>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();
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