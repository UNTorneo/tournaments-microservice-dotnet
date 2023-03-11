using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TournamentWebService.Teams.Models
{
    public class Team
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public string? clanId { get; set; }
        public List<string> members { get; set; }
        public List<string> tournaments { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
