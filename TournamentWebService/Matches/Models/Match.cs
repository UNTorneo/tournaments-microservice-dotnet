﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TournamentWebService.Matches.Models
{
    public class Match
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string tournamentId { get; set; }
        public string homeTeam { get; set; }
        public string visitingTeam { get; set; }
        public int homeTeamScore { get; set; } = 0;
        public int visitingTeamScore { get; set; } = 0;
        public DateTime? date { get; set; }
        public string courtId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}