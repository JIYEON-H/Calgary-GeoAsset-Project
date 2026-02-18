using GeoAsset.Api.Data;
using GeoAsset.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Temporary disable HTTPS redirection while developing
// app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Apply pending migrations (e.g. when running in Docker)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    if (!db.Assets.Any())
    {
        db.Assets.AddRange(new List<Asset>
        {
            new Asset { Name = "Deerfoot Solar Array North", Type = "Solar", Latitude = 51.0823, Longitude = -113.9872, Status = "Active", LastUpdated = DateTime.Parse("2025-02-10T14:22:00Z"), 
            InspectionLogs = new List<InspectionLog> 
            {
                new InspectionLog { InspectionDate = DateTime.Parse("2025-01-15"), InspectorName = "John Doe", Notes = "All panels cleaned." },
                new InspectionLog { InspectionDate = DateTime.Parse("2025-02-10"), InspectorName = "Jane Smith", Notes = "Inverter check passed." }
            }},
            new Asset { Name = "Bow River Substation A", Type = "Substation", Latitude = 51.0447, Longitude = -114.0719, Status = "Maintenance", LastUpdated = DateTime.Parse("2024-12-08T09:15:00Z"),
            InspectionLogs = new List<InspectionLog>
            {
                new InspectionLog { InspectionDate = DateTime.Parse("2024-12-08"), InspectorName = "Mike Ross", Notes = "Cooling system needs repair." }
            }},
            new Asset { Name = "Crowfoot Wind Turbine Cluster", Type = "Wind", Latitude = 51.1289, Longitude = -114.2134, Status = "Active", LastUpdated = DateTime.Parse("2025-02-01T11:00:00Z") },
            new Asset { Name = "Downtown Calgary District Heating Plant", Type = "District Energy", Latitude = 51.0486, Longitude = -114.0708, Status = "Active", LastUpdated = DateTime.Parse("2025-02-15T08:30:00Z") },
            new Asset { Name = "Glenmore Reservoir Hydro Unit 2", Type = "Hydro", Latitude = 50.9845, Longitude = -114.0923, Status = "Offline", LastUpdated = DateTime.Parse("2024-12-12T16:45:00Z") },
            new Asset { Name = "Macleod Trail Battery Storage", Type = "Battery Storage", Latitude = 51.0312, Longitude = -114.0589, Status = "Active", LastUpdated = DateTime.Parse("2025-02-14T07:00:00Z") },
            new Asset { Name = "Stoney Trail East Transformer Station", Type = "Substation", Latitude = 51.0678, Longitude = -113.8912, Status = "Maintenance", LastUpdated = DateTime.Parse("2024-11-20T13:20:00Z") },
            new Asset { Name = "Fish Creek Park Solar Farm", Type = "Solar", Latitude = 50.9123, Longitude = -114.0345, Status = "Active", LastUpdated = DateTime.Parse("2025-01-28T12:00:00Z") },
            new Asset { Name = "Inglewood Biomass CHP", Type = "Biomass", Latitude = 51.0389, Longitude = -114.0012, Status = "Offline", LastUpdated = DateTime.Parse("2024-12-18T10:30:00Z") },
            new Asset { Name = "Saddledome Arena Solar Rooftop", Type = "Solar", Latitude = 51.0372, Longitude = -114.0523, Status = "Active", LastUpdated = DateTime.Parse("2025-02-16T09:15:00Z") },
            new Asset { Name = "Calgary Airport Microgrid", Type = "Microgrid", Latitude = 51.1134, Longitude = -114.0203, Status = "Active", LastUpdated = DateTime.Parse("2025-02-11T06:00:00Z") },
            new Asset { Name = "Nose Hill Wind Sensor Station", Type = "Wind", Latitude = 51.1098, Longitude = -114.1245, Status = "Maintenance", LastUpdated = DateTime.Parse("2024-12-01T14:00:00Z") },
            new Asset { Name = "Shepard Energy Centre Gas Unit 3", Type = "Natural Gas", Latitude = 50.9456, Longitude = -113.9234, Status = "Active", LastUpdated = DateTime.Parse("2025-02-13T04:30:00Z") },
            new Asset { Name = "West Hills Distribution Hub", Type = "Substation", Latitude = 51.0567, Longitude = -114.1987, Status = "Offline", LastUpdated = DateTime.Parse("2024-11-28T11:00:00Z") },
            new Asset { Name = "University of Calgary Solar Carport", Type = "Solar", Latitude = 51.0789, Longitude = -114.1312, Status = "Active", LastUpdated = DateTime.Parse("2025-01-15T15:45:00Z") },
            new Asset { Name = "East Village District Cooling", Type = "District Energy", Latitude = 51.0467, Longitude = -114.0412, Status = "Active", LastUpdated = DateTime.Parse("2025-02-09T10:20:00Z") },
            new Asset { Name = "Springbank ESS Pilot", Type = "Battery Storage", Latitude = 51.0923, Longitude = -114.2678, Status = "Maintenance", LastUpdated = DateTime.Parse("2024-12-22T08:00:00Z") },
            new Asset { Name = "McKenzie Towne Solar Community", Type = "Solar", Latitude = 50.8934, Longitude = -113.9678, Status = "Active", LastUpdated = DateTime.Parse("2025-02-05T13:30:00Z") },
            new Asset { Name = "Bowness Feeder Station", Type = "Substation", Latitude = 51.0987, Longitude = -114.2134, Status = "Offline", LastUpdated = DateTime.Parse("2024-12-05T17:00:00Z") },
            new Asset { Name = "Calgary Transit CTrain Solar Canopy", Type = "Solar", Latitude = 51.0623, Longitude = -114.0789, Status = "Active", LastUpdated = DateTime.Parse("2025-01-22T09:00:00Z") }
        });
        db.SaveChanges();
    }
}

app.Run();
