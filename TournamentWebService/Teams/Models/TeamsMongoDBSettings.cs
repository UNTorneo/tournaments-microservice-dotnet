namespace TournamentWebService.Teams.Models
{
    public class TeamsMongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CollectionName { get; set; } = null!;
    }
}
