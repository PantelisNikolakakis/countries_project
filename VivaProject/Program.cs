using Microsoft.EntityFrameworkCore;
using VivaProject.Context;
using VivaProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ICountryService, CountryService>();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("RestCountriesClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(60); // Increase timeout
    client.DefaultRequestVersion = new Version(1, 1); // Force HTTP/1.1
});

builder.Services.AddDbContext<IDataContext, DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// We use EnsureCreated() because we don’t need to handle schema evolution.
// Despite being static, it is simple for our example
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    // Ensure database is created without using migrations
    dbContext.Database.EnsureCreated();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IDataContext>();

    if (!dbContext.Database.CanConnect())
    {
        throw new NotImplementedException("Cannot connect to the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
