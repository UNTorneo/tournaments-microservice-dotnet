namespace TournamentWebService.Matches.Models
{
    public class MatchDataValidation
    {
        public static readonly List<string> matchStatus = new List<string>
        {
            "confirmed",
            "finished",
            "canceled",
            "postponed",
            "playing"
        };
    }

    public enum MatchStatusIndex
    {
        Confirmed,
        Finished,
        Canceled,
        Postponed,
        Playing
    }
}
