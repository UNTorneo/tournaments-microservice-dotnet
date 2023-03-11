using TournamentWebService.Tournaments.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TournamentWebService.Tournaments.Services
{
    public class TournamentMongoDBService
    {
        private readonly IMongoCollection<Tournament> _tournamentsCollection;

        public TournamentMongoDBService(IOptions<TournamentsMongoDBSettings> tournamentsMongoDBSettings)
        {
            MongoClient client = new MongoClient(tournamentsMongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(tournamentsMongoDBSettings.Value.DatabaseName);
            _tournamentsCollection = database.GetCollection<Tournament>(tournamentsMongoDBSettings.Value.CollectionName);
        }

        public async Task CreateAsync(Tournament tournament)
        {
            tournament.createdAt = DateTime.Now;
            tournament.updatedAt = DateTime.Now;
            await _tournamentsCollection.InsertOneAsync(tournament);
            return;
        }

        public async Task<List<Tournament>> GetAllAsync()
        {
            return await _tournamentsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateAsync(string id, Tournament tournament)
        {
            FilterDefinition<Tournament> filter = Builders<Tournament>.Filter.Eq("Id", id);
            UpdateDefinition<Tournament> update = Builders<Tournament>.Update.Set("updatedAt", DateTime.Now);
            if (tournament.name !=null)
                update = update.Set(nameof(tournament.name), tournament.name);
            if (tournament.teams !=null)
                update = update.Set(nameof(tournament.teams), tournament.teams);
            if (tournament.sportId != null)
                update = update.Set(nameof(tournament.sportId), tournament.sportId);
            if (tournament.clanId != null)
                update = update.Set(nameof(tournament.clanId), tournament.clanId);
            if (tournament.venueId != null)
                update = update.Set(nameof(tournament.venueId), tournament.venueId);
            if (tournament.venueName != null)
                update = update.Set(nameof(tournament.venueName), tournament.venueName);
            if (tournament.access != null)
                update = update.Set(nameof(tournament.access), tournament.access);
            if (tournament.status != null)
                update = update.Set(nameof(tournament.status), tournament.status);
            await _tournamentsCollection.UpdateOneAsync(filter, update);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Tournament> filter = Builders<Tournament>.Filter.Eq("Id", id);
            await _tournamentsCollection.DeleteOneAsync(filter);
            return;
        }

        public async Task<Tournament> GetOneAsync(string id)
        {
            //FilterDefinition<Tournament> filter = Builders<Tournament>.Filter.Eq("Id", id);
            return await _tournamentsCollection.Find(tournament => tournament.Id == id).FirstOrDefaultAsync();
        }
    }
}
