using System;

namespace TournamentWebService.Core.Publisher
{
    public static class Constants
    {
        public static readonly string mqUrl = "amqps://ukmbdsfm:dOWlVVt2yuDrb6JvIuQYX9ok0ffjyidW@woodpecker.rmq.cloudamqp.com/ukmbdsfm";
        public static readonly string matchesQueue = "matches";
        public static readonly string tournamentsQueue = "tournaments";
    }
}
