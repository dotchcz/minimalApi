using Microsoft.EntityFrameworkCore;
using MinimalApi.ApiModels;
using MinimalApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(c =>
    {
        c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    }
);
builder.Services.AddDbContext<MyHomeAutomationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MyHomeAutomationConn")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(option => option.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapGet("/temperatures",
    async (CancellationToken cancellationToken, MyHomeAutomationDbContext dbContext) =>
    {
        var result = await dbContext.Temperatures
            .Join(
                dbContext.SensorLocations,
                temperature => temperature.SensorId,
                sensorLocation => sensorLocation.SensorId,
                ((temperature, location) => new TemperatureResponse
                {
                    SensorName = temperature.Sensor.Name,
                    Value = temperature.Value,
                    LocationName = location.Location.Name,
                    Created = temperature.Created
                })
            ).OrderByDescending(t => t.Created).ToListAsync(cancellationToken);
        return Results.Ok(result);
    });

app.MapGet("/temperatures/{id:int}",
    async (int id, MyHomeAutomationDbContext dbContext) =>
    {
        var location = await dbContext.Locations.FindAsync(id);
        return location is null ? Results.NotFound() : Results.Ok(location);
    });

app.MapPost("/temperature",
        async (
            TemperatureRequest requestTemperature,
            MyHomeAutomationDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            Console.WriteLine($"Input: {requestTemperature.SensorId}, {requestTemperature.Value}");

            await dbContext.Temperatures.AddAsync(new Temperature()
            {
                Created = DateTime.UtcNow,
                SensorId = requestTemperature.SensorId,
                Value = decimal.Parse(requestTemperature.Value)
            }, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        })
    .WithName("PostTemperature");

app.Run();

public class MyHomeAutomationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    public MyHomeAutomationDbContext(DbContextOptions<MyHomeAutomationDbContext> options) : base(options)
    {
    }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Temperature> Temperatures => Set<Temperature>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<SensorLocation> SensorLocations => Set<SensorLocation>();
}