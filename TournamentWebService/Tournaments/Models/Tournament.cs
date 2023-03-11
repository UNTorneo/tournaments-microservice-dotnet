using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

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
        public string clanId { get; set; }
        public string? venueId { get; set; } = null!;
        public string? venueName { get; set; } = null!;
        public string access { get; set; }
        public string status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set;}

    }
}
