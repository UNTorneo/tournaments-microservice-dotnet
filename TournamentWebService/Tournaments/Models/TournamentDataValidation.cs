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
            "in-progress"
        };
    }

    public enum TournamentAccessIndex
    {
        Private,
        Public
    }

    public enum TournamentStatusIndex
    {
        Confirmed,
        Finished,
        Canceled,
        InProgres
    }
}
