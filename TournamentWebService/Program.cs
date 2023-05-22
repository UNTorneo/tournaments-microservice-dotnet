using TournamentWebService.Matches.Models;
using TournamentWebService.Matches.Services;
using TournamentWebService.Teams.Models;
using TournamentWebService.Teams.Services;
using TournamentWebService.Tournaments.Models;
using TournamentWebService.Tournaments.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(prefix: "ASPNETCORE_");

// Set port (for container running)
//builder.WebHost.UseUrls("http://*:443");


// Find connectionURI in environment variables (doesn't work)
//TournamentsMongoDBSettings tournamentsMongoDBSettings = builder.Configuration.GetSection("TournamentsMongoDB").Get<TournamentsMongoDBSettings>();
//string tournamentConnectionURI = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_TournamentsMongoDB__ConnectionURI"))
//    ? "No ConnectionURI" : Environment.GetEnvironmentVariable("ASPNETCORE_TournamentsMongoDB__ConnectionURI");
//tournamentsMongoDBSettings.ConnectionURI = tournamentConnectionURI;

//Console.WriteLine("##################### LOGS #####################");
//Console.WriteLine("tournamentConnectionURI: ", tournamentConnectionURI);
//Console.WriteLine("tournamentsMongoDBSettings.ConnectionURI", tournamentsMongoDBSettings.ConnectionURI);


// Configure Kestrel (ports for local debug)
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(443); // to listen for incoming http connection on port 443
    options.ListenAnyIP(7001, configure => configure.UseHttps()); // to listen for incoming https connection on port 7001
});


builder.Services.Configure<TournamentsMongoDBSettings>(builder.Configuration.GetSection("TournamentsMongoDB"));
builder.Services.AddSingleton<TournamentMongoDBService>();
builder.Services.Configure<TeamsMongoDBSettings>(builder.Configuration.GetSection("TeamsMongoDB"));
builder.Services.AddSingleton<TeamMongoDBService>();
builder.Services.Configure<MatchesMongoDBSettings>(builder.Configuration.GetSection("MatchesMongoDB"));
builder.Services.AddSingleton<MatchMongoDBService>();

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
