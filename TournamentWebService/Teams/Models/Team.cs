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
        public string? clanId { get; set; } = null!;
        public List<string> members { get; set; }
        public List<string> tournaments { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class Users
    {
        public string username { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public int id { get; set; }
    }

    public class TeamMembersPopulated
    {
        public TeamMembersPopulated(Team team) {
            Id = team.Id;
            name = team.name;
            clanId = team.clanId;
            members = new List<Users?>();
            createdAt = team.createdAt;
            tournaments = team.tournaments;
            updatedAt = team.updatedAt;
        }

        public string Id { get; set; }
        public string name { get; set; }
        public string? clanId { get; set; } = null!;
        public List<Users?> members { get; set; }
        public List<string> tournaments { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
