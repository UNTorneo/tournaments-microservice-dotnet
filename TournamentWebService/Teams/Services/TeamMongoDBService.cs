using TournamentWebService.Teams.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TournamentWebService.Teams.Services
{
    public class TeamMongoDBService
    {
        private readonly IMongoCollection<Team> _teamsCollection;

        public TeamMongoDBService(IOptions<TeamsMongoDBSettings> teamsMongoDBSettings)
        {
            MongoClient client = new MongoClient(teamsMongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(teamsMongoDBSettings.Value.DatabaseName);
            _teamsCollection = database.GetCollection<Team>(teamsMongoDBSettings.Value.CollectionName);
        }

        public async Task CreateAsync(Team team)
        {
            team.createdAt = DateTime.Now;
            team.updatedAt = DateTime.Now;
            await _teamsCollection.InsertOneAsync(team);
            return;
        }

        public async Task<List<Team>> GetAllAsync()
        {
            return await _teamsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task UpdateAsync(string id, Team team)
        {
            FilterDefinition<Team> filter = Builders<Team>.Filter.Eq("Id", id);
            UpdateDefinition<Team> update = Builders<Team>.Update.Set("updatedAt", DateTime.Now);
            if (team.name != null)
                update = update.Set(nameof(team.name), team.name);
            if (team.clanId != null)
                update = update.Set(nameof(team.clanId), team.clanId);
            if (team.members != null)
                update = update.Set(nameof(team.members), team.members);
            if (team.tournaments != null)
                update = update.Set(nameof(team.tournaments), team.tournaments);
            await _teamsCollection.UpdateOneAsync(filter, update);
            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Team> filter = Builders<Team>.Filter.Eq("Id", id);
            await _teamsCollection.DeleteOneAsync(filter);
            return;
        }

        public async Task<Team> GetOneAsync(string id)
        {
            //FilterDefinition<Tournament> filter = Builders<Tournament>.Filter.Eq("Id", id);
            return await _teamsCollection.Find(team => team.Id == id).FirstOrDefaultAsync();
        }
    }
}
