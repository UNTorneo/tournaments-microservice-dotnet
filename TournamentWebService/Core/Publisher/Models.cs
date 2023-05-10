namespace TournamentWebService.Core.Publisher
{
    #region Chat queue message models
    public class StartChatMessage
    {
        public string TYPE { get; set; } = "START_CHAT";
        public StartChatData DATA { get; set; }
    }

    public class StartChatData
    {
        public string room { get; set; }
        public int[] users { get; set; }
        public StartChatData( string room, int[] users)
        {
            this.room = room;
            this.users = users;
        }
    }

    public class EndChatMessage
    {
        public string TYPE { get; set; } = "SAVE_MESSAGES";
        public EndChatData DATA { get; set; }
    }

    public class EndChatData
    {
        public string room { get; set; }
        public EndChatData(string room)
        {
            this.room = room;
        }
    }
    #endregion

    #region Tournament queue message models
    public class StartTournamentMessage
    {
        public string TYPE { get; set; } = "START_CHAT";
        public StartTournamentData DATA { get; set; }
    }

    public class StartTournamentData
    {
        public string room { get; set; }
        public int[] users { get; set; }
        public StartTournamentData(string room, int[] users)
        {
            this.room = room;
            this.users = users;
        }
    }

    public class EndTournamentMessage
    {
        public string TYPE { get; set; } = "SAVE_MESSAGES";
        public EndTournamentData DATA { get; set; }
    }

    public class EndTournamentData
    {
        public string room { get; set; }
        public EndTournamentData(string room)
        {
            this.room = room;
        }
    }
    #endregion
}
