namespace TournamentWebService.Tournaments.Models
{
    public static class TournamentDataValidation
    {
        public static readonly List<string> tournamentAcces = new List<string>
        {
            "private",
            "public"
        };

        public static readonly List<string> tournamentStatus = new List<string>
        {
            "confirmed",
            "finished",
            "canceled",
            "in progres"
        };
    }
}
