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
            if (match.homeTeam != null)
                update = update.Set(nameof(match.homeTeam), match.homeTeam);
            if (match.visitingTeam != null)
                update = update.Set(nameof(match.visitingTeam), match.visitingTeam);
            update = update.Set(nameof(match.homeTeamScore), match.homeTeamScore);
            update = update.Set(nameof(match.visitingTeamScore), match.visitingTeamScore);
            if (match.date != null)
                update = update.Set(nameof(match.date), match.date);
            if (match.courtId != null)
                update = update.Set(nameof(match.courtId), match.courtId);
            if (match.status != null)
                update = update.Set(nameof(match.status), match.status);
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

        public async Task<List<Match>> GetMatchesByTournamentAsync(string id)
        {
            //FilterDefinition<Tournament> filter = Builders<Tournament>.Filter.Eq("Id", id);
            return await _matchesCollection.Find(match => match.tournamentId == id).ToListAsync();
        }

        public async Task<List<Match>> GetMatchesByTeamAsync(string id)
        {
            return await _matchesCollection.Find(match => match.visitingTeam == id || match.homeTeam == id).ToListAsync();
        }

    }
}
