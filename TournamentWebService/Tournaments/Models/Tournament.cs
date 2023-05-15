using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using TournamentWebService.Teams.Models;

namespace TournamentWebService.Tournaments.Models
{
    public class Tournament
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string name { get; set; }
        public List<string> teams { get; set; }
        public string sportId { get; set; }
        public string modeId { get; set; }
        public string? clanId { get; set; }
        public string? venueId { get; set; } = null!;
        public string? venueName { get; set; } = null!;
        public string access { get; set; }
        public string status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set;}

    }

    public class Sport
    {
        public string _id { get; set; }
        public string Name { get; set; }
        public string[]? Modes { get; set; }
        public string? Description { get; set; }
        public string[]? Recommendation { get; set; }
    }

    public class Mode
    {
        public string _id { get; set; }
        public string? SportId { get; set; }
        public string? Name { get; set; }
        public int? WinningPoints { get; set; }
        public int? TeamsNumber { get; set; }
        public int? PlayersPerTeam { get; set; }
        public string? Description { get; set; }
        public int? SubstitutePlayers { get; set; }
    }

    public class Clan
    {
        public int id { get; set; }
        public string? leaderId { get; set; }
        public string name { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class Venue
    {
        public int id { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public bool isActive { get; set; }
    }

    public class TournamentPopulated
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string name { get; set; }
        public List<Team> teams { get; set; }
        public Sport sportId { get; set; }
        public Mode modeId { get; set; }
        public Clan? clanId { get; set; }
        public Venue? venueId { get; set; } = null!;
        public string? venueName { get; set; } = null!;
        public string access { get; set; }
        public string status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }

    }
}
