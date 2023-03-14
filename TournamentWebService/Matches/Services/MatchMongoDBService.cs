using TournamentWebService.Matches.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;


namespace TournamentWebService.Matches.Services
{
    public class MatchMongoDBService
    {
        private readonly IMongoCollection<Match> _matchesCollection;

        public MatchMongoDBService(IOptions<MatchesMongoDBSettings> matchesMongoDBSettings)
        {
            MongoClient client = new MongoClient(matchesMongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(matchesMongoDBSettings.Value.DatabaseName);
            _matchesCollection = database.GetCollection<Match>(matchesMongoDBSettings.Value.CollectionName);
        }

        public async Task CreateAsync(Match match)
        {
            match.createdAt = DateTime.Now;
            match.updatedAt = DateTime.Now;
            await _matchesCollection.InsertOneAsync(match);
            return;
        }

        public async Task<List<Match>> GetAllAsync()
        {
            return await _matchesCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateAsync(string id, Match match)
        {
            FilterDefinition<Match> filter = Builders<Match>.Filter.Eq("Id", id);
            UpdateDefinition<Match> update = Builders<Match>.Update.Set("updatedAt", DateTime.Now);
            if (match.tournamentId != null)
                update = update.Set(nameof(match.tournamentId), match.tournamentId);
            if (match.teams != null)
                update = update.Set(nameof(match.teams), match.teams);
            if (match.date != null)
                update = update.Set(nameof(match.date), match.date);
            if (match.courtId != null)
                update = update.Set(nameof(match.courtId), match.courtId);
            await _matchesCollection.UpdateOneAsync(filter, update);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Match> filter = Builders<Match>.Filter.Eq("Id", id);
            await _matchesCollection.DeleteOneAsync(filter);
            return;
        }

        public async Task<Match> GetOneAsync(string id)
        {
            //FilterDefinition<Tournament> filter = Builders<Tournament>.Filter.Eq("Id", id);
            return await _matchesCollection.Find(match => match.Id == id).FirstOrDefaultAsync();
        }
    }
}
