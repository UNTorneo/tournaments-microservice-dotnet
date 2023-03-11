using TournamentWebService.Teams.Models;
using TournamentWebService.Teams.Services;
using TournamentWebService.Tournaments.Models;
using TournamentWebService.Tournaments.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TournamentsMongoDBSettings>(builder.Configuration.GetSection("TournamentsMongoDB"));
builder.Services.AddSingleton<TournamentMongoDBService>();
builder.Services.Configure<TeamsMongoDBSettings>(builder.Configuration.GetSection("TeamsMongoDB"));
builder.Services.AddSingleton<TeamMongoDBService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
