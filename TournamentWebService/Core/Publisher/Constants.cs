using System;

namespace TournamentWebService.Core.Publisher
{
    public static class Constants
    {
        public static readonly string mqHost = "localhost";
        public static readonly string matchesQueue = "matches";
        public static readonly string tournamentsQueue = "tournaments";
    }
}
